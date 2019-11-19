using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Threading;

#if NETFX_CORE || BUILD_FOR_WP8
    using System.Threading.Tasks;
    using Windows.Networking.Sockets;

    using TcpClient = BestHTTP.PlatformSupport.TcpClient.WinRT.TcpClient;

    //Disable CD4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable 4014
#elif UNITY_WP8 && !UNITY_EDITOR
    using TcpClient = BestHTTP.PlatformSupport.TcpClient.WP8.TcpClient;
#else
//using TcpClient = BestHTTP.PlatformSupport.TcpClient.General.TcpClient;
using TcpClient = System.Net.Sockets.TcpClient;
#endif
public class TCPSocket
{
    private string tcpAddress;
    private int tcpPort;
    public bool isConnected;
	public bool bufferRemain;
    
    TcpClient socket;
    Stream theStream;


#if NETFX_CORE
#else
    Thread m_thread;
#endif
    private static object receiveLockObj = new object();

    BinaryWriter theWriter;
    BinaryReader theReader;

    string m_Error = null;
    private Queue<byte[]> m_receives;
    

    public TCPSocket(string address, int port)
    {
        tcpAddress = address;
        tcpPort = port;
        m_receives = new Queue<byte[]>();
        socket = new TcpClient();
    }
    public IEnumerator Connect(Action actionOnConnected = null)
    {
        setupSocket();
        while (theStream == null || !theStream.CanRead)
        {
            yield return 0;
        }
        isConnected = true;
        if (actionOnConnected != null)
            actionOnConnected();
    }
    private void setupSocket()
    {
        m_Error = "";
        try
        {
            socket.Connect(tcpAddress, tcpPort);
            theStream = socket.GetStream();
            theWriter = new BinaryWriter(theStream);
            theReader = new BinaryReader(theStream);

#if NETFX_CORE
        Windows.System.Threading.ThreadPool.RunAsync(Receive);
#else
            socket.SendTimeout = 3000;
            socket.SendBufferSize = 1024;
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
        catch (Exception e)
        {
            Close();
            m_Error = "TCPSocket/ setupSocket error:" + e.Message;
            Debug.LogError("Socket error:" + e);
        }
    }

    public
#if NETFX_CORE
            async
#endif
        void Receive(System.Object obj)
    {
        while (socket != null)
        {
            byte[] buffer = new byte[8192];
            var byteRead = theReader.Read(buffer, 0, buffer.Length);
            if (byteRead > 0)
            {
                buffer = buffer.Take(byteRead).ToArray();
                m_receives.Enqueue(buffer);
            }
        }
        Debug.LogError("recv is ended....");

#if !NETFX_CORE
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
            if (isConnected)
            {
                theWriter.Write(buffer);
                theWriter.Flush();
            }
            else
            {
                Debug.LogError("TCPSocket/ Send: socket is not connected");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Socket error:" + e);
            Close();
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
					if (m_receives.Count > 1) 
						bufferRemain = true;
					else
						bufferRemain = false;
                    return m_receives.Dequeue();
                }
				else
				{
					bufferRemain = false;
                	return null;
				}
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
            isConnected = false;
            m_receives.Clear();

            if (socket != null)
            {
                socket.Close();
                socket = null;
            }

#if !NETFX_CORE
            if (m_thread != null)
            {
                m_thread.Abort();
                m_thread = null;
            }

            if (theWriter != null)
                theWriter.Close();
            if (theReader != null)
                theReader.Close();
#else

            if (theWriter != null)
                theWriter.Dispose();
            if (theReader != null)
                theReader.Dispose();
#endif
        }
        catch (Exception e)
        {
            Debug.LogError("TCPSocket Close error:" + e);
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

public static class ByteExtension
{
    
    public static byte[] TrimEnd(this byte[] array)
    {
        int lastIndex = Array.FindLastIndex(array, b => b != 0);

        Array.Resize(ref array, lastIndex + 1);

        return array;
    }
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
}
