using HypeRate.NET;
using System.Threading;
using UnhollowerBaseLib;

namespace HRtoVRChat.HRManagers
{
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
}
