/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if (UNITY_WSA || UNITY_WP_8_1) && !UNITY_EDITOR

using System;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace BestHTTP.PlatformSupport.TcpClient.WinRT
{
    public sealed class TcpClient : IDisposable
    {
#region Public Properties

        public bool Connected { get; private set; }
        public TimeSpan ConnectTimeout { get; set; }

        public bool UseHTTPSProtocol { get; set; }

#endregion

#region Private Properties

        internal StreamSocket Socket { get; set; }
        private System.IO.Stream Stream { get; set; }

#endregion

        public TcpClient()
        {
            ConnectTimeout = TimeSpan.FromSeconds(2);
        }

        public void Connect(string hostName, int port)
        {
            //How to secure socket connections with TLS/SSL:
            //http://msdn.microsoft.com/en-us/library/windows/apps/jj150597.aspx

            //Networking in Windows 8 Apps - Using StreamSocket for TCP Communication
            //http://blogs.msdn.com/b/metulev/archive/2012/10/22/networking-in-windows-8-apps-using-streamsocket-for-tcp-communication.aspx

            Socket = new StreamSocket();
            Socket.Control.KeepAlive = true;

            var host = new HostName(hostName);

            SocketProtectionLevel spl = SocketProtectionLevel.PlainSocket;
            if (UseHTTPSProtocol)
                spl = SocketProtectionLevel.
#if UNITY_WSA_8_0 || BUILD_FOR_WP8
                        Ssl;
#else
                        Tls12;
#endif

            // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/jj710176.aspx#content
            try
            {
                var result = Socket.ConnectAsync(host, UseHTTPSProtocol ? "https" : port.ToString(), spl);
                Connected = result.AsTask().Wait(ConnectTimeout);
            }
            catch(AggregateException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw ex;
            }

            if (!Connected)
                throw new TimeoutException("Connection timed out!");
        }

        public bool IsConnected()
        {
            return true;
        }

        public System.IO.Stream GetStream()
        {
            if (Stream == null)
                Stream = new DataReaderWriterStream(this);
            return Stream;
        }

        public void Close()
        {
            Dispose();
        }

#region IDisposeble

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Stream != null)
                        Stream.Dispose();
                    Stream = null;
                    Connected = false;
                }
                disposed = true;
            }
        }

        ~TcpClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#endregion
    }
}

#endif
