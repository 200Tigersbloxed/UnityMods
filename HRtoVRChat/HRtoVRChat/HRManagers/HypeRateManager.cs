using HypeRate.NET;

namespace HRtoVRChat.HRManagers
{
    public static class HypeRateManager
    {
        public static HeartRate hypeRate;
        public static bool IsSubscribed = false;

        public static void Initialize(string sessionId)
        {
            if (hypeRate == null)
            {
                hypeRate = new HeartRate(sessionId);
                LogHelper.Log("HypeRateManager", "HypeRate Initialized!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate already initialized! Please Unsubscribe() then Dispose() before continuing!");
        }

        public static void Subscribe()
        {
            if (hypeRate != null)
            {
                if (!IsSubscribed)
                {
                    hypeRate.Subscribe();
                    LogHelper.Log("HypeRateManager", "Subscribed to HypeRate Data!");
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate is already Subscribed! Did you mean to Unsubscribe?");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public static int GetHR()
        {
            int iTR = 0;
            if (hypeRate != null)
            {
                if (IsSubscribed)
                {
                    iTR = hypeRate.HR;
                }
            }

            return iTR;
        }

        public static void Unsubscribe()
        {
            if (hypeRate != null)
            {
                if (IsSubscribed)
                {
                    hypeRate.Unsubscribe();
                    LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                }
                else
                    LogHelper.Warn("HypeRateManager", "hypeRate is not Subscribed! Subscribe() first!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is null! Did you Initialize()?");
        }

        public static void Dispose()
        {
            if (hypeRate != null)
            {
                if (IsSubscribed)
                {
                    hypeRate.Unsubscribe();
                    LogHelper.Log("HypeRateManager", "Unsubscribed from HypeRate Data!");
                }
                hypeRate = null;
                LogHelper.Log("HypeRateManager", "HypeRate disposed!");
            }
            else
                LogHelper.Warn("HypeRateManager", "hypeRate is already Disposed! Did you mean to Initialize()?");
        }
    }
}
