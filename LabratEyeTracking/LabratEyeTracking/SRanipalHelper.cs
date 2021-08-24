// Thank you to benaclejames for VRCFaceTracking
// Most code in this file is because of them =3
// https://github.com/benaclejames/VRCFaceTracking/blob/bdcb862749d7348e337b08355f38334717718d12/VRCFaceTracking/SRanipalTrackingInterface.cs#L15

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ViveSR;
using ViveSR.anipal;
using ViveSR.anipal.Eye;

namespace LabratEyeTracking
{
    public static class SRanipalHelper
    {
        public static bool EyeTrackingEnabled = false;
        private static bool VerifyEyeError(this Error error) => error != Error.WORK && error != Error.UNDEFINED && error != (Error)1051;
        private static Thread SRanipalWorker;

        public static EyeData_v2 EyeData;

        public static void Initialize()
        {
            Error eyeError = Error.UNDEFINED;
            if (EyeTrackingEnabled)
            {
                LogHelper.Warn("SRanipal was already active, restarting... Please wait 5 seconds.");
                SRanipal_API.Release(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2);
                Thread.Sleep(5000);
            }
            eyeError = SRanipal_API.Initial(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2, IntPtr.Zero);

            if (eyeError.VerifyEyeError()) { LogHelper.Error("Failed to Initialize SRanipal Eye Tracking! EXCEPTION: " + eyeError.ToString()); return; }
            LogHelper.Debug("SRanipal Eye Tracking Initialized!"); EyeTrackingEnabled = true;
            StartThread();
        }

        public static void StartThread()
        {
            SRanipalWorker = new Thread(delegate()
            {
                while (EyeTrackingEnabled)
                {
                    UpdateEyeData();
                    Thread.Sleep(10);
                }
            });
        }

        public static void Kill()
        {
            if (!EyeTrackingEnabled)
            {
                LogHelper.Warn("Eye Tracking is not enabled, can't kill.");
                return;
            }
            // Abort Thread
            if (SRanipalWorker != null)
            {
                if (SRanipalWorker.IsAlive)
                    SRanipalWorker.Abort();
            }
            SRanipalWorker = null;
            // Release Eye Tracking
            SRanipal_API.Release(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2); LogHelper.Debug("Killed SRanipal");
        }

        public static void UpdateEyeData() { if (EyeTrackingEnabled) { SRanipal_Eye_API.GetEyeData_v2(ref EyeData); } }
    }
}
