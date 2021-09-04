using System;
using WebSocketSharp;
using System.Collections;
using MelonLoader;
using UnityEngine;

namespace HRtoVRChat.HRManagers
{
    public class FitbitManager : HRManager
    {
        private WebSocket ws = null;
        public bool FitbitIsConnected { get; private set; } = false;
        public int HR { get; private set; } = 0;

        public bool isConnected { get; private set; }

        public bool Init(string url)
        {
            if(ws != null)
            {
                Close();
            }
            ws = new WebSocket(url);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += OnMessage;
            ws.OnClose += Ws_OnClose;
            ws.Connect();
            if (!ws.IsAlive)
                return false;
            MelonCoroutines.Start(waitSeconds());
            LogHelper.Log("FitbitManager", "WebSocket Initialized!");
            return ws.IsAlive;
        }

        IEnumerator waitSeconds()
        {
            yield return new WaitForSeconds(1);
            if (isConnected)
            {
                getHRMessage();
                getFitbitConnectionMessage();
                MelonCoroutines.Start(waitSeconds());
            }
        }

        private void getHRMessage()
        {
            if(ws != null)
                if (ws.IsAlive)
                    ws.Send("getHR");
        }

        private void getFitbitConnectionMessage()
        {
            if (ws != null)
                if (ws.IsAlive)
                    ws.Send("checkFitbitConnection");
        }

        public int GetHR() => HR;

        private void Close()
        {
            if (ws != null)
                if (ws.IsAlive)
                    ws.Close();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (ws != null)
                if (ws.IsAlive)
                {
                    Close();
                    ws = null;
                }
                else
                    ws = null;
            else
                LogHelper.Warn("FitbitManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

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
