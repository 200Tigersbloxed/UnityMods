using System;
using System.Collections.Generic;
using System.Text;

namespace LabratEyeTracking
{
    public static class Blinking
    {
        public static bool IsBlinking = false;

        public static void CloseEyes(Activateblink BlinkComponent)
        {
            BlinkComponent.Forcedblink();
            IsBlinking = true;
        }

        public static void OpenEyes(Activateblink BlinkComponent)
        {
            BlinkComponent.Forcedblink();
            BlinkComponent.BlinkMeterRestart();
            BlinkComponent.blinkingEnabled = false;
            IsBlinking = false;
        }

        public static void UpdateWaitTime(Activateblink BlinkComponent, float time) { BlinkComponent.waitBetween = time; }
    }
}
