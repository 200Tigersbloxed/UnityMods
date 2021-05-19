using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LabratEyeTracking
{
    public class LogHelper
    {
        public static void Debug(object message) => UnityEngine.Debug.Log("[LabratEyeTracking] (DEBUG): " + message);
        public static void Warn(object message) => UnityEngine.Debug.LogWarning("[LabratEyeTracking] (WARN): " + message);
        public static void Error(object message) => UnityEngine.Debug.LogError("[LabratEyeTracking] (ERROR): " + message);
        public static void Critical(object message) => UnityEngine.Debug.LogError("[LabratEyeTracking] (CRITICAL): " + message);
    }
}
