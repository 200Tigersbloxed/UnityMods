using CustomLabratNPC.NPC;
using HarmonyLib;
using UnityEngine;

namespace CustomLabratNPC
{
    public static class HarmonyCache
    {
        public static bool DidLoadNPCs;
    }
    
    public class HarmonyManager
    {
        private static readonly HarmonyLib.Harmony _harmony = new HarmonyLib.Harmony("LabratNPCPatch");
        
        public static void Init()
        {
            _harmony.PatchAll();
            LogHelper.Debug("Patched Harmony Methods!");
        }
        
        [HarmonyPatch(typeof(Kill), "kill", typeof(GameObject))]
        class KillHooks
        {
            [HarmonyPrefix]
            static void kill(Kill __instance, GameObject plr)
            {
                bool found = false;
                GameObject SCP = __instance.gameObject;
                for (int i = 0; i < SCP.transform.childCount; i++)
                {
                    GameObject child = SCP.transform.GetChild(i).gameObject;
                    if (LabratTools.HasComponent<CustomNPCDescriptor>(child))
                    {
                        CustomNPCDescriptor cnpcd = child.GetComponent<CustomNPCDescriptor>();
                        if (cnpcd != null)
                        {
                            INPCController npcc = NPCControllerCache.FindAssociatedNPC(cnpcd);
                            npcc?.ProxyEventInvoke(NPCEvents.Killed);
                            LogHelper.Debug("Sent Kill Message to NPC");
                            found = true;
                        }
                    }
                    if(!found)
                        LogHelper.Debug($"{__instance.gameObject.name} does not contain a CustomNPCDescriptor!");
                }
            }
        }

        [HarmonyPatch(typeof(discordManager), "updateDiscord")]
        public class discordPatch
        {
            [HarmonyPostfix]
            static void updateDiscord()
            {
                if (!HarmonyCache.DidLoadNPCs)
                {
                    NPCLoader.LoadNPCs();
                    HarmonyCache.DidLoadNPCs = true;
                }
            }
        }

        [HarmonyPatch(typeof(enemyController173), "OnEnable")]
        public class enemyController173Patch
        {
            [HarmonyPostfix]
            static void OnEnable(enemyController173 __instance)
            {
                GameObject scp173 = __instance.gameObject;
                CustomNPCDescriptor npc173 = NPCLoader.GetFirstNPCbyNPCType(NPCEnum.SCP173);
                bool isRegistered = false;
                for (int i = 0; i < scp173.transform.childCount; i++)
                {
                    if (LabratTools.HasComponent<NPC173Controller>(scp173.transform.GetChild(i).gameObject))
                        isRegistered = true;
                }
                // Only instantiate and register if the NPC isn't null and not already registered
                if (npc173 != null && !isRegistered)
                {
                    if (!LabratTools.HasComponent<NPC173Protector>(__instance.gameObject))
                    {
                        NPC173Protector protector = scp173.AddComponent<NPC173Protector>();
                        protector.Init(NPCEnum.SCP173, npc173.gameObject.name);
                    }
                    (CustomNPCDescriptor, GameObject) newNPC = NPCLoader.InstantiateNPC(npc173);
                    NPC173Controller npc173Controller = newNPC.Item2.gameObject.AddComponent<NPC173Controller>();
                    npc173Controller.Descriptor = newNPC.Item1;
                    npc173Controller.HookedNPC = npc173Controller.gameObject;
                    npc173Controller.TargetSCP = scp173;
                    bool create = NPCControllerCache.Register(npc173Controller);
                    if (!create)
                    {
                        // 3 cent solution for checking if there's already an npc since isRegistered cant pick it up???
                        Object.Destroy(npc173Controller);
                        return;
                    }
                    LogHelper.Debug($"Attached {npc173.gameObject.name} to {scp173.gameObject.name}!");
                }
                else
                    LogHelper.Error(
                        $"Failed to init NPC! NPC: {npc173.gameObject.name} RegisterStatus: {isRegistered}");
            }
        }
    }
}