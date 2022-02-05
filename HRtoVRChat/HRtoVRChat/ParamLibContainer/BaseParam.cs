using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace ParamLib
{
    public class BaseParam
    {
        protected BaseParam(string paramName, VRCExpressionParameters.ValueType paramType)
        {
            ParamName = paramName;
            _paramType = paramType;
            ResetParam();
        }

        public void ResetParam() => (ParamIndex, ParameterLiteral) = ParamLib.FindParam(ParamName, _paramType);

        public void ZeroParam() => ParamIndex = null;

        protected double ParamValue
        {
            get => _paramValue;
            set
            {
                if (!ParamIndex.HasValue) return;
                if (ParamLib.SetParameter(ParamIndex.Value, (float) value))
                    _paramValue = value;
            }
        }

        public int? ParamIndex;
        
        public readonly string ParamName;
        private readonly VRCExpressionParameters.ValueType _paramType;
        protected VRCExpressionParameters.Parameter ParameterLiteral;
        private double _paramValue;
    }

    public class BoolBaseParam : BaseParam
    {
        public new bool ParamValue
        {
            get => Convert.ToBoolean(base.ParamValue);
            set => base.ParamValue = Convert.ToDouble(value);
        }
        
        protected BoolBaseParam(string paramName) : base(paramName, VRCExpressionParameters.ValueType.Bool)
        {
        }
    }

    public class IntBaseParam : BaseParam
    {
        public new int ParamValue
        {
            get => (int) base.ParamValue;
            set => base.ParamValue = value;
        }
        
        public IntBaseParam(string paramName) : base(paramName, VRCExpressionParameters.ValueType.Int)
        {
        }
    }
    
    public class FloatBaseParam : BaseParam
    {
        private static readonly List<FloatBaseParam> PrioritisedParams = new List<FloatBaseParam>();
        private readonly bool _wantsPriority;
        
        public new float ParamValue
        {
            get => (float) base.ParamValue;
            set => base.ParamValue = value;
        }
        
        public FloatBaseParam(string paramName, bool prioritised = false) : base(paramName, VRCExpressionParameters.ValueType.Float)
        {
            _wantsPriority = prioritised;
            if (_wantsPriority)
                MelonCoroutines.Start(KeepParamPrioritised());
        }

        public new void ResetParam()
        {
            base.ResetParam();

            // If we found a parameter literal, and this param need priority, and it's one of the first 8 params
            if (!ParamIndex.HasValue || !_wantsPriority) return;    // Check if we have a value since sometimes people don't use both x and y for XY params
            
            // Check if this parameter has an index lower than any of the prioritised params, and if so, replace the parameter it's lower than
            if (PrioritisedParams.Count < 8) // If we have less than 8 params, we can just add it to the end
                PrioritisedParams.Add(this);
            else
                foreach (var param in PrioritisedParams.Where(param => param.ParamIndex.HasValue && ParamIndex.Value < param.ParamIndex.Value))
                {
                    // Prioritise this param
                    PrioritisedParams.Remove(param);
                    PrioritisedParams.Add(this);
                    return;
                }
        }

        public new void ZeroParam()
        {
            base.ZeroParam();

            if (PrioritisedParams.Contains(this))
                PrioritisedParams.Remove(this);
        }
        
        private IEnumerator KeepParamPrioritised()
        {
            for (;;)
            {
                yield return new WaitForSeconds(5);
                if (!PrioritisedParams.Contains(this) || !ParamIndex.HasValue) continue;
                ParamLib.PrioritizeParameter(ParamIndex.Value);
            }
        }
    }
}