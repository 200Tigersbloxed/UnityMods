using System.Linq;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LabratUIKit
{
    public class MainMod : MelonMod
    {
        public static Texture2D melonloaderImage;
        public static Texture2D modificationImage;
        
        public override void OnApplicationStart()
        {
            AssetBundleManager.LoadBundle();
            // ModMenu
            // -- Images
            byte[] mod_modImage = images.ImageTools.GetImageBytes("modification", "png");
            if (mod_modImage != null)
            {
                Texture2D texture = new images.Texture2DConvert(mod_modImage).ToTexture2D();
                modificationImage = texture;
            }
            else
                LogHelper.Error("mod_modImage is null!", ShouldStackFrame:false);
            byte[] melon_modImage = images.ImageTools.GetImageBytes("melonloader", "png");
            if (melon_modImage != null)
            {
                Texture2D texture = new images.Texture2DConvert(melon_modImage).ToTexture2D();
                melonloaderImage = texture;
            }
            else
                LogHelper.Error("melon_modImage is null!", ShouldStackFrame:false);
            // -- Load
            ExampleModMenu.CreateModMenu();
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "Menu")
            {
                Transform OptionsButton = arg0.GetRootGameObjects().Where(x => x.name == "Menu").FirstOrDefault()?
                    .transform.Find("Canvas (1)").Find("MainMenu").Find("MP").Find("OptionsButton");
                if (OptionsButton != null)
                {
                    OptionsButton.position = new Vector3(-0.56f, 3.5877f, 4.4118f);
                    OptionsButton.localScale = new Vector3(0.0133f, 0.0266f, 0.0266f);
                    OptionsButton.GetChild(0).localScale = new Vector3(1, 0.5f, 1);
                    // Create new one
                    Transform ModOptionsButton =
                        Object.Instantiate(OptionsButton, OptionsButton.transform.parent, true);
                    ModOptionsButton.transform.position = new Vector3(0.8f, 3.5877f, 4.4118f);
                    Transform ModOptionsButtonTextContainer = ModOptionsButton.GetChild(0);
                    ModOptionsButtonTextContainer.localScale = new Vector3(1, 0.5f, 1);
                    Text ModOptionsButtonText = ModOptionsButtonTextContainer.GetComponent<Text>();
                    ModOptionsButtonText.text = "MODS";
                    UIKitManager.ResetButton(ModOptionsButton.gameObject, OptionsButton.GetComponent<Button>().colors, () => 
                    {
                        if (!UIKitManager.IsModListVisible)
                            UIKitManager.CreateModList(Object.Instantiate(AssetBundleManager.LabratUIKit));
                    });
                }
                else
                    LogHelper.Error("Failed to setup ModMenu! OptionsButton is null!", ShouldStackFrame: false);
            }
        }
    }
}