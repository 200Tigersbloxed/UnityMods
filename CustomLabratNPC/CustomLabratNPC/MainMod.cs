using MelonLoader;
using UnityEngine.SceneManagement;

namespace CustomLabratNPC
{
    public class MainMod : MelonMod
    {
        private bool isAppQuitting;
        
        public override void OnApplicationStart()
        {
            HarmonyManager.Init();
            ConfigHelper.Config lc = ConfigHelper.LoadConfig();
            if(lc.loadUnsafeCode.Value)
                LogHelper.Error("loadUnsafeCode is true. **WARNING**: " +
                                "THIS WILL RUN ANY CODE THAT MAY CAUSE DAMAGE TO YOUR " +
                                "MACHINE! NEVER EXCEPT ANY UAC PROMPTS FROM LABRAT, AND " +
                                "ALWAYS MAKE SURE THAT LIBRARIES ARE SAFE BEFORE RUNNING! " +
                                "YOU. HAVE. BEEN. WARNED.", ShouldStackFrame:false);
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        public override void OnApplicationQuit()
        {
            isAppQuitting = true;
        }

        public override void OnPreferencesSaved()
        {
            if (!isAppQuitting)
                ConfigHelper.LoadConfig();
            else
                LogHelper.Debug("Application is quitting; not loading config.");
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            LabratTools._playerStats = LabratTools.LocalPlayer.GetComponent<PlayerStats>();
            //NPCControllerCache.DestroyAll();
        }
    }
}