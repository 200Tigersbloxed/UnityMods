using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MelonLoader;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Threading;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
    class PulsoidManager : HRManager
    {
        bool shouldUpdate = false;
        string pubUrl = String.Empty;
        static readonly HttpClient client = new HttpClient();
        int HR = 0;

        private Thread _thread = null;

        bool HRManager.Init(string url)
        {
            // Tests to see if the URL is valid
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "HEAD";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            response.Close();
            bool requestValid = response.StatusCode == HttpStatusCode.OK;
            pubUrl = url;
            shouldUpdate = requestValid;
            if (requestValid)
                StartThread();
            return requestValid;
        }

        void VerifyClosedThread()
        {
            if (_thread != null)
            {
                if (_thread.IsAlive)
                    _thread.Abort();
            }
        }

        void StartThread()
        {
            VerifyClosedThread();
            _thread = new Thread(async () =>
            {
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                while (shouldUpdate)
                {
                    int parsedHR = default(int);
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(pubUrl);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Now Parse the Information
                        JObject jo = null;
                        try { jo = JObject.Parse(responseBody); } catch (Exception e) { LogHelper.Error("PulsoidManager", "Failed to parse JObject! Exception: " + e); }
                        if (jo != null)
                            try { parsedHR = jo["bpm"].Value<int>(); } catch (Exception) { }
                    }
                    catch (HttpRequestException e)
                    {
                        LogHelper.Error("PulsoidManager", "Failed to get HttpRequest! Exception: " + e);
                    }
                    HR = parsedHR;
                }
                Thread.Sleep(500);
            });
            _thread.Start();
        }

        void HRManager.Stop()
        {
            shouldUpdate = false;
            VerifyClosedThread();
        }

        int HRManager.GetHR() => HR;
        public bool IsOpen() => shouldUpdate && !string.IsNullOrEmpty(pubUrl);
        bool HRManager.IsActive() => IsOpen();
    }
}
