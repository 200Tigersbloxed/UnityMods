using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace HRtoVRChat
{
    public static class AssetManager
    {
        public static AssetBundle iconsAssetBundle { get; private set; } = null;
        public static LoadedAssets loadedAssets { get; private set; } = new LoadedAssets();

        public class LoadedAssets
        {
            public bool didLoad = false;
            public Texture2D heartbeat;
            public Texture2D refresh;
            public Texture2D go;
            public Texture2D stop;
        }

        public static void Init()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HRtoVRChat.hrtovrchat"))
            using (var tempStream = new MemoryStream((int)stream.Length))
            {
                stream.CopyTo(tempStream);
                iconsAssetBundle = AssetBundle.LoadFromMemory_Internal(tempStream.ToArray(), 0);
                iconsAssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            if(iconsAssetBundle != null)
            {
                LoadedAssets la = new LoadedAssets();
                la.heartbeat = iconsAssetBundle.LoadAsset_Internal("Assets/heartbeat.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                la.heartbeat.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                la.refresh = iconsAssetBundle.LoadAsset_Internal("Assets/refresh.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                la.refresh.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                la.go = iconsAssetBundle.LoadAsset_Internal("Assets/go.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                la.go.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                la.stop = iconsAssetBundle.LoadAsset_Internal("Assets/stop.png", Il2CppType.Of<Texture2D>()).Cast<Texture2D>();
                la.stop.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                la.didLoad = la.heartbeat != null && la.refresh != null && la.go != null && la.stop != null;
                loadedAssets = la;
                if (loadedAssets.didLoad)
                    LogHelper.Log("AssetManager", "Loaded AssetBudle!");
                else
                    LogHelper.Error("AssetManager", "Failed to Load AssetBudle!");
            }
            else
                LogHelper.Error("AssetManager", "Failed to Load AssetBudle!");
        }
    }
}
