using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LabratEyeTracking
{
    [BepInPlugin("lol.fortnite.www.labrateyetracking", "LabratEyeTracking", "1.1.0")]
    [BepInProcess("SCP Labrat.exe")]
    class MainMod : BaseUnityPlugin
    {
        private bool SubscribeEyeData = false;

        // Cache values so we don't waste CPU getting them
        private GameObject PlayerModel = null;
        private GameObject BlinkContainer = null;
        private Activateblink BlinkComponent = null;
        private Scene currentScene;

        // Config Values
        private ConfigEntry<int> sdkType;

        void Awake()
        {
            // Load Config
            sdkType = Config.Bind(new ConfigDefinition("SDK", "SDKs Configuration"), 0);
            // Get unmanaged assemblies where they need to be
            UnmanagedAssemblyManager.Initialize(sdkType.Value);
            // Subscribe to Scene Loading Events
            LogHelper.Debug("Subscribing to Scene Events...");
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogHelper.Debug("Subscribed to Scene Events!");
            switch (sdkType.Value)
            {
                case 1:
                    // Start the SRanipal SDK
                    LogHelper.Debug("Initializing SRanipal SDK...");
                    SRanipalHelper.Initialize();
                    break;
                case 2:
                    // Start the Pimax Eye Tracker
                    LogHelper.Debug("Initializing Pimax Eye Tracking...");
                    PimaxHelper.Initialize();
                    break;
                default:
                    // None were selected
                    LogHelper.Warn("Config values was set to 0. Have you changed the config yet?");
                    LogHelper.Warn("You can find the config under 'BepInEx/config/lol.fortnite.www.labrateyetracking.cfg'");
                    LogHelper.Warn("Set the Value to 1 for SRanipal, or set the value to 2 for Pimax");
                    break;
            }
        }

        void OnApplicationQuit()
        {
            if (SRanipalHelper.EyeTrackingEnabled)
            {
                LogHelper.Debug("Killing the SRanipal SDK...");
                SRanipalHelper.Kill();
            }
            if (PimaxHelper.eyeTracker.Active)
            {
                LogHelper.Debug("Killing the Pimax Eye Tracker...");
                PimaxHelper.Kill();
            }
        }

        IEnumerator StartEyeCoroutine()
        {
            LogHelper.Debug("Setting up Scene Values in 2 seconds...");
            yield return new WaitForSeconds(2);
            SetupEyeValues();
        }

        void SetupEyeValues()
        {
            PlayerModel = GameHelper.FindPlayerModel();
            BlinkContainer = GameHelper.FindBlinkContainer(PlayerModel);
            BlinkComponent = GameHelper.GetBlinkComponent(BlinkContainer);
            if(sdkType.Value != 0) { GameHelper.SetupBlinkComponent(BlinkComponent); SubscribeEyeData = true; }
            // soon
            //GameHelper.SetupUI();
            LogHelper.Debug("Scene Values Setup!");
        }

        void Update()
        {
            // Start updating eye values if SubscribeEyeData is true
            if (SubscribeEyeData)
            {
                // Update the Eye Values
                SRanipalHelper.UpdateEyeData();
                // Check if the user is blinking
                if (GameHelper.IsGameScene(TryGetScene()))
                {
                    if (BlinkComponent != null)
                    {
                        // set the UI text soon
                        //GameHelper.SetUILabelText(SRanipalHelper.EyeData.verbose_data.combined.eye_data.eye_openness.ToString());
                        // Left and Right are separate, because combined wasn't working for me
                        float combinedEyeOpeness = 1;
                        switch (sdkType.Value)
                        {
                            case 1:
                                // SRanipal
                                combinedEyeOpeness = (SRanipalHelper.EyeData.verbose_data.left.eye_openness + SRanipalHelper.EyeData.verbose_data.right.eye_openness) / 2;
                                break;
                            case 2:
                                // Pimax
                                combinedEyeOpeness = (PimaxHelper.eyeTracker.LeftEye.Expression.Openness + PimaxHelper.eyeTracker.RightEye.Expression.Openness) / 2;
                                break;
                        }
                        if (combinedEyeOpeness <= 0.4f)
                        {
                            Blinking.UpdateWaitTime(BlinkComponent, Mathf.Infinity);
                            if (!Blinking.IsBlinking) { Blinking.CloseEyes(BlinkComponent); }
                        }
                        else
                        {
                            BlinkComponent.waitBetween = 0;
                            if (Blinking.IsBlinking) { Blinking.OpenEyes(BlinkComponent); }
                        }
                    }
                }
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LogHelper.Debug("Scene Loaded.");
            try
            {
                currentScene = scene;
                if (GameHelper.IsGameScene(scene))
                {
                    SubscribeEyeData = false;
                    StartCoroutine(StartEyeCoroutine());
                }
                else
                {
                    // soon
                    //GameHelper.ResetUILabels();
                    PlayerModel = null;
                    BlinkContainer = null;
                    BlinkComponent = null;
                }
            }
            catch (Exception e)
            {
                LogHelper.Critical("Type failed to load. EXCEPTION: " + e.ToString());
            }
        }

        Scene TryGetScene()
        {
            Scene str;
            if(currentScene == null)
            {
                str = SceneManager.GetActiveScene();
            }
            else
            {
                str = currentScene;
            }

            return str;
        }
    }
}
