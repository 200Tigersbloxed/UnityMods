using System.Linq;
using UnhollowerRuntimeLib.XrefScans;
using VRC.SDK3.Avatars.ScriptableObjects;
using MethodInfo = System.Reflection.MethodInfo;

// ReSharper disable Unity.NoNullPropagation        Rider please, we both know this wasn't gonna be pretty...

namespace ParamLib
{
    public static class ParamLib
    {
        private static AvatarPlayableController LocalPlayableController => LocalAnimParamController
            ?.field_Private_AvatarPlayableController_0;

        private static AvatarAnimParamController LocalAnimParamController => VRCPlayer
            .field_Internal_Static_VRCPlayer_0
            ?.field_Private_AnimatorControllerManager_0?.field_Private_AvatarAnimParamController_0;

        private static readonly MethodInfo SetMethod = typeof(AvatarPlayableController).GetMethods().First(m =>
            m.Name.Contains("Boolean_Int32_Single") &&
            XrefScanner.UsedBy(m).All(inst => inst.Type == XrefType.Method && inst.TryResolve()?.DeclaringType == typeof(AvatarPlayableController)));

        private static readonly MethodInfo PrioritizeMethod = typeof(AvatarPlayableController).GetMethods().First(m =>
            m.Name.Contains("Void_Int32") &&
            XrefScanner.UsedBy(m).Any(inst => inst.Type == XrefType.Method && inst.TryResolve()?.DeclaringType == typeof(ActionMenu)));

        public static void PrioritizeParameter(int paramIndex)
        {
            var controller = LocalPlayableController;
            if (controller == null) return;

            PrioritizeMethod.Invoke(controller, new object[] { paramIndex });
        }

        public static (int, VRCExpressionParameters.Parameter) FindParam(string paramName, VRCExpressionParameters.ValueType paramType)
        {
            VRCExpressionParameters.Parameter[] parameters = VRCPlayer.field_Internal_Static_VRCPlayer_0
                ?.prop_VRCAvatarManager_0?.prop_VRCAvatarDescriptor_0?.expressionParameters
                ?.parameters;

            if (parameters == null)
                return (-1, null);

            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                if (param.name == null) continue;
                if (param.name == paramName && param.valueType == paramType) return (i, parameters[i]);
            }

            return (-1, null);
        }

        public static bool SetParameter(int paramIndex, float value)
        {
            var controller = LocalAnimParamController;
            if (controller?.field_Private_AvatarPlayableController_0 == null) return false;

            SetMethod.Invoke(controller.field_Private_AvatarPlayableController_0, new object[] { paramIndex, value });
            return true;
        }
    }
}