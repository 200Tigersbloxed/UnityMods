using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HRtoVRChat
{
    public static class EasyAvatarHook
    {
        public static List<GameObject> HookedUsers = new List<GameObject>();

        public static Action<VRCPlayer> OnPlayerJoined = (user) => { };
        public static Action<VRCPlayer> OnPlayerLeft = (user) => { };
        public static Action<GameObject, bool> OnAvatarInstantiated = (avatar, isLocal) => { };

        private static bool IsUserHooked(GameObject user)
        {
            bool bTR = false;
            foreach(GameObject possibleUser in HookedUsers)
            {
                if (possibleUser == user)
                    bTR = true;
            }

            return bTR;
        }

        public static void Update()
        {
            // Find all GameObjects that match the VRCPlayer[Remote] name
            // The only rule is no spam looping GetComponent<>(), that's bad, we want CPU light stuff
            Scene currentScene = SceneManager.GetActiveScene();
            GameObject[] gos = currentScene.GetRootGameObjects();
            foreach(GameObject go in gos)
            {
                string[] gonamesplit = go.name.Split(' ');
                if(gonamesplit[0].Contains("VRCPlayer[Local]") || gonamesplit[0].Contains("VRCPlayer[Remote]"))
                {
                    if (!IsUserHooked(go))
                    {
                        HookedUsers.Add(go);
                        HookListener hl = go.AddComponent<HookListener>();
                        hl.enabled = true;
                    }
                }
            }
        }
    }

    [RegisterTypeInIl2Cpp]
    class HookListener : MonoBehaviour
    {
        public HookListener(IntPtr ptr) : base(ptr) { }

        private GameObject _avatar;
        private AvatarHook ah;
        private VRCPlayer cached_vrcplayer;

        public void FindAvatar()
        {
            _avatar = this.transform.Find("ForwardDirection").Find("Avatar").gameObject;
            if(_avatar != null)
                if (ah == null)
                {
                    ah = _avatar.AddComponent<AvatarHook>();
                    ah.hl = this;
                    ah.enabled = true;
                }
        }

        public void EventTriggered(AvatarHook sender, AvatarEvent ae)
        {
            switch (ae)
            {
                case AvatarEvent.AvatarChanged:
                    EasyAvatarHook.OnAvatarInstantiated.Invoke(sender.gameObject, sender.isLocal);
                    break;
                case AvatarEvent.AvatarDestroyed:
                    ah = null;
                    break;
            }
        }

        void OnEnable()
        {
            LogHelper.Debug("HookListener", "Hooked to user!");
            cached_vrcplayer = this.gameObject.GetComponent<VRCPlayer>();
            EasyAvatarHook.OnPlayerJoined.Invoke(cached_vrcplayer);
            FindAvatar();
        }

        void OnDisable()
        {
            LogHelper.Debug("HookListener", "Unhooked user!");
            EasyAvatarHook.OnPlayerLeft.Invoke(cached_vrcplayer);
            if (EasyAvatarHook.HookedUsers.Contains(this.gameObject))
                EasyAvatarHook.HookedUsers.Remove(this.gameObject);
        }

        void Update()
        {
            FindAvatar();
        }
    }

    [RegisterTypeInIl2Cpp]
    class AvatarHook : MonoBehaviour
    {
        public AvatarHook(IntPtr ptr) : base(ptr) { }

        public bool isLocal = false;
        public HookListener hl;

        private bool waitForHl = true;

        void OnEnable()
        {
            if (hl == null)
                waitForHl = true;
            else
                hl.EventTriggered(this, AvatarEvent.AvatarChanged);
        }

        void OnDisabe()
        {
            hl.EventTriggered(this, AvatarEvent.AvatarDestroyed);
        }

        void Update()
        {
            if (waitForHl)
            {
                if(hl != null)
                {
                    isLocal = hl.gameObject.name.Split(' ')[0].Contains("VRCPlayer[Local]");
                    hl.EventTriggered(this, AvatarEvent.AvatarChanged);
                    waitForHl = false;
                }
            }
        }
    }

    public enum AvatarEvent
    {
        AvatarChanged,
        AvatarDestroyed
    }
}
