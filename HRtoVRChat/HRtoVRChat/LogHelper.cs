using MelonLoader;

namespace HRtoVRChat
{
    public static class LogHelper
    {
        public static void Log(string handler, object obj) => MelonLogger.Msg($"[HRtoVRChat/{handler}] (LOG): {obj}");
        public static void Debug(string handler, object obj) { if (ConfigHelper.LoadedConfig.ShowDebug) MelonLogger.Msg($"[HRtoVRChat/{handler}] (DEBUG): {obj}"); }
        public static void Warn(string handler, object obj) => MelonLogger.Warning($"[HRtoVRChat/{handler}] (LOG): {obj}");
        public static void Error(string handler, object obj) => MelonLogger.Error($"[HRtoVRChat/{handler}] (LOG): {obj}");
    }
}
