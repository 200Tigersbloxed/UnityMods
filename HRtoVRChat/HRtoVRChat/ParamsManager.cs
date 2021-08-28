using System;
using System.Collections.Generic;
//using ParamLib;

namespace HRtoVRChat
{
    public static class ParamsManager
    {
        public static List<IntParameter> Parameters = new List<IntParameter>() { };

        public class IntParameter
        {
            public IntParameter(Func<HROutput, int> getVal, string parameterName)
            {
                LogHelper.Debug("ParamsManager", $"IntParameter with ParameterName: {parameterName}, has been created!");
                MainMod.OnHRValuesUpdated += (ones, tens, hundreds) =>
                {
                    HROutput hro = new HROutput()
                    {
                        ones = ones,
                        tens = tens,
                        hundreds = hundreds
                    };
                    // Find index
                    (int, VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter) foundParameter = 
                    ParamLib.ParamLib.FindParam(parameterName, VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Int);
                    if(foundParameter.Item2 != null)
                    {
                        int valToSet = getVal.Invoke(hro);
                        // Set parameter
                        bool setValue = ParamLib.ParamLib.SetParameter(foundParameter.Item1, valToSet);
                        if (!setValue)
                            LogHelper.Error("ParamsManager", $"Failed to set Parameter {parameterName} to value {valToSet}!");
                    }
                };
            }
        }

        public class HROutput
        {
            public int ones;
            public int tens;
            public int hundreds;
        }
    }
}
