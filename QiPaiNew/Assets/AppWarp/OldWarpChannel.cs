using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class OldWarpChannel : MonoBehaviour
{

    public static OldWarpChannel Channel;

#if UNITY_WEBGL
    public WebSocket cSocket { get; set; }
	private static string httpRequestServer = "http://192.168.1.95:8181/api/";
	private static string prod_server = "ws://192.168.1.95:8889/websocket";
    private static int prod_port = 8888;
#else
    private static string httpRequestServer = "http://192.168.1.95:8181/api/";
    private static string prod_server = "ws://192.168.1.95:8889/websocket";
    private static int prod_port = 8888;
#endif

    public static string warp_host = prod_server;
    public static int warp_port = prod_port;
    public static string httpRequest = httpRequestServer;

#if UNITY_WEBGL
    public WebSocket client_socket = null;
#else
    public WebSocket client_socket = null;
#endif
    public float recieveDelay = 1;
    public float recieveTimeOut = 5;
    public string err;
    public bool isConnected = false;
    public bool setReceive = false;
    public int tryToConnectCount = 0;
    public int minutesToStopSocketCount = 10;
    private float firstTimeConnect;

    public Action actionOnConnected;

    [HideInInspector]
    public bool getKickedOut = false;
    [HideInInspector]
    public float timeToRecovery = 0;
    [HideInInspector]
    public float timeToConnecting = 0;
    // For Reconnecting 
    [HideInInspector]
    public bool isInGame = false;
    [HideInInspector]
    public bool getApiFailed = false;
    [HideInInspector]
    public int countPendingKeepAlives = 0; // can access from TemplateWarpRequestBuilder for SignOut

    public int countPendingRecovery = 0;
    private bool isRecovery = false;
    public float tcpLastSendTime;
	private DateTime recieveLastTime;
	private float receiveLastTime;
    private int status;

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    void Awake()
    {
        if (Channel != null)
            Destroy(Channel.gameObject);

        Channel = this;
        DontDestroyOnLoad(gameObject);

        InvokeRepeating("DataReceive", 1, recieveDelay);
    }

    public void DataReceive()
    {
		if (isConnected == true )
        {
            // Neu buffer con du lieu thi giai tiep, neu het du lieu thi doi them 10 lan receive delay nua (giam so lan goi ham)
#if UNITY_ANDROID || UNITY_IOS
            //if ((Time.time - receiveLastTime > recieveDelay * 10 && !client_socket.bufferRemain) || client_socket.bufferRemain) 
            if (Time.time - receiveLastTime > recieveDelay)
#else
			if (Time.time - receiveLastTime > recieveDelay) 
#endif
            {
				//Debug.Log ("DIFF: " + (Time.time - receiveLastTime));
				recieveLastTime = DateTime.Now;
				receiveLastTime = Time.time;
				socket_recv ();
			}
        }
    }

    public void socket_recovering(bool reset)
    {
        if (WarpClient.currentState == WarpConnectionState.RECOVERING && isConnected == false)
        {
            if (reset)
            {
                client_socket.Close();
                client_socket = null;
            }

            if (client_socket == null)
            {
                if (connectCoroutine != null)
                {
                    StopCoroutine(connectCoroutine);
                    connectCoroutine = null;
                }
#if UNITY_WEBGL
				string uri = warp_host;
                client_socket = new WebSocket(new Uri(uri));
#else
                client_socket = new WebSocket(new Uri(warp_host));
#endif

                connectCoroutine = client_socket.Connect();
                StartCoroutine(connectCoroutine);
                firstTimeConnect = Time.time;
            }
            else
            {
                if (client_socket.isConnected)
                {
                    this.isConnected = true;
                    if (actionOnConnected != null)
                        actionOnConnected();
                }
            }
        }
    }

    IEnumerator connectCoroutine;
    public void socket_connect(Action onConnected = null)
    {
        if (onConnected != null)
            actionOnConnected = onConnected;
        if (WarpClient.currentState == WarpConnectionState.CONNECTING && this.isConnected == false)
        {
            if (client_socket == null)
            {
                if (connectCoroutine != null)
                {
                    StopCoroutine(connectCoroutine);
                    connectCoroutine = null;
                }
				#if UNITY_WEBGL
                string uri = warp_host;
				client_socket = new WebSocket(new Uri(uri));
				UILogView.Log("CONNECTING TO " + warp_host);
				#else
				client_socket = new WebSocket(new Uri(warp_host));
				UILogView.Log("CONNECTING TO " + warp_host + ":" + warp_port);
				#endif


                connectCoroutine = client_socket.Connect();
                StartCoroutine(connectCoroutine);
                firstTimeConnect = Time.time;

            }
            else
            {
                if (client_socket.isConnected)
				{
					#if UNITY_WEBGL
					UILogView.Log("CONNECTED TO " + warp_host);
					#else
					UILogView.Log("CONNECTED TO " + warp_host + ":" + warp_port);
					#endif
                    this.isConnected = true;
                    WarpClient.currentState = WarpConnectionState.CONNECTED;
              
                    if (actionOnConnected != null)
                        actionOnConnected();
                }
                else
                {
                    if (!string.IsNullOrEmpty(client_socket.error))
                    {
                        UILogView.Log(client_socket.error, true);

                        serverCloseSocket();
                    }
                }
            }
        }
        else if (client_socket.isConnected)
        {
            UILogView.Log("CONNECTED TO " + warp_host + ":" + warp_port);
            if (actionOnConnected != null)
                actionOnConnected();
        }
    }

    public void socket_close()
    {
        if (connectCoroutine != null)
        {
            StopCoroutine(connectCoroutine);
            connectCoroutine = null;
        }
        if (client_socket != null)
            client_socket.Close();
        client_socket = null;
        setReceive = false;
        isConnected = false;
        WarpClient.currentState = (int)WarpConnectionState.DISCONNECTED;
    }

    public void socket_recv()
    {
        if (client_socket == null)
        {
            if (WarpClient.currentState != WarpConnectionState.RECOVERING)
            {
                serverCloseSocket();
            }
        }
        if (!string.IsNullOrEmpty(client_socket.error) && err != client_socket.error)
        {
            err = client_socket.error;
            UILogView.Log("Error: " + err);
            UILogView.Log("client socket not null but have error then close socket");
            serverCloseSocket();
            return;
        }


        byte[] reply = client_socket.Recv();
		WarpClient.wc.ResponseData (reply);


        if ((client_socket == null || client_socket.isConnected == false) && WarpClient.currentState != WarpConnectionState.DISCONNECTED)
        {
            Debug.Log("++++++++++++++++++++ DISCONNECT FROM IN GAME ++++++++++++++++++++++++++");
            Debug.Log("set status to RECOVERING");
            WarpClient.currentState = WarpConnectionState.RECOVERING;
            socket_close();
            isConnected = false;
        }
    }

    public void socket_send(byte[] buffer)
    {
        if (client_socket != null && this.isConnected == true)
            client_socket.Send(buffer);
    }

    public void socket_disconnect()
    {
        isConnected = false;
        client_socket = null;
    }

    private void serverCloseSocket()
    {

        Debug.Log(" **************** SERVER CLOSED SOCKET *****************");
        WarpClient.currentState = (int)WarpConnectionState.DISCONNECTED;
        countPendingKeepAlives = 0;
        timeToRecovery = Time.time;
        isRecovery = true;
        isConnected = false;
        getKickedOut = true;
        onLostConnection(0);
        socket_close();
    }

    public void onLostConnection(int status)
    {
        Debug.Log("onLostConnection");
        string text_lost_connection = "Kết nối với máy chủ thất bại! Vui lòng kiểm tra lại kết nối Internet";
        if (status == 2)
            text_lost_connection = "Quá thời gian kết nối. vui lòng đăng nhập lại";
        else if (status == 3)
            text_lost_connection = "Phiên làm việc hết hạn, vui lòng đăng nhập lại";
        else if (status == 4)
            text_lost_connection = "Có lỗi xảy ra, vui lòng đăng nhập lại";

        socket_disconnect();
        WarpClient.currentState = WarpConnectionState.DISCONNECTED;
        try
        {

            if (WarpClient.wc == null || WarpClient.wc.sessionId != 0)
                OGUIM.UnLoadGameScene(true);
            else
                //OGUIM.MessengerBox.Show("Kết nối máy chủ thất bại!", "\n\n" + "Có vẻ như như không có tín hiệu Internet!?" + "\n" + "Vui lòng kiểm tra kết nối 3G hoặc WIFI và thử lại");
                OGUIM.MessengerBox.Show("Thông Báo", "Kết nối thất bại, vui lòng kiểm tra mạng");


          OGUIM.Toast.Show(text_lost_connection, UIToast.ToastType.Warning);
        }
        catch (Exception ex)
        {
            UILogView.Log("onLostConnection: " + ex.Message + " " + ex.StackTrace, true);
        }
    }

    void OnApplicationQuit()
    {
        // Call logout instead of socket close cuz need to inform server that you are quit, not for the client
        WarpRequest.Logout();
        //socket_close();
        // fix for Unity Hangout after Stop from Play
#if UNITY_EDITOR
        socket_close();
#endif
        UILogView.Log("Application ending after " + Time.time + " seconds");
    }

    private void OnApplicationPause(bool pause)
    {
        UILogView.Log("Pause: " + pause + " " + recieveLastTime.ToString("HH:mm") + " " + (DateTime.Now - recieveLastTime).Minutes + " s ago!!!!!!!!!!!!!!", true);
        //if (!pause && !OGUIM.isTheFirst && recieveLastTime.AddMinutes(minutesToStopSocketCount) < DateTime.Now)
        //{
        //    serverCloseSocket();
        //    OGUIM.UnLoadGameScene(true);
        //}
    }

    void Update()
    {
        if (isConnected == false && WarpClient.currentState == WarpConnectionState.CONNECTING)
        {
            timeToConnecting += Time.deltaTime;
            socket_connect();
            if (timeToConnecting >= 2)
            {
                OGUIM.Toast.Show("Không thể kết nối", UIToast.ToastType.Warning);
                timeToConnecting = 0;
            }
        }

        float diff = Time.time - tcpLastSendTime;
        if ((WarpClient.currentState == WarpConnectionState.CONNECTED && (diff > 4)) || ((WarpClient.currentState == WarpConnectionState.RECOVERING) && (diff > 2)))
        {
            if (WarpClient.currentState == WarpConnectionState.RECOVERING)
            {
                socket_recovering(countPendingRecovery % 3 == 1);
                OGUIM.Toast.Show("Mất kết nối, đang thử lại ...", UIToast.ToastType.Warning);
            }
            WarpRequest.SendKeepAlive();
            tcpLastSendTime = Time.time;
            incrementKeepAlives();
        }

    }

    void incrementKeepAlives()
    {
        if (WarpClient.currentState == WarpConnectionState.RECOVERING)
        {
            UILogView.Log("recovering++" + countPendingRecovery);
            countPendingRecovery = countPendingRecovery + 1;
            if (countPendingRecovery > 15)
            {
                countPendingRecovery = 0;
                sendTimeOut();
            }
        }
        else
        {
            //Debug.Log("countPendingKeepAlives: " + countPendingKeepAlives);
            countPendingKeepAlives = countPendingKeepAlives + 1;
            if (countPendingKeepAlives > 2)
            {
                UILogView.Log("reach limit keepalive");
                countPendingKeepAlives = 0;
                countPendingRecovery = 0;
                UILogView.Log("set status to RECOVERING");
                socket_close();
                isConnected = false;
                WarpClient.currentState = WarpConnectionState.RECOVERING;
            }
        }
    }

    void sendTimeOut()
    {
        Debug.Log("**************** SEND TIME OUT, RESET THE GAME ************");
        WarpClient.currentState = WarpConnectionState.DISCONNECTED;
        countPendingKeepAlives = 0;
        countPendingRecovery = 0;
        timeToRecovery = Time.time;
        isRecovery = true;
        isConnected = false;
        getKickedOut = true;

        onLostConnection(2);

        try
        {
            socket_close();
        }
        catch (Exception ex)
        {
            UILogView.Log(this.GetType().Name + " -- sendTimeOut: " + ex.Message);
        }
    }

	public void SwichServerToTestCheck(Toggle toggle)
	{
		if (toggle != null)
		{
			int swichServerToTest = PlayerPrefs.GetInt("swichServerToTest", 0);
			toggle.isOn = (swichServerToTest == 1 ? true : false);
			SwichServerToTest (toggle);
		}
	}

    public void SwichServerToTest(Toggle toggle)
    {
        if (toggle.isOn)
		{
#if UNITY_ANDROID || UNITY_IOS
			//warp_host = "52.74.93.182";
			warp_host = "192.168.1.95";
			//warp_host = "192.168.1.34";
			//warp_host = "192.168.1.95";
			httpRequest = "http://"+ warp_host + ":8686/api/";
#elif UNITY_WEBGL
            warp_host =  "ws://192.168.1.95:8889/websocket";
#endif
            warp_port = 8888;
        }
        else
		{
			warp_host = prod_server;
			httpRequest = httpRequestServer;
			warp_port = prod_port;
        }

        if (Channel != null && Channel.isConnected)
            Channel.socket_close();

        string content = "Swich to connect to: " + warp_host + ":" + warp_port;
        //OGUIM.Toast.Show(content, UIToast.ToastType.Notification, 1);

		PlayerPrefs.SetInt("swichServerToTest", toggle.isOn ? 1 : 0);
		PlayerPrefs.Save();
    }
}

