using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR;

namespace HideWithCanvasVRC
{
    public static class NotificationData
    {
        public static bool shouldBeVisible = true;
    }

    class NotificationManager : MonoBehaviour
    {
        public NotificationManager(IntPtr ptr) : base(ptr) { }

        static class CachedObjects
        {
            // Main Containers
            public static GameObject prefabContainer;
            public static GameObject pcfContainer;
            // Canvas Containers and Button
            public static GameObject canvasContainer;
            public static GameObject panelContainer;
            // PCF Containers
            public static GameObject soundContainer;
            public static GameObject textContainer;
            public static GameObject notifierContainer;
        }

        void OnEnable()
        {
            // Main Containers
            GameObject prefabContainer = this.gameObject;
            CachedObjects.prefabContainer = prefabContainer;
            GameObject pcfContainer = prefabContainer.transform.Find("PlayerCameraFollower").gameObject;
            CachedObjects.pcfContainer = pcfContainer;
            // Canvas Stuff
            GameObject canvasContainer = prefabContainer.transform.GetChild(0).gameObject;
            CachedObjects.canvasContainer = canvasContainer;
            Destroy(canvasContainer.GetComponent<GraphicRaycaster>());
            GameObject panelContainer = canvasContainer.transform.GetChild(0).gameObject;
            CachedObjects.panelContainer = panelContainer;
            // PCF stuff
            CachedObjects.soundContainer = pcfContainer.transform.Find("NotificationSound").gameObject;
            CachedObjects.textContainer = pcfContainer.transform.Find("NotificationText").gameObject;
            CachedObjects.notifierContainer = pcfContainer.transform.Find("EnabledNotifier").gameObject;
            CachedObjects.textContainer.SetActive(false);
            CachedObjects.notifierContainer.SetActive(false);
            CachedObjects.notifierContainer.AddComponent<FollowHead>();
        }

        void Update()
        {
            CachedObjects.canvasContainer.SetActive(NotificationData.shouldBeVisible);
            CachedObjects.notifierContainer.SetActive(!NotificationData.shouldBeVisible);
            // transform parent
            this.transform.SetParent(null);
            if (Input.GetKeyDown(KeyCode.F5))
            {
                RequestSee();
            }
        }

        public void RequestSee()
        {
            CachedObjects.soundContainer.GetComponent<AudioSource>().Play();
            // Still in development
            //MelonLoader.MelonCoroutines.Start(textThingy());
        }

        IEnumerator textThingy()
        {
            GameObject newTC = Instantiate(CachedObjects.textContainer);
            newTC.SetActive(true);
            newTC.gameObject.AddComponent<FollowHead>();
            yield return new WaitForSeconds(5);
            newTC.SetActive(false);
            Destroy(newTC);
        }
    }

    public class FollowHead : MonoBehaviour
    {
        public FollowHead(IntPtr ptr) : base(ptr) { }
        private bool foundHead = false;
        private bool completedCheck = false;

        void OnEnable()
        {
            try
            {
                if (!foundHead)
                    this.transform.SetParent(PlayerKit.localPlayer.transform);
            }
            catch (Exception) { }
        }

        void FindHead()
        {
            try
            {
                GameObject localPlayerAvatar = PlayerKit.localPlayer.transform.Find("ForwardDirection").Find("Avatar").gameObject;
                if (localPlayerAvatar != null)
                {
                    Transform tryfindhead = PlayerKit.GetArmatureBySearch(PlayerKit.localPlayer, "head");
                    if (tryfindhead != null)
                        this.transform.SetParent(tryfindhead);
                    completedCheck = true;
                }
                else
                {
                    completedCheck = false;
                }
            }
            catch (Exception) { }
        }

        void Update()
        {
            if (completedCheck && foundHead)
            {
                this.transform.localPosition = Vector3.zero;
            }
            else
            {
                FindHead();
                this.transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(XRNode.Head);
                this.transform.localRotation = InputTracking.GetLocalRotation(XRNode.Head);
            }
        }
    }
}