using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC.SDKBase;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HRtoVRChat
{
    public class MainMod : MelonMod
    {
        private static HRType hrType = HRType.Unknown;
        private static HRManager activeHRManager = null;
        private bool UpdateIENum = true;
        private bool isRestarting = false;

        public static Action<int, int, int> OnHRValuesUpdated = (ones, tens, hundreds) => { };

        private class currentHRSplit
        {
            public int ones = 0;
            public int tens = 0;
            public int hundreds = 0;
        }

        private void LogSupportedMod(string modName, int result)
        {
            if (result == 0)
                LogHelper.Log("MainMod", $"{modName} loaded!");
            else
                LogHelper.Error("MainMod", $"Failed to load mod {modName}!");
        }

        public override void OnApplicationLateStart()
        {
            LogHelper.Log("MainMod", "Starting HRtoVRChat!");
            // Get Config
            ConfigHelper.Config lc = ConfigHelper.LoadConfig();
            // AssetBundles
            AssetManager.Init();
            // Mod Support
            if (ConfigHelper.LoadedConfig.UIXSupport)
            {
                int uixSupported = ModSupport.UIX.Init(new Action(delegate () { RestartHRListener(); }));
                LogSupportedMod("UI Expansion Kit", uixSupported);
            }
            if (ConfigHelper.LoadedConfig.AMAPISupport)
            {
                List<ModSupport.AMAPI.NewButton> nbList = new List<ModSupport.AMAPI.NewButton>
                {
                    new ModSupport.AMAPI.NewButton
                    {
                        buttonText = "RestartHR",
                        buttonAction = new Action(delegate(){ RestartHRListener(); }),
                        texture = AssetManager.loadedAssets.heartbeat
                    },
                    new ModSupport.AMAPI.NewButton
                    {
                        buttonText = "StartHR",
                        buttonAction = new Action(delegate(){ StartHRListener(); }),
                        texture = AssetManager.loadedAssets.go
                    },
                    new ModSupport.AMAPI.NewButton
                    {
                        buttonText = "StopHR",
                        buttonAction = new Action(delegate(){ StopHRListener(); }),
                        texture = AssetManager.loadedAssets.stop
                    }
                };
                int amapiSupported = ModSupport.AMAPI.Init(nbList, AssetManager.loadedAssets.heartbeat);
                LogSupportedMod("ActionMenuApi", amapiSupported);
            }
            VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += NetworkEvents_OnAvatarInstantiated;
            // Start everything else
            Start();
            // based, red-pilled
            base.OnApplicationLateStart();
        }

        private void NetworkEvents_OnAvatarInstantiated(VRCAvatarManager arg1, VRC.Core.ApiAvatar arg2, GameObject arg3)
        {
            VRCPlayerApi papi = arg1.field_Private_VRCPlayer_0.prop_VRCPlayerApi_0;
            LogHelper.Debug("MainMod", "Avatar Instantiated : " + papi.isLocal);
            if(papi.isLocal)
                foreach(ParamsManager.IntParameter param in ParamsManager.Parameters)
                    param.ResetParam();
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
            // Get Config
            ConfigHelper.Config lc = ConfigHelper.LoadConfig();
            // Restart and read new config
            LogHelper.Log("MainMod", "Restarting HRtoVRChat!");
            RestartHRListener();
            // based, red-pilled
            base.OnPreferencesSaved();
        }

        private void Start()
        {
            // Start Manager based on Config
            hrType = StringToHRType(ConfigHelper.LoadedConfig.hrType);
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

        private void RestartHRListener()
        {
            int loops = 0;
            if (!isRestarting)
            {
                isRestarting = true;
                // Called for when you need to Reset the HRListener
                StopHRListener();
                Task.Factory.StartNew(() =>
                {
                    while(loops <= 2)
                    {
                        Task.Delay(1000);
                        loops++;
                    }
                    isRestarting = false;
                    StartHRListener();
                });
                /*
                IEnumerator waitNumerator()
                {
                    yield return new WaitForSeconds(2);
                    isRestarting = false;
                    StartHRListener();
                }
                MelonCoroutines.Start(waitNumerator());
                */
            }
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
            if(activeHRManager != null)
                if (activeHRManager.IsOpen())
                {
                    LogHelper.Warn("MainMod", "HRListener is currently active! Stop it first");
                    return;
                }
            switch (hrType)
            {
                case HRType.FitbitHRtoWS:
                    activeHRManager = new HRManagers.FitbitManager();
                    activeHRManager.Init(ConfigHelper.LoadedConfig.fitbitURL);
                    break;
                case HRType.HypeRate:
                    activeHRManager = new HRManagers.HypeRateManager();
                    activeHRManager.Init(ConfigHelper.LoadedConfig.hyperateSessionId);
                    break;
                default:
                    LogHelper.Warn("MainMod", "No hrType was selected! Please see README if you think this is an error!");
                    break;
            }
        }

        private static void StopHRListener()
        {
            if(activeHRManager != null)
            {
                if (!activeHRManager.IsOpen())
                {
                    LogHelper.Warn("MainMod", "HRListener is currently inactive! Start it first!");
                    return;
                }
                activeHRManager.Stop();
            }
        }

        IEnumerator BoopUwU()
        {
            // Get HR
            int HR = activeHRManager.GetHR();
            // Cast to currentHRSplit
            currentHRSplit chs = intToHRSplit(HR);
            OnHRValuesUpdated.Invoke(chs.ones, chs.tens, chs.hundreds);
            yield return new WaitForSeconds(1);
            if (UpdateIENum) MelonCoroutines.Start(BoopUwU());
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
