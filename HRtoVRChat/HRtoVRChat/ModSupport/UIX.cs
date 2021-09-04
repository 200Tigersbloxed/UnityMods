using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRtoVRChat.ModSupport
{
    public static class UIX
    {
        public static int Init(Action buttonAction)
        {
            // Create UIX Button if we have UIX
            int uixIndex = MelonHandler.Mods.FindIndex(mm => mm.Info.Name == "UI Expansion Kit");
            bool foundUIX = uixIndex != -1;
            if (foundUIX)
            {
                LogHelper.Log("ModSupport.UIX", "Found UIExpansionKit!");
                MethodBase gemMethod = MelonHandler.Mods[uixIndex].Assembly.GetType("UIExpansionKit.API.ExpansionKitApi").GetMethod("GetExpandedMenu", BindingFlags.Static | BindingFlags.Public);
                // Enums
                Type enumType = MelonHandler.Mods[uixIndex].Assembly.GetType("UIExpansionKit.API.ExpandedMenu");
                object[] enumValues = Enum.GetValues(enumType).Cast<object>().ToArray();
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                // Return Objects
                object gemReturnObject = null;
                object asbReturnObject = null;
                // Parameters
                object[] gemParameters = new object[1];
                gemParameters[0] = Convert.ChangeType(enumValues[0], underlyingType);
                object[] asbParameters = new object[3];
                asbParameters[0] = "Restart HRListener";
                asbParameters[1] = buttonAction;
                asbParameters[2] = null;
                // Get Return Method
                object returnValue = gemMethod.Invoke(gemReturnObject, gemParameters);
                MethodBase asbMethod = null;
                if (returnValue != null)
                    asbMethod = returnValue.GetType().GetMethod("AddSimpleButton");
                else if (gemReturnObject != null)
                    asbMethod = gemReturnObject.GetType().GetMethod("AddSimpleButton");
                else
                {
                    LogHelper.Error("ModSupport.UIX", "Failed to GetExpandedMenu return!");
                    return 2;
                }
                // Invoke Return Method
                if (asbMethod != null)
                    asbMethod.Invoke(asbReturnObject, asbParameters);
                else
                {
                    LogHelper.Error("ModSupport.UIX", "Failed to AddSimpleButton!");
                    return 3;
                }
            }
            else
            {
                LogHelper.Warn("ModSupport.UIX", "Could not find UIExpansionKit! This is a helpful tool, you should think about installing it!");
                return 1;
            }

            return 0;
        }
    }
}
