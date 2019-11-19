using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Threading;

#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using TcpClient = BestHTTP.PlatformSupport.TcpClient.WinRT.TcpClient;
//Disable CD4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable 4014
#elif UNITY_WP8 && !UNITY_EDITOR
using TcpClient = BestHTTP.PlatformSupport.TcpClient.WP8.TcpClient;
#else
using TcpClient = System.Net.Sockets.TcpClient;
#endif
public class TcpSocket
{
    private string tcpAddress { get; set; }
    private int tcpPort { get; set; }

    public TcpClient socket { get; set; }
    Stream theStream { get; set; }


#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR
#else
    Thread m_thread;
#endif
    private static object receiveLockObj = new object();

    BinaryWriter theWriter;
    BinaryReader theReader;

    string m_Error = "NULL";
    private Queue<byte[]> m_receives;


    public TcpSocket(string address, int port)
    {
        tcpAddress = address;
        tcpPort = port;
        m_receives = new Queue<byte[]>();
    }

    public IEnumerator SocketConnectAsync(bool isReConnect, Action actionOnConnected)
    {
        Setup();
        while (socket == null || !socket.Connected || theStream == null || !theStream.CanRead)
        {
            if (!isReConnect)
                WarpClient.currentState = WarpConnectionState.CONNECTING;
            Debug.LogWarning(WarpClient.currentState.ToString());
            yield return null;
        }
        UILogView.Log(WarpClient.currentState + " " + tcpAddress + ":" + tcpPort);
        WarpClient.currentState = WarpConnectionState.CONNECTED;
        if (actionOnConnected != null)
            actionOnConnected();
    }

    private void Setup()
    {
        m_Error = "";
        try
        {
            socket = new TcpClient(tcpAddress, tcpPort);
            socket.SendTimeout = 3000;
            socket.SendBufferSize = 1024;
            theStream = socket.GetStream();
            theWriter = new BinaryWriter(theStream);
            theReader = new BinaryReader(theStream);

#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR
            Windows.System.Threading.ThreadPool.RunAsync(Receive);
#else
            if (m_thread == null)
            {
                m_thread = new Thread(Receive);
            }

            if (m_thread.ThreadState != ThreadState.Running)
            {
                m_thread.IsBackground = true;
                m_thread.Start();
            }
#endif

        }
        catch (Exception ex)
        {
            Close();
            Debug.LogError("Socket error:" + ex);
        }
    }

    public
#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR
        async
#endif
        void Receive(object obj)
    {
        while (socket != null)
        {
            byte[] buffer = new byte[8096];
            var byteRead = theReader.Read(buffer, 0, buffer.Length);
            if (byteRead > 0)
            {
                buffer = buffer.Take(byteRead).ToArray();
                m_receives.Enqueue(buffer);
            }
        }
        Debug.LogError("recv is ended....");

#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR

#else
        lock (m_thread)
        {
            m_thread.Abort();
            m_thread = null;
        }
#endif
    }

    public void Send(byte[] buffer)
    {
        try
        {
            if (socket.Connected)
            {
                theWriter.Write(buffer);
                theWriter.Flush();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error:" + e);
        }
    }


    public byte[] Recv()
    {
        try
        {
            lock (receiveLockObj)
            {
                if (m_receives.Count > 0)
                {
                    return m_receives.Dequeue();
                }
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Socket Recv error:" + e);
        }
        return null;
    }

    public void Close()
    {
        try
        {
            m_receives.Clear();

            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR
            if (theWriter != null)
                theWriter.Dispose();
            if (theReader != null)
                theReader.Dispose();
#else
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }

            if (theWriter != null)
                theWriter.Close();
            if (theReader != null)
                theReader.Close();
#endif
        }
        catch (Exception e)
        {
            UILogView.Log("TCPSocket Close error: " + e, true);
        }
    }

    public string error
    {
        get
        {
            return m_Error;
        }
    }

}
