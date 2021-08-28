using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;

namespace HRtoVRChat
{
    public class MainMod : MelonMod
    {
        private static HRType hrType = HRType.Unknown;
        private bool UpdateIENum = true;

        public static Action<int, int, int> OnHRValuesUpdated = (ones, tens, hundreds) => { };

        private class currentHRSplit
        {
            public int ones = 0;
            public int tens = 0;
            public int hundreds = 0;
        }

        public override void OnApplicationLateStart()
        {
            LogHelper.Log("MainMod", "Starting HRtoVRChat!");
            Start();
            // based, red-pilled
            base.OnApplicationLateStart();
        }

        public override void OnApplicationQuit()
        {
            LogHelper.Log("MainMod", "Stopping HRtoVRChat!");
            Stop();
            // based, red-pilled
            base.OnApplicationQuit();
        }

        public override void OnPreferencesSaved()
        {
            // Restart and read new config
            LogHelper.Log("MainMod", "Restarting HRtoVRChat!");
            Stop();
            Start();
            // based, red-pilled
            base.OnPreferencesSaved();
        }

        private void Start()
        {
            // Get Config
            ConfigHelper.Config lc = ConfigHelper.LoadConfig();
            // Start Manager based on Config
            hrType = StringToHRType(lc.hrType);
            StartHRListener();
            // Start Coroutine
            MelonCoroutines.Start(BoopUwU());
            // Create IntParameters
            ParamsManager.Parameters.Add(new ParamsManager.IntParameter(hro => hro.ones, "onesHR"));
            ParamsManager.Parameters.Add(new ParamsManager.IntParameter(hro => hro.tens, "tensHR"));
            ParamsManager.Parameters.Add(new ParamsManager.IntParameter(hro => hro.hundreds, "hundredsHR"));
        }

        private void Stop()
        {
            // Stop MelonCoroutine
            UpdateIENum = false;
            MelonCoroutines.Stop(BoopUwU());
            // Stop HR Listener
            StopHRListener();
            // Clear IntParameters
            ParamsManager.Parameters.Clear();
        }

        private static HRType StringToHRType(string input)
        {
            HRType hrt = HRType.Unknown;
            switch (input.ToLower())
            {
                case "fitbithrtows":
                    hrt = HRType.FitbitHRtoWS;
                    break;
                case "hyperate":
                    hrt = HRType.HypeRate;
                    break;
            }

            return hrt;
        }

        private static void StartHRListener()
        {
            switch (hrType)
            {
                case HRType.FitbitHRtoWS:
                    HRManagers.FitbitManager.Initialize(ConfigHelper.LoadedConfig.fitbitURL);
                    break;
                case HRType.HypeRate:
                    HRManagers.HypeRateManager.Initialize(ConfigHelper.LoadedConfig.hyperateSessionId);
                    HRManagers.HypeRateManager.Subscribe();
                    break;
                default:
                    LogHelper.Warn("MainMod", "No hrType was selected! Please see README if you think this is an error!");
                    break;
            }
        }

        private static void StopHRListener()
        {
            switch (hrType)
            {
                case HRType.FitbitHRtoWS:
                    HRManagers.FitbitManager.Close();
                    HRManagers.FitbitManager.Dispose();
                    break;
                case HRType.HypeRate:
                    HRManagers.HypeRateManager.Unsubscribe();
                    HRManagers.HypeRateManager.Dispose();
                    break;
            }
        }

        IEnumerator BoopUwU()
        {
            // Get HR
            int HR = GetHR();
            // Cast to currentHRSplit
            currentHRSplit chs = intToHRSplit(HR);
            OnHRValuesUpdated.Invoke(chs.ones, chs.tens, chs.hundreds);
            yield return new WaitForSeconds(1);
            if (UpdateIENum) MelonCoroutines.Start(BoopUwU());
        }

        private int GetHR()
        {
            int hrToReturn = 0;
            switch (hrType)
            {
                case HRType.FitbitHRtoWS:
                    HRManagers.FitbitManager.getHRMessage();
                    HRManagers.FitbitManager.getFitbitConnectionMessage();
                    hrToReturn = HRManagers.FitbitManager.HR;
                    break;
                case HRType.HypeRate:
                    hrToReturn = HRManagers.HypeRateManager.GetHR();
                    break;
            }

            return hrToReturn;
        }

        private currentHRSplit intToHRSplit(int hr)
        {
            currentHRSplit chs = new currentHRSplit();
            if (hr < 0)
                LogHelper.Error("MainMod", "Why is your HeartRate below zero???? Dude, please stop... I care bro.");
            else
            {
                var currentNumber = hr.ToString().Select(x => int.Parse(x.ToString()));
                int[] numbers = currentNumber.ToArray();
                if(hr <= 9)
                {
                    // why is your HR less than 10????
                    try
                    {
                        chs.ones = numbers[0];
                        chs.tens = 0;
                        chs.hundreds = 0;
                    }
                    catch (Exception) { }
                }
                else if(hr <= 99)
                {
                    try
                    {
                        chs.ones = numbers[1];
                        chs.tens = numbers[0];
                        chs.hundreds = 0;
                    }
                    catch (Exception) { }
                }
                else if(hr >= 100)
                {
                    try
                    {
                        chs.ones = numbers[2];
                        chs.tens = numbers[1];
                        chs.hundreds = numbers[0];
                    }
                    catch (Exception) { }
                }
                else
                {
                    // if your heart rate is above 999 then you need to see a doctor
                    // for real what
                }
            }

            return chs;
        }

        private enum HRType
        {
            FitbitHRtoWS,
            HypeRate,
            Unknown
        }
    }
}
