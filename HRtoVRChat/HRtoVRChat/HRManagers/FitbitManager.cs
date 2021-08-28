using System;
using WebSocketSharp;

namespace HRtoVRChat.HRManagers
{
    public static class FitbitManager
    {
        public static WebSocket ws = null;
        public static bool FitbitIsConnected = false;
        public static int HR = 0;

        public static void Initialize(string url)
        {
            if (ws == null)
            {
                ws = new WebSocket(url);
                ws.OnMessage += OnMessage;
                ws.Connect();
                LogHelper.Log("FitbitManager", "WebSocket Initialized!");
            }
            else
                LogHelper.Warn("FitbitManager", "WebSocket is not null! Did you mean to Close or Dispose?");
        }

        private static void Open()
        {
            if (ws != null)
                if (!ws.IsAlive)
                    ws.Connect();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is already alive! Did you mean to Close()?");
            LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public static void getHRMessage()
        {
            if(ws != null)
                if (ws.IsAlive)
                    ws.Send("getHR");
        }

        public static void getFitbitConnectionMessage()
        {
            if (ws != null)
                if (ws.IsAlive)
                    ws.Send("checkFitbitConnection");
        }

        public static void Close()
        {
            if (ws != null)
                if (ws.IsAlive)
                    ws.Close();
                else
                    LogHelper.Warn("FitbitManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("FitbitManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public static void Dispose(bool closeAutomatically = false)
        {
            if (ws != null)
                if (ws.IsAlive)
                    if (closeAutomatically)
                        ws.Close();
                    else
                        LogHelper.Warn("FitbitManager", "WebSocket is not closed! Override closeAutomatically to true or call Close() then run Dispose() again!");
                else
                    ws = null;
            else
                LogHelper.Warn("FitbitManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

        private static void OnMessage(object sender, MessageEventArgs e)
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
    }
}
