using System;
using System.Collections;
using System.Linq;
using MelonLoader;
using UnityEngine;
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

        public static Action<int, int, int, int, bool, bool> OnHRValuesUpdated = (ones, tens, hundreds, HR, isConnected, isActive) => { };
        public static Action<bool, bool> OnHeartBeatUpdate = (isHeartBeat, shouldStart) => { };
        public static bool isHeartBeat { get; private set; } = false;

        private bool isAppClosing = false;

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
                        texture = AssetManager.loadedAssets.refresh
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
            //VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += NetworkEvents_OnAvatarInstantiated;
            EasyAvatarHook.OnAvatarInstantiated += OnAvatarInstantiatedListener;
            OnHeartBeatUpdate += (hrb, restart) => { if(restart) MelonCoroutines.Start(WaitStartHeartBeat()); };
            // Start everything else
            Start();
            // based, red-pilled
            base.OnApplicationLateStart();
        }

        private void OnAvatarInstantiatedListener(GameObject avatar, bool isLocal)
        {
            LogHelper.Debug("MainMod", "Avatar Instantiated: " + isLocal);
            if(isLocal)
                foreach (ParamsManager.HRParameter param in ParamsManager.Parameters)
                    param.ResetParam();
        }

        public override void OnUpdate()
        {
            EasyAvatarHook.Update();
            base.OnUpdate();
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
            if (!isAppClosing)
            {
                // Get Config
                ConfigHelper.Config lc = ConfigHelper.LoadConfig();
                // Restart and read new config
                LogHelper.Log("MainMod", "Restarting HRtoVRChat!");
                RestartHRListener();
                // based, red-pilled
                base.OnPreferencesSaved();
            }
            else
                LogHelper.Debug("MainMod", "Application is quitting! Not loading Config.");
        }

        private void Start()
        {
            StartHRListener();
            // Start Coroutine
            MelonCoroutines.Start(BoopUwU());
            MelonCoroutines.Start(HeartBeat());
        }

        private void Stop()
        {
            isAppClosing = true;
            // Stop MelonCoroutine
            UpdateIENum = false;
            if(activeHRManager != null)
                MelonCoroutines.Stop(BoopUwU());
            // Stop HR Listener
            StopHRListener();
            // Clear IntParameters no point in doing this
            // ParamsManager.Parameters.Clear();
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
                case "pulsoid":
                    hrt = HRType.Pulsoid;
                    break;
                case "textfile":
                    hrt = HRType.TextFile;
                    break;
                /*
                case "win-blegatt":
                    hrt = HRType.WinBLEGATT;
                    break;
                */
            }

            return hrt;
        }

        private static void StartHRListener()
        {
            // Start Manager based on Config
            hrType = StringToHRType(ConfigHelper.LoadedConfig.hrType);
            // Check activeHRManager
            if (activeHRManager != null)
                if (activeHRManager.IsActive())
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
                case HRType.Pulsoid:
                    activeHRManager = new HRManagers.PulsoidManager();
                    activeHRManager.Init(ConfigHelper.LoadedConfig.pulsoidfeed);
                    break;
                case HRType.TextFile:
                    activeHRManager = new HRManagers.TextFileManager();
                    activeHRManager.Init(ConfigHelper.LoadedConfig.textfilelocation);
                    break;
                /*
                case HRType.WinBLEGATT:
                    activeHRManager = new HRManagers.WinBLEGATTManager();
                    activeHRManager.Init("");
                    break;
                */
                default:
                    LogHelper.Warn("MainMod", "No hrType was selected! Please see README if you think this is an error!");
                    break;
            }
        }

        private static void StopHRListener()
        {
            if (activeHRManager != null)
            {
                if (!activeHRManager.IsActive())
                {
                    LogHelper.Warn("MainMod", "HRListener is currently inactive! Start it first!");
                    return;
                }
                activeHRManager.Stop();
            }
            activeHRManager = null;
        }

        // why did i name the ienumerator this and why haven't i changed it
        IEnumerator BoopUwU()
        {
            currentHRSplit chs = new currentHRSplit();
            bool isOpen = false;
            bool isActive = false;
            // Get HR
            int HR = 0;
            if (activeHRManager != null)
            {
                HR = activeHRManager.GetHR();
                isOpen = activeHRManager.IsOpen();
                isActive = activeHRManager.IsActive();
                // Cast to currentHRSplit
                chs = intToHRSplit(HR);
            }
            OnHRValuesUpdated.Invoke(chs.ones, chs.tens, chs.hundreds, HR, isOpen, isActive);
            yield return new WaitForSeconds(1);
            if (UpdateIENum) MelonCoroutines.Start(BoopUwU());
        }

        static IEnumerator WaitStartHeartBeat()
        {
            yield return new WaitForSeconds(0.2f);
            MelonCoroutines.Start(HeartBeat());
        }

        static IEnumerator HeartBeat()
        {
            if(activeHRManager != null)
            {
                bool io = activeHRManager.IsOpen();
                // This should be started by the Melon Update void
                if (io)
                {
                    isHeartBeat = false;
                    // Get HR
                    float HR = activeHRManager.GetHR();
                    if (HR != 0)
                    {
                        isHeartBeat = false;
                        OnHeartBeatUpdate.Invoke(isHeartBeat, false);
                        // Calculate wait interval
                        float waitTime = default(float);
                        // When lowering the HR significantly, this will cause issues with the beat bool
                        // Dubbed the "Breathing Excersise" bug
                        // There's a 'temp' fix for it right now, but I'm not sure how it'll hold up
                        try { waitTime = (1 / ((HR - 0.2f) / 60)); } catch (Exception) { /*Just a Divide by Zero Exception*/ }
                        yield return new WaitForSeconds(waitTime);
                        isHeartBeat = true;
                        OnHeartBeatUpdate.Invoke(isHeartBeat, false);
                    }
                }
                else
                {
                    ParamsManager.HRParameter foundParam = ParamsManager.Parameters.Find(x => x.GetParamName() == "isHRBeat");
                    if (foundParam.GetParamValue() >= 1f)
                    {
                        isHeartBeat = false;
                    }
                }
            }
            OnHeartBeatUpdate.Invoke(isHeartBeat, true);
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
            Pulsoid,
            TextFile,
            WinBLEGATT,
            Unknown
        }
    }
}
