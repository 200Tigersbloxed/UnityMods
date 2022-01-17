// Thank you to benaclejames for VRCFaceTracking
// Most code in this file is because of them =3
// https://github.com/benaclejames/VRCFaceTracking/blob/bdcb862749d7348e337b08355f38334717718d12/VRCFaceTracking/SRanipalTrackingInterface.cs#L15

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViveSR;
using ViveSR.anipal;
using ViveSR.anipal.Eye;

namespace LabratEyeTracking
{
    public class SRanipalHelper : IEyeTracking
    {
        public bool EyeTrackingEnabled { get; set; } = false;
        private Thread SRanipalWorker;

        public static EyeData_v2 EyeData;

        public void Init()
        {
            if (!EyeTrackingEnabled)
            {
                StartThread();
            }
            else
                LogHelper.Warn("SRanipal is already initialized!");
        }

        public void StartThread()
        {
            SRanipalWorker = new Thread(() =>
            {
                LogHelper.Debug("Initializing SRanipal SDK...");
                EyeTrackingEnabled = true;
                try
                {
                    SRanipal_API.Initial(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2, IntPtr.Zero);
                    LogHelper.Debug("SRanipal Eye Tracking Initialized!");
                }
                catch (Exception e)
                {
                    EyeTrackingEnabled = false;
                    LogHelper.Error("Failed to Initialize SRanipal Eye Tracking! EXCEPTION: " + e);
                    return;
                }
                while (EyeTrackingEnabled)
                {
                    UpdateEyeData();
                    Thread.Sleep(10);
                }
                // Release Eye Tracking
                SRanipal_API.Release(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2);
                LogHelper.Debug("Killed SRanipal");
                // Abort Thread
                if (SRanipalWorker != null)
                {
                    if (SRanipalWorker.IsAlive)
                        SRanipalWorker.Abort();
                }
                SRanipalWorker = null;
            });
            SRanipalWorker.Start();
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

        public void UpdateEyeData() 
        { 
            if (EyeTrackingEnabled) 
            { 
                SRanipal_Eye_API.GetEyeData_v2(ref EyeData);
                Eye LeftEye = new Eye()
                {
                    x = EyeData.verbose_data.left.pupil_position_in_sensor_area.x,
                    y = EyeData.verbose_data.left.pupil_position_in_sensor_area.y,
                    Widen = EyeData.verbose_data.left.eye_openness,
                    origin = EyeData.verbose_data.left.gaze_origin_mm.normalized,
                    direction = EyeData.verbose_data.left.gaze_direction_normalized
                };
                Eye RightEye = new Eye()
                {
                    x = EyeData.verbose_data.right.pupil_position_in_sensor_area.x,
                    y = EyeData.verbose_data.right.pupil_position_in_sensor_area.y,
                    Widen = EyeData.verbose_data.right.eye_openness,
                    origin = EyeData.verbose_data.right.gaze_origin_mm.normalized,
                    direction = EyeData.verbose_data.right.gaze_direction_normalized
                };
                UniversalEyeData.UpdateLeftEyeData(LeftEye);
                UniversalEyeData.UpdateRightEyeData(RightEye);
                MainMod.OnEyeDataUpdate.Invoke(UniversalEyeData.LeftEye, UniversalEyeData.RightEye,
                    UniversalEyeData.CombinedEye);
            }
        }
    }
}
