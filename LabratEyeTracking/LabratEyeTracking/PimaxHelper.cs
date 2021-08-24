using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

namespace LabratEyeTracking
{
    public static class PimaxHelper
    {
        public static bool EnableEyeTracking = false;
        public static Pimax.EyeTracking.EyeTracker eyeTracker;

        private static Thread PimaxWorker;

        public static void Initialize() => Start();

        public static void Start() 
        {
            PimaxWorker = new Thread(delegate ()
            {
                eyeTracker = new Pimax.EyeTracking.EyeTracker();
                //eyeTracker.OnUpdate += OnEyeTrackerUpdate;
                eyeTracker.OnStart += OnEyeTrackerStart;
                eyeTracker.OnStop += OnEyeTrackerStop;
                EnableEyeTracking = true;
                eyeTracker.Start();
                // Keep the Thread Alive
                while (EnableEyeTracking) { Thread.Sleep(10); }
            });
        }
        public static void Kill() 
        {
            if (!EnableEyeTracking)
            {
                LogHelper.Warn("Eye Tracking is not enabled, can't kill.");
                return;
            }
            // Abort Thread
            if (PimaxWorker != null)
            {
                if (PimaxWorker.IsAlive)
                    PimaxWorker.Abort();
            }
            PimaxWorker = null;
            // Stop eye tracking and reset
            eyeTracker.Stop();
            eyeTracker = null;
        }

        private static void OnEyeTrackerStart() => LogHelper.Debug("Pimax Eye Tracker Started!");

        private static void OnEyeTrackerStop()
        {
            LogHelper.Debug("Pimax Eye Tracker Stopped!");
            // Call Kill in-case we weren't supposed to stop
            Kill();
        }
    }
}
