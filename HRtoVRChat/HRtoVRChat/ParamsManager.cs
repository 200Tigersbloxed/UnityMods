using System;
using System.Collections.Generic;
//using ParamLib;

namespace HRtoVRChat
{
    public static class ParamsManager
    {
        public static List<IntParameter> Parameters = new List<IntParameter>() { };

        public class IntParameter : ParamLib.IntBaseParam
        {
            public IntParameter(Func<HROutput, int> getVal, string parameterName) : base(paramName: parameterName)
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
                    int valueToSet = getVal.Invoke(hro);
                    ParamValue = valueToSet;
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
