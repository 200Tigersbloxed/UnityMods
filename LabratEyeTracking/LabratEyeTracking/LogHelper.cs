using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace LabratEyeTracking
{
    public class LogHelper
    {
        private static readonly ManualLogSource _logger = Logger.CreateLogSource("LabratEyeTracking");
        
        public static void Debug(object message) => _logger.Log(LogLevel.Info, "[LabratEyeTracking] (DEBUG): " + message);
        public static void Warn(object message) => _logger.LogWarning("[LabratEyeTracking] (WARN): " + message);
        public static void Error(object message) => _logger.LogError("[LabratEyeTracking] (ERROR): " + message);
        public static void Critical(object message) => _logger.LogError("[LabratEyeTracking] (CRITICAL): " + message);
    }
}
