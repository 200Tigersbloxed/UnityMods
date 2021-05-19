using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LabratEyeTracking
{
    public static class GameHelper
    {
        public static GameObject FindPlayerModel()
        {
            GameObject goToReturn = null;
            try
            {
                goToReturn = GameObject.Find("PlayerV2");
            }
            catch (Exception) { }

            return goToReturn;
        }

        public static GameObject FindBlinkContainer(GameObject playerObject)
        {
            GameObject containerToReturn = null;
            if (playerObject.name == "PlayerV2")
            {
                // get children via. Transform  because it's easier on the CPU
                containerToReturn = playerObject.transform.Find("SteamVRObjects")
                    .Find("VRCamera")
                    .Find("Blink").gameObject;
            }

            return containerToReturn;
        }

        public static Activateblink GetBlinkComponent(GameObject go) { return go.GetComponent<Activateblink>(); }
        public static void SetupBlinkComponent(Activateblink blinkcomponent)
        {
            blinkcomponent.blinkingEnabled = false; blinkcomponent.fadeIn = 0;
            blinkcomponent.fadeOut = 0;
        }
        public static bool IsGameScene(Scene scene)
        {
            bool valToReturn = false;
            if (scene.name == "Singleplayer") { valToReturn = true; }

            return valToReturn;
        }

        // soon
        /*
        public static Text CachedTextHand = null;
        public static Text CachedTextScreen = null;

        public static void SetupUI()
        {
            // Assumes you're in the game scene and loaded
            GameObject playermodel = FindPlayerModel();
            // See if it already exists
            if (CachedTextHand == null)
            {
                // it doesn't exist
                // make the hand label
                GameObject BlinkCanvas = playermodel.transform.Find("SteamVRObjects")
                    .Find("LeftHand")
                    .Find("Canvas")
                    .Find("Blink").gameObject;
                GameObject BlinkIcon = BlinkCanvas.transform.Find("BlinkIcon").gameObject;
                GameObject TextObject = new GameObject();
                TextObject.transform.position = BlinkIcon.transform.position;
                TextObject.transform.parent = BlinkCanvas.transform;
                TextObject.name = "SRanipalLabel";
                Text TextLabel = TextObject.AddComponent<Text>();
                TextLabel.text = "Eye Openess: wait";
                RectTransform Trt = TextObject.GetComponent<RectTransform>();
                RectTransform Brt = BlinkIcon.GetComponent<RectTransform>();
                Trt.anchoredPosition = new Vector2(Brt.anchoredPosition.x, Brt.anchoredPosition.y + 50);
                Trt.anchoredPosition3D = new Vector3(Brt.anchoredPosition3D.x, Brt.anchoredPosition3D.y, Brt.anchoredPosition3D.z);
                CachedTextHand = TextLabel;
            }
            if(CachedTextScreen == null)
            {
                // make the screen label
                GameObject BlinkCanvas = playermodel.transform.Find("ScreenOverlay")
                    .Find("Blink").gameObject;
                GameObject BlinkIcon = BlinkCanvas.transform.Find("BlinkIcon").gameObject;
                GameObject TextObject = new GameObject();
                TextObject.transform.position = BlinkIcon.transform.position;
                TextObject.transform.parent = BlinkCanvas.transform;
                Text TextLabel = TextObject.AddComponent<Text>();
                TextObject.name = "SRanipalLabel";
                TextLabel.text = "Eye Openess: wait";
                RectTransform Trt = TextObject.GetComponent<RectTransform>();
                RectTransform Brt = BlinkIcon.GetComponent<RectTransform>();
                Trt.anchoredPosition = new Vector2(Brt.anchoredPosition.x, Brt.anchoredPosition.y + 50);
                Trt.anchoredPosition3D = new Vector3(Brt.anchoredPosition3D.x, Brt.anchoredPosition3D.y, Brt.anchoredPosition3D.z);
                CachedTextScreen = TextLabel;
            }
        }

        public static void SetUILabelText(string EyeOpeness)
        {
            if(CachedTextHand != null && CachedTextScreen != null)
            {
                CachedTextHand.text = "Eye Openess: " + EyeOpeness;
                CachedTextScreen.text = "Eye Openess: " + EyeOpeness;
            }
        }

        public static void ResetUILabels()
        {
            if (CachedTextHand != null && CachedTextScreen != null)
            {
                UnityEngine.Object.Destroy(CachedTextHand.gameObject);
                UnityEngine.Object.Destroy(CachedTextScreen.gameObject);
                CachedTextHand = null;
                CachedTextScreen = null;
            }
        }
        */
    }
}
