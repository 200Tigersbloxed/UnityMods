using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MelonLoader;
using UnityEngine;

namespace HRtoVRChat.ModSupport
{
    public static class AMAPI
    {
        public class NewButton
        {
            public string buttonText;
            public Action buttonAction;
            public Texture2D texture;
        }

        public static int Init(List<NewButton> nbList, Texture2D submenutexture)
        {
            int status = 0;
            int amapiIndex = MelonHandler.Mods.FindIndex(mm => mm.Info.Name == "ActionMenuApi");
            if(status != -1)
            {
                // 4
                LogHelper.Debug("ModSupport.AMAPI", "Found ActionMenuApi!");
                Type vrcampType = MelonHandler.Mods[amapiIndex].Assembly.GetType("ActionMenuApi.Api.VRCActionMenuPage");
                MethodBase asmMethod = vrcampType.GetMethod("AddSubMenu");
                if(asmMethod != null)
                {
                    object[] asmParams = new object[asmMethod.GetParameters().Length];
                    asmParams[0] = 4;
                    asmParams[1] = "HRtoVRChat";
                    asmParams[2] = new Action(delegate () 
                    {
                        Type csmType = MelonHandler.Mods[amapiIndex].Assembly.GetType("ActionMenuApi.Api.CustomSubMenu");
                        if (csmType != null)
                        {
                            MethodBase addButtonMethodBase = csmType.GetMethod("AddButton");
                            if (addButtonMethodBase != null)
                            {
                                foreach(NewButton nb in nbList)
                                {
                                    // Parameters
                                    object[] abMBParameters = new object[addButtonMethodBase.GetParameters().Length];
                                    abMBParameters[0] = nb.buttonText;
                                    abMBParameters[1] = nb.buttonAction;
                                    abMBParameters[2] = nb.texture;
                                    // Invoke
                                    var returnValue = default(object);
                                    try { returnValue = addButtonMethodBase.Invoke(null, abMBParameters); }
                                    catch (Exception e)
                                    {
                                        LogHelper.Error("ModSupport.AMAPI", "Failed to Invoke addButtonMethodBase! Exception: " + e);
                                    }
                                }
                            }
                            else
                            {
                                LogHelper.Error("ModSupport.AMAPI", "Failed to find method AddButton!");
                            }
                        }
                        else
                        {
                            LogHelper.Error("ModSupport.AMAPI", "Failed to find ActionMenuApi.Api.VRCActionMenuPage!");
                        }
                    });
                    asmParams[3] = submenutexture;
                    try { asmMethod.Invoke(null, asmParams); }
                    catch(Exception e)
                    {
                        status = 2;
                        LogHelper.Error("ModSupport.AMAPI", "Failed to create SubMenu! Exception: " + e);
                    }
                }
            }
            else
            {
                status = 1;
                LogHelper.Error("ModSupport.AMAPI", "Failed to find ActionMenuApi!");
            }

            return status;
        }
    }
}
