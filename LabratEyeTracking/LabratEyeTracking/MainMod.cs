using System;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LabratEyeTracking
{
    class MainMod : MelonMod
    {
        // Cache values so we don't waste CPU getting them
        private GameObject PlayerModel = null;
        private GameObject BlinkContainer = null;
        private static Activateblink BlinkComponent = null;
        private Scene currentScene;

        public static Action<Eye, Eye, Eye> OnEyeDataUpdate = (leftEye, rightEye, combinedEye) => { };

        // IEyeTracking Stuff
        public static IEyeTracking currentEyeTrackingRuntime = null;

        public override void OnApplicationLateStart()
        {
            HarmonyHelper.Patch();
            // Load Config
            ConfigHelper.Config lc = ConfigHelper.LoadConfig();
            // Get unmanaged assemblies where they need to be
            UnmanagedAssemblyManager.Initialize(lc.sdkType.Value);
            // Subscribe to Scene Loading Events
            LogHelper.Debug("Subscribing to Scene Events...");
            SceneManager.sceneLoaded += OnSceneLoaded;
            LogHelper.Debug("Subscribed to Scene Events!");
            switch (lc.sdkType.Value)
            {
                case 1:
                    // Start the SRanipal SDK
                    currentEyeTrackingRuntime = new SRanipalHelper();
                    break;
                case 2:
                    // Start the Pimax Eye Tracker
                    currentEyeTrackingRuntime = new PimaxHelper();
                    break;
                default:
                    // None were selected
                    LogHelper.Warn($"Config values was set to {lc.sdkType.Value}, which is not recognized as an SDK Type! Have you changed the config yet?");
                    LogHelper.Warn("You can find the config at 'UserData/MelonPreferences.cfg' in the LabratEyeTracking section");
                    LogHelper.Warn("Set the Value to 1 for SRanipal, or set the value to 2 for Pimax");
                    break;
            }
            // Events
            OnEyeDataUpdate += (leftEye, rightEye, combinedEye) =>
            {
                if (GameHelper.IsGameScene(TryGetScene()))
                {
                    // Verify the EyeTracking is active
                    if (currentEyeTrackingRuntime.EyeTrackingEnabled)
                    {
                        // Check if the user is blinking
                        if (BlinkComponent != null)
                        {
                            // set the UI text soon
                            //GameHelper.SetUILabelText(SRanipalHelper.EyeData.verbose_data.combined.eye_data.eye_openness.ToString());
                            // Left and Right are separate, because combined wasn't working for me
                            if (combinedEye.Widen <= 0.4f)
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
                        else
                        {
                            LogHelper.Warn("Blink Component is null!");
                        }
                    }
                }
            };
        }

        public override void OnPreferencesSaved() => ConfigHelper.LoadConfig();

        public override void OnApplicationQuit()
        {
            if (currentEyeTrackingRuntime?.EyeTrackingEnabled ?? false)
            {
                LogHelper.Debug("Killing the Current Eye Tracker's SDK...");
                currentEyeTrackingRuntime.Kill();
            }
        }

        void SetupEyeValues()
        {
            PlayerModel = GameHelper.FindPlayerModel();
            BlinkContainer = GameHelper.FindBlinkContainer(PlayerModel);
            BlinkComponent = GameHelper.GetBlinkComponent(BlinkContainer);
            if(ConfigHelper.LoadedConfig.sdkType.Value != 0) { GameHelper.SetupBlinkComponent(BlinkComponent); }
            // soon
            //GameHelper.SetupUI();
            LogHelper.Debug("Scene Values Setup!");
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LogHelper.Debug("Scene Loaded.");
            try
            {
                currentScene = scene;
                if (GameHelper.IsGameScene(scene))
                {
                    /*
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        SetupEyeValues();
                        GameObject newsphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        newsphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        newsphere.AddComponent<EyeDriver>();
                    });
                    */
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
                LogHelper.Error("Type failed to load. EXCEPTION: " + e.ToString());
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
