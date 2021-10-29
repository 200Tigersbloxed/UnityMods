using System;
using WebSocketSharp;
using System.Threading;
using UnhollowerBaseLib;
using System.Net;

namespace HRtoVRChat.HRManagers
{
    public class FitbitManager : HRManager
    {
        private WebSocket ws = null;
        public bool FitbitIsConnected { get; private set; } = false;
        public int HR { get; private set; } = 0;

        public bool isConnected { get; private set; }

        private Thread _thread = null;

        public bool Init(string url)
        {
            StartThread(url);
            if (isConnected)
                return false;
            LogHelper.Log("FitbitManager", "WebSocket Initialized!");
            return isConnected;
        }

        void VerifyClosedThread()
        {
            if(_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        class WebSocketURIInfo
        {
            public bool isValid = false;
            public bool isSecure = false;
        }

        bool validateIP(string ip)
        {
            IPAddress ipAddress = null;
            bool isValidIp = IPAddress.TryParse(ip, out ipAddress);
            return isValidIp;
        }

        WebSocketURIInfo GetSocketInfo(string url)
        {
            WebSocketURIInfo wsurii = new WebSocketURIInfo();
            wsurii.isValid = url.Contains("ws://") || url.Contains("wss://");
            try
            {
                string[] colonSplit = url.Split(':');
                wsurii.isSecure = colonSplit[0] == "wss";
            }
            catch (Exception)
            {
                wsurii.isValid = false;
            }

            return wsurii;
        }

        void StartThread(string url)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                WebSocketURIInfo wsurii = GetSocketInfo(url);
                if (wsurii.isValid)
                {
                    if (ws != null)
                    {
                        Close();
                    }
                    ws = new WebSocket(url);
                    if(wsurii.isSecure)
                        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    LogHelper.Debug("FitbitManager", "isValid: " + wsurii.isValid + ", isSecure: " + wsurii.isSecure);
                    ws.OnOpen += Ws_OnOpen;
                    ws.OnMessage += OnMessage;
                    ws.OnClose += Ws_OnClose;
                    ws.Connect();
                }
                else
                {
                    LogHelper.Error("FitbitManager", "The WebSocket URI is invalid!");
                }
                while (isConnected)
                {
                    getHRMessage();
                    getFitbitConnectionMessage();
                    Thread.Sleep(500);
                }
            });
            _thread.Start();
        }

        private void getHRMessage()
        {
            if(ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Send("getHR");
        }

        private void getFitbitConnectionMessage()
        {
            if (ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Send("checkFitbitConnection");
        }

        public int GetHR() => HR;

        private void Close()
        {
            if (ws != null)
                if (ws.ReadyState == WebSocketState.Open)
                    ws.Close();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (ws != null)
            {
                if (ws.ReadyState == WebSocketState.Open)
                {
                    Close();
                    ws = null;
                }
                else
                    ws = null;
                VerifyClosedThread();
            }
            else
                LogHelper.Warn("FitbitManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => isConnected && FitbitIsConnected;

        public bool IsActive() => isConnected;

        private void OnMessage(object sender, MessageEventArgs e)
        {
            switch (e.Data.ToLower())
            {
                case "yes":
                    FitbitIsConnected = true;
                    break;
                case "no":
                    FitbitIsConnected = false;
                    break;
                default:
                    // Assume it's the HeartRate
                    try { HR = Convert.ToInt32(e.Data); } catch (Exception) { /*I have no clue what this is then*/ }
                    break;
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e) => isConnected = true;
        private void Ws_OnClose(object sender, CloseEventArgs e) => isConnected = false;
    }
}
