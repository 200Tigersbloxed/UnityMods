using System;
using System.Collections.Generic;

namespace HRtoVRChat
{
    public static class ParamsManager
    {
        public static readonly List<HRParameter> Parameters = new List<HRParameter>()
        {
            new IntParameter(hro => hro.ones, "onesHR"),
            new IntParameter(hro => hro.tens, "tensHR"),
            new IntParameter(hro => hro.hundreds, "hundredsHR"),
            new BoolParameter(hro => hro.isConnected, "isHRConnected"),
            new BoolParameter(BoolCheckType.HeartBeat, "isHRBeat"),
            new FloatParameter((hro) => 
            {
                float targetFloat = 0f;
                float maxhr = (float)ConfigHelper.LoadedConfig.MaxHR;
                float minhr = (float)ConfigHelper.LoadedConfig.MinHR;
                float HR = (float)hro.HR;
                if(HR > maxhr)
                    targetFloat = 1;
                else if(HR < minhr)
                    targetFloat = 0;
                else
                    targetFloat = (HR - minhr) / maxhr;
                return targetFloat;
            }, "HRPercent", false),
            new IntParameter((hro) =>
            {
                string HRstring = $"{hro.hundreds}{hro.tens}{hro.ones}";
                int HR = 0;
                try{HR = Convert.ToInt32(HRstring); }catch(Exception){}
                if(HR > 255)
                    HR = 255;
                if(HR < 0)
                    HR = 0;
                return HR;
            }, "HR")
        };

        public class IntParameter : ParamLib.IntBaseParam, HRParameter
        {
            private string SetParamName = String.Empty;

            public IntParameter(Func<HROutput, int> getVal, string parameterName) : base(paramName: parameterName)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"IntParameter with ParameterName: {parameterName}, has been created!");
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        isConnected = isConnected
                    };
                    int valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet;
                };
            }

            string HRParameter.GetParamName() => SetParamName;
            void HRParameter.ResetParam() => ResetParam();
            float HRParameter.GetParamValue() => ParamValue;
        }

        public class BoolParameter : ParamLib.BoolBaseParam, HRParameter
        {
            private string SetParamName = String.Empty;

            public BoolParameter(Func<HROutput, bool> getVal, string parameterName) : base(paramName: parameterName)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"BoolParameter with ParameterName: {parameterName}, has been created!");
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        HR = HR,
                        isConnected = isConnected,
                        isActive = isActive
                    };
                    bool valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet;
                };
            }

            public BoolParameter(BoolCheckType bct, string parameterName) : base(paramName: parameterName)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"BoolParameter with ParameterName: {parameterName} and BoolCheckType of: {bct}, has been created!");
                MainMod.OnHeartBeatUpdate += (isHeartBeat, shouldRestart) =>
                {
                    switch (bct)
                    {
                        case BoolCheckType.HeartBeat:
                            ParamValue = isHeartBeat;
                            break;
                    }
                };
            }

            string HRParameter.GetParamName() => SetParamName;
            void HRParameter.ResetParam() => ResetParam();
            float HRParameter.GetParamValue() { if (ParamValue) return 1; else return 0; }
        }

        public class FloatParameter : ParamLib.FloatBaseParam, HRParameter
        {
            private string SetParamName = String.Empty;

            public FloatParameter(Func<HROutput, float> getVal, string parameterName, bool prioritisedFloat) : base(paramName: parameterName, prioritised: prioritisedFloat)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"FloatParameter with ParameterName: {parameterName} and Priority set to: {prioritisedFloat}, has been created!");
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds, HR, isConnected, isActive) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        HR = HR,
                        isConnected = isConnected,
                        isActive = isActive
                    };
                    float targetValue = getVal.Invoke(hro);
                    ParamValue = targetValue;
                };
            }

            string HRParameter.GetParamName() => SetParamName;
            float HRParameter.GetParamValue() => ParamValue;
            void HRParameter.ResetParam() => ResetParam();
        }

        public class HROutput
        {
            public int ones;
            public int tens;
            public int hundreds;
            public int HR;
            public bool isConnected;
            public bool isActive;
        }

        public interface HRParameter
        {
            public string GetParamName();
            public float GetParamValue();
            public void ResetParam();
        }

        public enum BoolCheckType
        {
            HeartBeat
        }
    }
}
