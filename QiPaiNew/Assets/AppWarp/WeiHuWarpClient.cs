using UnityEngine;
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MsgPack;

public class WeiHuWarpClient : MonoBehaviour
{
    public static WeiHuWarpClient wc;
    public static TimeOutHelper timeOutHelper = new TimeOutHelper();
    public static WarpConnectionState currentState = WarpConnectionState.DISCONNECTED;

    public int sessionId;

    public string WeiHuTemp;

    void Awake()
    {
        wc = this;
    }

    public void ConnectedToServer(bool isReConnect, Action actionOnConnected)
    {
        OGUIM.Toast.ShowLoading("Đang kết nối máy chủ...");
        WeiHuWarpClient.currentState = WarpConnectionState.CONNECTING;
        WeiHuWarpChannel.Channel.socket_connect(actionOnConnected);
    }

    #region Socket

    private byte[] currentData;
    private int readedCount;
    private BinaryReader reader;

    public void ResponseData(byte[] data)
    {
        if (data != null && data.Length > 0)
        {
            if (currentData == null)
                currentData = data;
            else
                currentData = currentData.Concat(data).ToArray();

        }
        if (currentData == null || currentData.Length <= 0)
            return;
        if (reader == null)
        {
            try
            {
                var temp = new byte[currentData.Length];
                Array.Copy(currentData, temp, currentData.Length);
                reader = new BinaryReader(new MemoryStream(temp));
                StartCoroutine(ResponseData(reader));
            }
            catch (Exception ex)
            {
                UILogView.Log("ResponseData(byte[] data) throw: " + ex.Message, true);
            }
            finally
            {

                if (reader != null)
#if !NETFX_CORE
                    reader.Close();
#else
                    reader.Dispose();
#endif
                reader = null;
            }
        }
    }

    IEnumerator ResponseData(BinaryReader reader)
    {
        var calltype = reader.ReadByte();
        if (calltype == (int)WarpMessageTypeCode.RESPONSE)
        {
            byte requestType = reader.ReadByte();
            byte resultCode = reader.ReadByte();
            byte reserved = reader.ReadByte();
            byte payLoadType = reader.ReadByte();

            byte[] payLoadSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            int payLoadSize = BitConverter.ToInt32(payLoadSizeBytes, 0);

            int readableSize = (int)reader.BaseStream.Length;
            if (payLoadSize + 9 <= readableSize)
            {
                byte[] payLoadBytes = reader.ReadBytes(payLoadSize);
                OnResponse((WarpRequestTypeCode)requestType, resultCode, payLoadBytes);
                // Reset last send time when receive data successfull
                WeiHuWarpChannel.Channel.tcpLastSendTime = Time.time;
                readedCount += payLoadSize + 9;
                if (readableSize - readedCount > 9)
                    ResponseData(reader);
            }
            else
            {
            }
        }
        else if (calltype == (int)WarpMessageTypeCode.UPDATE)
        {
            byte notifyType = reader.ReadByte();
            byte reserved = reader.ReadByte();
            byte payLoadType = reader.ReadByte();

            byte[] payLoadSizeBytes = reader.ReadBytes(4).Reverse().ToArray();
            int payLoadSize = BitConverter.ToInt32(payLoadSizeBytes, 0);

            int readableSize = (int)reader.BaseStream.Length;
            if (payLoadSize + 8 <= readableSize)
            {
                byte[] payLoadBytes = reader.ReadBytes(payLoadSize);
                OnNotify((WarpNotifyTypeCode)notifyType, payLoadBytes);
                // Reset last send time when receive data successfull
                WeiHuWarpChannel.Channel.tcpLastSendTime = Time.time;
                readedCount += payLoadSize + 8;
                if (readableSize - readedCount > 8)
                    ResponseData(reader);
            }
            else
            {

            }
        }
        else
        {
            currentData = null;
            readedCount = 0;
        }

        if (readedCount > 0 && currentData != null && currentData.Length > 0)
        {
            currentData = currentData.Skip(readedCount).ToArray();
            readedCount = 0;
        }
        yield return 0;
    }
    #endregion



    #region Notify
    private void OnNotify(WarpNotifyTypeCode notifyType, byte[] payLoadBytes)
    {
        UILogView.Log("----- OnNotify :" + notifyType.ToString());

        try
        {
            
        }
        catch (Exception ex)
        {
            UILogView.Log("WarpClient: OnNotify: " + ex.Message + " " + ex.StackTrace, true);
        }
    }
    #endregion

    public delegate void WeiHuDelegate(WarpResponseResultCode status, string data);
    public event WeiHuDelegate OnWeiHu;

