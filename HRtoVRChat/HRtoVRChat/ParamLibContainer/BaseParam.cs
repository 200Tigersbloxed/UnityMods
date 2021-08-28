using System;
using System.Collections;
using MelonLoader;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace ParamLib
{
    public class BaseParam
    {
        protected BaseParam(string paramName, VRCExpressionParameters.ValueType paramType)
        {
            _paramName = paramName;
            _paramType = paramType;
            ResetParam();
        }

        public void ResetParam()
        {
            var (paramIndex, parameter) = ParamLib.FindParam(_paramName, _paramType);
            if (parameter == null) return;

            ParamIndex = paramIndex;
        }

        public void ZeroParam() => ParamIndex = null;

        protected double ParamValue
        {
            get => _paramValue;
            set
            {
                if (!ParamIndex.HasValue) return;
                if (ParamLib.SetParameter(ParamIndex.Value, (float)value))
                    _paramValue = value;
            }
        }

        public int? ParamIndex;

        private readonly string _paramName;
        private readonly VRCExpressionParameters.ValueType _paramType;
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
            get => (int)base.ParamValue;
            set => base.ParamValue = value;
        }

        public IntBaseParam(string paramName) : base(paramName, VRCExpressionParameters.ValueType.Int)
        {
        }
    }

    public class FloatBaseParam : BaseParam
    {
        public new float ParamValue
        {
            get => (float)base.ParamValue;
            set => base.ParamValue = value;
        }

        private bool Prioritised
        {
            get => _prioritised;
            set
            {
                if (value && ParamIndex.HasValue)
                    ParamLib.PrioritizeParameter(ParamIndex.Value);

                _prioritised = value;
            }
        }
        private bool _prioritised;

        public FloatBaseParam(string paramName, bool prioritised = false) : base(paramName, VRCExpressionParameters.ValueType.Float)
        {
            if (!prioritised) return;

            Prioritised = true;
            MelonCoroutines.Start(KeepParamPrioritised());
        }

        private IEnumerator KeepParamPrioritised()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(5);
                if (!Prioritised || !ParamIndex.HasValue) continue;
                ParamLib.PrioritizeParameter(ParamIndex.Value);
            }
        }
    }

    public class XYParam
    {
        protected FloatBaseParam X, Y;

        protected Vector2 ParamValue
        {
            set
            {
                X.ParamValue = value.x;
                Y.ParamValue = value.y;
            }
        }

        protected XYParam(FloatBaseParam x, FloatBaseParam y)
        {
            X = x;
            Y = y;
        }

        protected void ResetParams()
        {
            X.ResetParam();
            Y.ResetParam();
        }

        protected void ZeroParams()
        {
            X.ParamIndex = null;
            Y.ParamIndex = null;
        }
    }
}