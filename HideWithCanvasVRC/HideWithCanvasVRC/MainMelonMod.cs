using System;
using System.Collections;
using System.IO;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace HideWithCanvasVRC
{
    public class MainMelonMod : MelonMod
    {
        private bool expectedCanvas = false;

        public override void OnApplicationStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<NotificationManager>();
            ClassInjector.RegisterTypeInIl2Cpp<FollowHead>();
            base.OnApplicationStart();
        }

        public override void OnApplicationLateStart()
        {
            ResourceHelper.Init();
            AssetManager.LoadPrefabFromPath(Path.Combine(ResourceHelper.directory, "hidewithcanvasvrc"));
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Toggle Canvas", new Action(delegate () {
                NotificationData.shouldBeVisible = !NotificationData.shouldBeVisible;
            }));
            base.OnApplicationLateStart();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (UnityEngine.XR.XRDevice.isPresent)
            {
                expectedCanvas = false;
                MelonCoroutines.Start(waitForScene());
            }
            else
                LogHelper.Log("Currently on Desktop Mode. Not applying!");
            base.OnSceneWasLoaded(buildIndex, sceneName);
        }

        IEnumerator waitForScene()
        {
            yield return new WaitForSeconds(5);
            if (!AssetManager.isSetup)
            {
                PlayerKit.InitOnSceneLoaded();
                AssetManager.SetupPrefabInScene();
                expectedCanvas = true;
                LogHelper.Log($"Setup prefab at location: {AssetManager.prefab}!");
                MelonCoroutines.Stop(waitForScene());
            }
        }

        public override void OnFixedUpdate()
        {
            //if(expectedCanvas)
                //CanvasManager.UpdatePanelSize();
            base.OnFixedUpdate();
        }
    }
}
