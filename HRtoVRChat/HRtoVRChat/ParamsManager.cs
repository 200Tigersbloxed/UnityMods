using System;
using System.Collections.Generic;
//using ParamLib;

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
            new BoolParameter(BoolCheckType.HeartBeat, "isHRBeat")
        };

        public class IntParameter : ParamLib.IntBaseParam, HRParameter
        {
            private string SetParamName = String.Empty;

            public IntParameter(Func<HROutput, int> getVal, string parameterName) : base(paramName: parameterName)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"IntParameter with ParameterName: {parameterName}, has been created!");
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds, isConnected) =>
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
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds, isConnected) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds,
                        isConnected = isConnected
                    };
                    bool valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet;
                };
            }

            public BoolParameter(BoolCheckType bct, string parameterName) : base(paramName: parameterName)
            {
                SetParamName = parameterName;
                LogHelper.Debug("ParamsManager", $"BoolParameter with ParameterName: {parameterName} and BoolCheckType of: {bct}, has been created!");
                MainMod.OnHeartBeatUpdate += (isHeartBeat) =>
                {
                    switch (bct)
                    {
                        case BoolCheckType.HeartBeat:
                            ParamValue = MainMod.isHeartBeat;
                            break;
                    }
                };
            }

            string HRParameter.GetParamName() => SetParamName;
            void HRParameter.ResetParam() => ResetParam();
            float HRParameter.GetParamValue() { if (ParamValue) return 1; else return 0; }
        }

        public class HROutput
        {
            public int ones;
            public int tens;
            public int hundreds;
            public bool isConnected;
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
