using System;
using BepInEx;
using CustomLabratNPC.NPC;
using UnityEngine.SceneManagement;

namespace CustomLabratNPC
{
    [BepInPlugin("lol.fortnite.www.customlabratnpc", "CustomLabratNPC", "0.1.0.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            HarmonyManager.Init();
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            NPCControllerCache.DestroyAll();
            if (LabratTools.IsGameScene())
            {
                // Load in the NPCs
                // -- SCP-173
                CustomNPCDescriptor npc173 = NPCLoader.GetFirstNPCbyNPCType(NPCEnum.SCP173);
                if (npc173 != null)
                {
                    NPC173Controller npc173Controller = new NPC173Controller(npc173);
                    npc173Controller.HookedNPC = NPCLoader.InstantiateNPC(npc173);
                    NPCControllerCache.Register(npc173Controller);
                }
            }
        }

        private void Update()
        {
            NPCControllerCache.UpdateAll();
        }
    }
}