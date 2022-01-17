using HarmonyLib;

namespace LabratEyeTracking
{
    public class HarmonyHelper
    {
        public static class initcache
        {
            public static bool hasinit;
        }
        
        class discordPatch
        {
            [HarmonyPatch(typeof(discordManager), "updateDiscord")]
            [HarmonyPostfix]
            static void updateDiscord()
            {
                if (!initcache.hasinit)
                {
                    MainMod.currentEyeTrackingRuntime?.Init();
                    initcache.hasinit = true;
                }
            }
        }

        public static void Patch()
        {
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(discordPatch));
            LogHelper.Debug("Patched Harmony!");
        }
    }
}