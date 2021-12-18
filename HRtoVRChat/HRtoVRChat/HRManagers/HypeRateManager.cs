// Replaced with Reflection
//using HypeRate.NET;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
    public class HypeRateManager : HRManager
    {
        private ClientWebSocket cws = null;
        private Thread _thread;
        private bool shouldOpen = false;

        private bool IsConnected => cws?.State == WebSocketState.Open;
        public int HR { get; private set; } = 0;

        public bool Init(string id)
        {
            shouldOpen = true;
            StartThread(id);
            LogHelper.Log("HypeRateManager", "Initialized WebSocket!");
            return IsConnected;
        }

        private bool didSendJsonMessage = false;

        int senderror = 0;

        private async Task SendMessage(string message)
        {
            if (cws != null)
            {
                if (cws.State == WebSocketState.Open)
                {
                    byte[] sendBody = Encoding.UTF8.GetBytes(message);
                    try
                    {
                        await cws.SendAsync(new ArraySegment<byte>(sendBody), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception e) 
                    {
                        LogHelper.Error("HypeRateManager", "Failed to SendMessage to HypeRate server! Exception:" + e);
                        senderror++;
                        if (senderror > 15)
                        {
                            await Close();
                            LogHelper.Warn("HypeRateManager", "Too many errors while trying to send a message. Closed Socket.");
                        }
                    }
                }
            }
        }

        int receiveerror = 0;
        private async Task<string> ReceiveMessage()
        {
            var clientbuffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = null;
            try
            {
                result = await cws.ReceiveAsync(clientbuffer, CancellationToken.None);
            }
            catch(Exception e)
            {
                LogHelper.Error("HypeRateManager", "Failed to Receive Message from HypeRate server! Exception: " + e);
                receiveerror++;
                if (receiveerror > 15)
                {
                    await Close();
                    LogHelper.Warn("HypeRateManager", "Too many errors while trying to receive a message. Closed Socket.");
                }
            }
            // Only check if result is not null
            if(result != null)
                if (result.Count != 0 || result.CloseStatus == WebSocketCloseStatus.Empty)
                {
                    string msg = Encoding.ASCII.GetString(clientbuffer.Array);
                    return msg;
                }
            return String.Empty;
        }

        private void HandleMessage(string message)
        {
            try
            {
                JObject json = JObject.Parse(message);
                if (json["event"] != null && json["event"].ToObject<string>() == "hr_update")
                {
                    if (json["payload"] != null && json["payload"]["hr"] != null)
                    {
                        HR = json["payload"]["hr"].ToObject<int>();
                    }
                    else
                    {
                        throw new Exception("json payload/hr is null!");
                    }
                }
                else
                {
                    throw new Exception("json event is null!");
                }
            }
            catch (Exception) { }
        }

        private string GenerateSessionJson(string sessionId)
        {
            // Assumes HeartRate has been created
            return "{\"topic\": \"hr:" + sessionId + "\",\"event\": \"phx_join\",\"payload\": {},\"ref\": 0}";
        }


        public void StartThread(string id)
        {
            _thread = new Thread(async () =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                cws = new ClientWebSocket();
                bool noerror = true;
                try
                {
                    await cws.ConnectAsync(new Uri("wss://app.hyperate.io/socket/websocket"), CancellationToken.None);
                }
                catch(Exception e)
                {
                    LogHelper.Error("HypeRateManager", "Failed to connect to HypeRate server! Exception: " + e);
                    noerror = false;
                }
                if (noerror)
                {
                    int i = 0;
                    while (shouldOpen && IsConnected)
                    {
                        if(IsConnected && !didSendJsonMessage)
                        {
                            await SendMessage(GenerateSessionJson(id));
                            didSendJsonMessage = true;
                        }
                        if (i >= 1500)
                        {
                            await SendMessage("{\"topic\": \"phoenix\",\"event\": \"heartbeat\",\"payload\": {},\"ref\": 123456}");
                            i = 0;
                        }
                        else
                            i++;
                        string message = await ReceiveMessage();
                        if (!string.IsNullOrEmpty(message))
                            HandleMessage(message);
                        Thread.Sleep(10);
                    }
                }
                await Close();
                LogHelper.Log("HypeRateManager", "Closed HypeRate");
                _thread.Abort();
            });
            _thread.Start();
        }

        public int GetHR() => HR;

        private async Task Close()
        {
            if (cws != null)
                if (cws.State == WebSocketState.Open)
                    try
                    {
                        await cws.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
                        didSendJsonMessage = false;
                        cws.Dispose();
                        cws = null;
                    }
                    catch(Exception e)
                    {
                        LogHelper.Error("HypeRateManager", "Failed to close connection to HypeRate Server! Exception: " + e);
                    }
                else
                    LogHelper.Warn("HypeRateManager", "WebSocket is not alive! Did you mean to Dispose()?");
            else
                LogHelper.Warn("HypeRateManager", "WebSocket is null! Did you mean to Initialize()?");
        }

        public void Stop()
        {
            if (cws != null)
            {
                shouldOpen = false;
                LogHelper.Debug("HypeRateManager", "Sent message to Stop WebSocket");
            }
            else
                LogHelper.Warn("HypeRateManager", "WebSocket is already null! Did you mean to Initialize()?");
        }

        public bool IsOpen() => IsConnected;

        public bool IsActive() => IsOpen();
    }

    /*
    public class HypeRateManager : HRManager
    {
        private object hypeRate = null;
        private Assembly hypeRate_assembly = null;

        public void hyperate_create(string id)
        {
            if (hypeRate_assembly == null)
                hypeRate_assembly = DependencyManager.GetAssemblyByName("HypeRate.NET.dll");
            Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
            if (hypeRate_type != null)
            {
                object[] parameters = new object[hypeRate_type.GetConstructors()[0].GetParameters().Length];
                parameters[0] = id;
                object hypeRate_instance = Activator.CreateInstance(hypeRate_type, parameters);
                if (hypeRate_instance != null)
                {
                    hypeRate = hypeRate_instance;
                    LogHelper.Debug("HypeRateManager", "Created hypeRate Instance!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to create HeartRate Instance!");
            }
            else
                LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
        }

        public void hyperate_subscribe()
        {
            if(hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    MethodInfo subMethod = hypeRate_type.GetMethod("Subscribe");
                    if(subMethod != null)
                    {
                        object[] subParameters = new object[subMethod.GetParameters().Length];
                        subMethod.Invoke(hypeRate, subParameters);
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find Subscribe method!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
        }

        public void hyperate_unsubscribe()
        {
            if (hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    MethodInfo unsubMethod = hypeRate_type.GetMethod("Unsubscribe");
                    if (unsubMethod != null)
                    {
                        object[] unsubParameters = new object[unsubMethod.GetParameters().Length];
                        unsubMethod.Invoke(hypeRate, unsubParameters);
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find Unsubscribe method!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
        }

        public int hyperate_gethr()
        {
            int hr = 0;
            if(hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    PropertyInfo hrp = hypeRate_type.GetProperty("HR");
                    if (hrp != null)
                    {
                        object value = null;
                        try
                        {
                            hrp.GetValue(hypeRate);
                            hr = (int)value;
                        }
                        catch (Exception e) { LogHelper.Error("HypeRateManager", "Failed to get or convert HR value! Exception: " + e); }
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find HR Property!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
            return hr;
        }

        public bool hyperate_getissubscribed()
        {
            bool issubscribed = false;
            if (hypeRate != null)
            {
                Type hypeRate_type = hypeRate_assembly.GetType("HypeRate.NET.HeartRate");
                if (hypeRate_type != null)
                {
                    PropertyInfo issub = hypeRate_type.GetProperty("isSubscribed");
                    if (issub != null)
                    {
                        object value = null;
                        try
                        {
                            issub.GetValue(hypeRate);
                            issubscribed = (bool)value;
                        }
                        catch (Exception e) { LogHelper.Error("HypeRateManager", "Failed to get or convert isSubscribed value! Exception: " + e); }
                    }
                    else
                        LogHelper.Error("HypeRateManager", "Failed to find isSubscribed Property!");
                }
                else
                    LogHelper.Error("HypeRateManager", "Failed to find HeartRate Type!");
            }
            return issubscribed;
        }

        private Thread _thread = null;
        private int forwardedHR = 0;

        public bool Init(string sessionId)
        {
            StartThread(sessionId);
            return IsOpen();
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread(string sessionId)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (hypeRate == null)
                {
                    hyperate_create(sessionId);
                    LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
                    Subscribe();
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
                while (hyperate_getissubscribed())
                {
                    forwardedHR = hyperate_gethr();
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void Subscribe()
        {
            if (hypeRate != null)
            {
                hyperate_subscribe();
                LogHelper.Log("HypeRateManager", "Subscribed to HypeRate Data!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public int GetHR() => forwardedHR;

        public void Stop()
        {
            if (hypeRate != null)
            {
                hyperate_unsubscribe();
                LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                hypeRate = null;
                VerifyClosedThread();
                forwardedHR = 0;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }

        public bool IsOpen()
        {
            if (hypeRate != null)
                return hyperate_getissubscribed();
            else
                return false;
        }

        public bool IsActive() => IsOpen();
    }
    */

    /*
    public class HypeRateManager : HRManager
    {
        public HeartRate hypeRate;
        private Thread _thread = null;
        private int forwardedHR = 0;

        public bool Init(string sessionId)
        {
            StartThread(sessionId);
            return IsOpen();
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread(string sessionId)
        {
            VerifyClosedThread();
            _thread = new Thread(() =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                if (hypeRate == null)
                {
                    hypeRate = new HeartRate(sessionId);
                    LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
                    Subscribe();
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
                while (hypeRate.isSubscribed)
                {
                    forwardedHR = hypeRate.HR;
                    Thread.Sleep(10);
                }
            });
            _thread.Start();
        }

        private void Subscribe()
        {
            if (hypeRate != null)
            {
                hypeRate.Subscribe();
                LogHelper.Log("HypeRateManager", "Subscribed to HypeRate Data!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public int GetHR() => forwardedHR;

        public void Stop()
        {
            if (hypeRate != null)
            {
                hypeRate.Unsubscribe();
                LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                hypeRate = null;
                VerifyClosedThread();
                forwardedHR = 0;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }

        public bool IsOpen()
        {
            if (hypeRate != null)
                return hypeRate.isSubscribed;
            else
                return false;
        }

        public bool IsActive() => IsOpen();
    }
    */
}
