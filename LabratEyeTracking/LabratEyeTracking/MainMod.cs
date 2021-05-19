using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LabratEyeTracking
{
    [BepInPlugin("lol.fortnite.www.labrateyetracking", "LabratEyeTracking", "1.0.0")]
    [BepInProcess("SCP Labrat.exe")]
    class MainMod : BaseUnityPlugin
    {
        private bool SubscribeEyeData = false;
        private bool IsBlinking = false;

        // Cache values so we don't waste CPU getting them
        private GameObject PlayerModel = null;
        private GameObject BlinkContainer = null;
        private Activateblink BlinkComponent = null;
        private Scene currentScene;

        void Awake()
        {
            // Get unmanaged assemblies where they need to be
            UnmanagedAssemblyManager.Initialize();
            // Subscribe to Scene Loading Events
            LogHelper.Debug("Subscribing to Scene Events...");
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogHelper.Debug("Subscribed to Scene Events!");
            // Start the SRanipal SDK
            LogHelper.Debug("Initializing SRanipal SDK...");
            SRanipalHelper.Initialize();
        }

        void OnApplicationQuit()
        {
            LogHelper.Debug("Killing the SRanipal SDK...");
            SRanipalHelper.Kill();
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
            GameHelper.SetupBlinkComponent(BlinkComponent);
            // soon
            //GameHelper.SetupUI();
            SubscribeEyeData = true;
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
                        float combinedEyeOpeness = (SRanipalHelper.EyeData.verbose_data.left.eye_openness + SRanipalHelper.EyeData.verbose_data.right.eye_openness) / 2;
                        if (combinedEyeOpeness <= 0.4f)
                        {
                            BlinkComponent.waitBetween = Mathf.Infinity;
                            if (!IsBlinking)
                            {
                                BlinkComponent.blinkingEnabled = true;
                                BlinkComponent.Forcedblink();
                                IsBlinking = true;
                            }
                        }
                        else
                        {
                            BlinkComponent.waitBetween = 0;
                            if (IsBlinking)
                            {
                                BlinkComponent.Forcedblink();
                                BlinkComponent.BlinkMeterRestart();
                                BlinkComponent.blinkingEnabled = false;
                                IsBlinking = false;
                            }
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
