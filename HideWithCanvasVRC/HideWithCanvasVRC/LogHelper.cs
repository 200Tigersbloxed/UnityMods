using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;

namespace HideWithCanvasVRC
{
    public static class LogHelper
    {
        public static void Log(object obj) => MelonLogger.Msg($"(LOG) {obj}");
        public static void Warn(object obj) => MelonLogger.Warning(obj);
        public static void Error(object obj) => MelonLogger.Error(obj);
    }
}