    #region Response
    private void OnResponse(WarpRequestTypeCode requestType, int resultCode, byte[] payLoad)
    {

        try
        {
           

            if (requestType == WarpRequestTypeCode.SEND_WEIHU)
            {
                StopTimeOut(requestType);
                var dataObject = ZenMessagePack.DeserializeObject<string>(payLoad, WarpContentTypeCode.MESSAGE_PACK);
                var raw = Unpacking.UnpackObject(payLoad).Value.ToString();
                WeiHuTemp = raw.ToString();

                if (OnWeiHu != null)
                    OnWeiHu((WarpResponseResultCode)resultCode, dataObject);
            }

        }
        catch (Exception ex)
        {
            UILogView.Log("WarpClient: OnResponse: " + ex.Message + " " + ex.StackTrace, true);
        }
    }
    #endregion

    #region Prive Method
    void SendRequest(int calltype, int requestType, int requestId, long sessionId, int reserved, WarpContentTypeCode payloadType, MemoryStream stream)
    {
        try
        {
            if (WeiHuWarpChannel.Channel != null && WeiHuWarpChannel.Channel.client_socket != null && WeiHuWarpChannel.Channel.client_socket.isConnected)
            {
                BinaryWriter writer = new BinaryWriter(new MemoryStream());
                var payLoadBytes = stream.ToArray();
                writer.Write((byte)calltype);
                writer.Write((byte)requestType);
                writer.Write(BitConverter.GetBytes((int)sessionId).Reverse().ToArray());
                writer.Write(BitConverter.GetBytes(requestId).Reverse().ToArray());
                writer.Write((byte)reserved);
                writer.Write((byte)payloadType);
                writer.Write(BitConverter.GetBytes(payLoadBytes.Length).Reverse().ToArray());
                writer.Write(payLoadBytes);

                byte[] bytes = (writer.BaseStream as MemoryStream).ToArray();
                WeiHuWarpChannel.Channel.socket_send(bytes);
            }
            else
            {
                Debug.Log("Socket write failed with error:  " + currentState);
                if (sessionId != 0 && currentState != WarpConnectionState.RECOVERING)
                    currentState = WarpConnectionState.RECOVERING;
            }
        }
        catch (Exception exp)
        {
            UILogView.Log("Socket write failed with Exception:  " + exp.Message, true);
        }
    }

    void SendRequest(int requestTypeCode, MemoryStream stream)
    {
        var requestId = 0;
        var reserved = 0;
        if (stream != null)
            SendRequest((int)WarpMessageTypeCode.REQUEST, requestTypeCode, requestId, sessionId, reserved, WarpContentTypeCode.MESSAGE_PACK, stream);
        else
            SendRequest((int)WarpMessageTypeCode.REQUEST, requestTypeCode, requestId, sessionId, reserved, WarpContentTypeCode.MESSAGE_PACK, new ZenDictionary { str = "z" }.ToStream());
    }

    IEnumerator CreateTimeOut(float timeOutDuration, Action onTimeOut)
    {
        yield return new WaitForSeconds(timeOutDuration);
        onTimeOut();
    }

    void StopTimeOut(object requestType)
    {
        OGUIM.Toast.deactvie.SetActive(false);
        Coroutine handle;
        if (timeOutHelper.TryGetValue(requestType + "", out handle))
        {
            StopCoroutine(handle);
            timeOutHelper.Remove(requestType + "");
        }
    }
    #endregion

    #region Public Method
    public void Send(object requestTypeCode, MemoryStream data, Action actionTimeOut = null, float timeOutDuration = -1)
    {
        if (timeOutHelper.IsWorking(requestTypeCode.ToString()))
        {
            UILogView.Log(requestTypeCode.ToString() + " is requesting...");
        }
        else
        {
            if (actionTimeOut != null)
            {
                var type = requestTypeCode;
                var timeOutHandle = CreateTimeOut(timeOutDuration == -1 ? WeiHuWarpChannel.Channel.recieveTimeOut : timeOutDuration, () =>
                {
                    StopTimeOut(type);
                    actionTimeOut();
                });
                timeOutHelper.Start(requestTypeCode.ToString(), StartCoroutine(timeOutHandle));
            }
            SendRequest((int)requestTypeCode, data);
        }
    }

    public void Send(object requestTypeCode, object notifyTypeCode, MemoryStream data, Action onTimeOutAction, float timeOutDuration = -1)
    {
        if (timeOutHelper.IsWorking(notifyTypeCode.ToString()))
        {
            UILogView.Log(notifyTypeCode.ToString() + " is requesting..", true);
        }
        else
        {
            if (onTimeOutAction != null)
            {
                var timeOutHandle = CreateTimeOut(timeOutDuration == -1 ? WeiHuWarpChannel.Channel.recieveTimeOut : timeOutDuration, () =>
                {
                    StopTimeOut(notifyTypeCode);
                    onTimeOutAction();
                });
                timeOutHelper.Start(notifyTypeCode.ToString(), StartCoroutine(timeOutHandle));
            }
            SendRequest((int)requestTypeCode, data);
        }
    }
    #endregion

    #region Fake Method
    public void FakeGetLobbyInfo()
    {
    }
    #endregion
}