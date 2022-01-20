using System.IO;
using System.Reflection;
using UnityEngine;

namespace LabratUIKit
{
    public static class AssetBundleManager
    {
        public static GameObject LabratUIKit;
        
        public static void LoadBundle()
        {
            AssetBundle loadedAssetBundle = null;
            // Get AssetBundle from Memory
            using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LabratUIKit.labratuikit"))
            using (MemoryStream ms = new MemoryStream())
            {
                if (stream != null)
                {
                    stream.CopyTo(ms);
                    loadedAssetBundle = AssetBundle.LoadFromMemory(ms.ToArray());
                }
                else
                    LogHelper.Error("Failed to LoadBundle! Stream is null!", ShouldStackFrame: false);
            }
            // Load AssetBundle's Asset(s)
            if (loadedAssetBundle != null)
            {
                GameObject LabratUIKit = loadedAssetBundle.LoadAsset<GameObject>("Assets/LabratUIKit.prefab");
                if (LabratUIKit != null)
                    AssetBundleManager.LabratUIKit = LabratUIKit;
                else
                    LogHelper.Error("Failed to find Asset!", ShouldStackFrame: false);
                loadedAssetBundle.Unload(false);
            }
            LogHelper.Debug("Loaded AssetBundle!");
        }
    }
}