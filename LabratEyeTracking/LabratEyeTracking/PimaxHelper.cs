using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

namespace LabratEyeTracking
{
    public class PimaxHelper : IEyeTracking
    {
        public bool EyeTrackingEnabled { get; set; } = false;
        public Pimax.EyeTracking.EyeTracker eyeTracker;

        private Thread PimaxWorker;

        public void Init() 
        {
            PimaxWorker = new Thread(delegate ()
            {
                LogHelper.Debug("Initializing Pimax Eye Tracking...");
                eyeTracker = new Pimax.EyeTracking.EyeTracker();
                eyeTracker.OnStart += OnEyeTrackerStart;
                eyeTracker.OnStop += OnEyeTrackerStop;
                EyeTrackingEnabled = true;
                try { eyeTracker.Start(); LogHelper.Debug("Pimax Eye Tracking Initialized!"); }
                catch(Exception e) { LogHelper.Error($"Pimax Eye Tracking failed to Start! Exception: {e}"); }
                // Keep the Thread Alive
                while (EyeTrackingEnabled)
                {
                    UpdateEyeData();
                    Thread.Sleep(10);
                }
                // Stop eye tracking and reset
                eyeTracker.Stop();
                eyeTracker = null;
                LogHelper.Debug("Killed PimaxEyeTracking");
                // Abort Thread
                if (PimaxWorker != null)
                {
                    if (PimaxWorker.IsAlive)
                        PimaxWorker.Abort();
                }
                PimaxWorker = null;
            });
            PimaxWorker.Start();
        }

        private void UpdateEyeData()
        {
            Eye LeftEye = new Eye()
            {
                x = eyeTracker.LeftEye.Gaze.x,
                y = eyeTracker.LeftEye.Gaze.y,
                Widen = eyeTracker.LeftEye.Openness,
                origin = eyeTracker.LeftEye.GazeOrigin.normalized,
                direction = eyeTracker.LeftEye.GazeDirection.normalized
                
            };
            Eye RightEye = new Eye()
            {
                x = eyeTracker.RightEye.Gaze.x,
                y = eyeTracker.RightEye.Gaze.y,
                Widen = eyeTracker.RightEye.Openness,
                origin = eyeTracker.RightEye.GazeOrigin.normalized,
                direction = eyeTracker.RightEye.GazeDirection.normalized
            };
            UniversalEyeData.UpdateLeftEyeData(LeftEye);
            UniversalEyeData.UpdateRightEyeData(RightEye);
            MainMod.OnEyeDataUpdate.Invoke(UniversalEyeData.LeftEye, UniversalEyeData.RightEye,
                UniversalEyeData.CombinedEye);
        }

        public void Kill() 
        {
            if (!EyeTrackingEnabled)
            {
                LogHelper.Warn("Eye Tracking is not enabled, can't kill.");
                return;
            }
            EyeTrackingEnabled = false;
        }

        private void OnEyeTrackerStart() => LogHelper.Debug("Pimax Eye Tracker Started!");
        private void OnEyeTrackerStop()
        {
            LogHelper.Debug("Pimax Eye Tracker Stopped!");
            // Call Kill in-case we weren't supposed to stop
            Kill();
        }
    }
}
