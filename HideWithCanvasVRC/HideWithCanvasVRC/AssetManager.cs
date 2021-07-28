using UnityEngine;
using UnityEngine.SceneManagement;

namespace HideWithCanvasVRC
{
    public static class AssetManager
    {
        public static AssetBundle loadedAssetBundle;
        public static GameObject prefab = null;
        public static bool isSetup
        {
            get
            {
                bool bTR = false;
                foreach(GameObject child in SceneManager.GetActiveScene().GetRootGameObjects())
                {
                    if (child.name.ToLower() == "hidewithcanvasvrc(clone)")
                        bTR = true;
                }

                return bTR;
            }
        }

        public static void LoadPrefabFromPath(string path, bool forceLoad = false)
        {
            if(prefab == null || forceLoad)
            {
                loadedAssetBundle = AssetBundle.LoadFromFile(path);
                if (loadedAssetBundle == null)
                {
                    LogHelper.Log("Failed to load asset bundle!");
                }
                else
                {
                    LogHelper.Log("Loaded Asset Bundle!");
                }
            }
        }

        public static void SetupPrefabInScene()
        {
            if(!isSetup)
            {
                LogHelper.Log("Setting up prefab in scene...");
                GameObject newPrefab = GameObject.Instantiate(loadedAssetBundle.LoadAsset<GameObject>("Assets/HideWithCanvasVRC/HideWithCanvasVRC.prefab")).AddComponent<NotificationManager>().gameObject;
                if(newPrefab == null)
                {
                    LogHelper.Error("newPrefab is null!");
                }
            }
            else
            {
                LogHelper.Error("prefab is already setup!");
            }
        }
    }
}
