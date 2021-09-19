using HypeRate.NET;

namespace HRtoVRChat.HRManagers
{
    public class HypeRateManager : HRManager
    {
        public HeartRate hypeRate;

        public bool Init(string sessionId)
        {
            if (hypeRate == null)
            {
                hypeRate = new HeartRate(sessionId);
                LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
                Subscribe();
                return true;
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
            return false;
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

        public int GetHR()
        {
            int iTR = 0;
            if (hypeRate != null)
            {
                iTR = hypeRate.HR;
            }

            return iTR;
        }

        public void Stop()
        {
            if (hypeRate != null)
            {
                hypeRate.Unsubscribe();
                LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                hypeRate = null;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }

        public bool IsOpen() => hypeRate.isSubscribed;
    }
}
