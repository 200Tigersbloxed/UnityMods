using System;
using MelonLoader;

namespace LabratEyeTracking
{
    public class LogHelper
    {
        private static readonly MelonLogger.Instance _logger = new MelonLogger.Instance("LabratEyeTracking");
        
        public static void Debug(object message) => _logger.Msg(message);
        public static void Warn(object message) => _logger.Warning(message);
        public static void Error(object message) => _logger.Error(message);
    }
}
