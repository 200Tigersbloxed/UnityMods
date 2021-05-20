using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LabratEyeTracking
{
    public static class PimaxHelper
    {
        public static Pimax.EyeTracking.EyeTracker eyeTracker;

        public static void Initialize()
        {
            eyeTracker = new Pimax.EyeTracking.EyeTracker();
            //eyeTracker.OnUpdate += OnEyeTrackerUpdate;
            eyeTracker.OnStart += OnEyeTrackerStart;
            eyeTracker.OnStop += OnEyeTrackerStop;
            eyeTracker.Start();
        }

        public static void Start() { eyeTracker.Start(); }
        public static void Kill() { eyeTracker.Stop(); }

        private static void OnEyeTrackerStart()
        {
            LogHelper.Debug("Pimax Eye Tracker Started!");
        }

        private static void OnEyeTrackerStop()
        {
            LogHelper.Debug("Pimax Eye Tracker Stopped!");
        }
    }
}
