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
            int status = 0;
            int uixIndex = -1;
            try { uixIndex = MelonHandler.Mods.FindIndex(mm => mm.Info.Name == "UI Expansion Kit"); }
            catch (Exception) { }
            if(uixIndex >= 0)
            {
                LogHelper.Debug("ModSupport.UIX", "Found UIX!");
                MethodBase gemMethod = MelonHandler.Mods[uixIndex].Assembly.GetType("UIExpansionKit.API.ExpansionKitApi").GetMethod("GetExpandedMenu", BindingFlags.Static | BindingFlags.Public);
                if(gemMethod != null)
                {
                    object[] gemParams = new object[gemMethod.GetParameters().Length];
                    gemParams[0] = 0;
                    object objASB = gemMethod.Invoke(null, gemParams);
                    if (objASB != null)
                    {
                        MethodBase asbMethod = objASB.GetType().GetMethod("AddSimpleButton",
                            BindingFlags.Instance | BindingFlags.Public,
                            null,
                            new[] { typeof(string), typeof(Action), typeof(Action<UnityEngine.GameObject>) },
                            null);
                        if(asbMethod != null)
                        {
                            var asbReturnValue = default(object);
                            object[] asbParameters = new object[asbMethod.GetParameters().Length];
                            asbParameters[0] = "RestartHRListener";
                            asbParameters[1] = buttonAction;
                            asbParameters[2] = null;
                            try { asbReturnValue = asbMethod.Invoke(objASB, asbParameters); }
                            catch (Exception e)
                            {
                                LogHelper.Error("ModSupport.UIX", "asbMethod failed to invoke! Exception: " + e);
                                status = 5;
                            }
                        }
                        else
                        {
                            LogHelper.Error("ModSupport.UIX", "asbMethod returned null!");
                            status = 4;
                        }
                    }
                    else
                    {
                        LogHelper.Error("ModSupport.UIX", "GetExpandedMenu returned null!");
                        status = 3;
                    }
                }
                else
                {
                    LogHelper.Error("ModSupport.UIX", "Could not find GetExpandedMenu method!");
                    status = 2;
                }
            }
            else
            {
                LogHelper.Error("ModSupport.UIX", "Failed to find UIX!");
                status = 1;
            }

            return status;
        }
    }
}
