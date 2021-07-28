using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HideWithCanvasVRC
{
    public static class PlayerKit
    {
        public static List<GameObject> remotePlayers = new List<GameObject>();
        public static GameObject localPlayer = null;
        private static bool erroredOnScene = false;

        public static void InitOnSceneLoaded()
        {
            erroredOnScene = false;
            remotePlayers.Clear();
            localPlayer = null;
            // Find localPlayer
            List<GameObject> players = new List<GameObject>();
            foreach(GameObject child in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (child.gameObject.name.Split(' ')[0].Contains("VRCPlayer[Local]"))
                    localPlayer = child.gameObject;
                else if (child.gameObject.name.Split(' ')[0].Contains("VRCPlayer[Remote]"))
                    remotePlayers.Add(child.gameObject);
            }
        }

        public static Transform GetArmatureBySearch(GameObject player, string keyword)
        {
            Transform ttr = null;
            foreach(Transform child in player.transform.Find("ForwardDirection").Find("Avatar").GetComponentsInChildren<Transform>())
            {
                if (child.name.ToLower().Contains(keyword.ToLower()))
                    ttr = child;
            }

            return ttr;
        }
    }
}
