using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CustomLabratNPC
{
    public static class NPCLoader
    {
        /// <summary>
        /// Defines where the WorkingDirectory for the mod is. Should not be changed in runtime.
        /// WindowsPlayer only (sorry OSXPlayer)
        /// </summary>
        public static readonly string WorkingDirectory = Application.dataPath + "/../CustomLabratNPCs";
        
        /// <summary>
        /// Location of where to Load Assemblies. THIS SHOULD NEVER BE CHANGED! (hence why its readonly)
        /// </summary>
        private static readonly string LibsDir = Path.Combine(WorkingDirectory, "Libraries");

        /// <summary>
        /// A list of all the CustomNPCDescriptors that were found and loaded from AssetBundles
        /// </summary>
        public static List<CustomNPCDescriptor> LoadedNPCs = new List<CustomNPCDescriptor>();
        
        /// <summary>
        /// Loads all the AssetBundles in the WorkingDirectory
        /// </summary>
        public static void LoadNPCs()
        {
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);
            if (!Directory.Exists(LibsDir))
                Directory.CreateDirectory(LibsDir);
            // Load Libraries, then AssetBundles
            if (ConfigHelper.LoadedConfig.loadUnsafeCode.Value)
            {
                LogHelper.Error("Loading unsafe code. **WARNING**: " +
                                "THIS WILL RUN ANY CODE THAT MAY CAUSE DAMAGE TO YOUR " +
                                "MACHINE! NEVER EXCEPT ANY UAC PROMPTS FROM LABRAT, AND " +
                                "ALWAYS MAKE SURE THAT LIBRARIES ARE SAFE BEFORE RUNNING! " +
                                "YOU. HAVE. BEEN. WARNED.", ShouldStackFrame:false);
                foreach (string library in Directory.GetFiles(LibsDir))
                {
                    Assembly assembly = Assembly.LoadFile(library);
                    LogHelper.Debug($"Loaded Library {assembly.FullName} at {library}");
                }
            }
            foreach (string file in Directory.GetFiles(WorkingDirectory))
            {
                LogHelper.Debug($"Loading file {Path.GetFileName(file)}");
                AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(file);
                if (loadedAssetBundle != null)
                {
                    Object[] loadedAssets = loadedAssetBundle.LoadAllAssets();
                    foreach (Object loadedAsset in loadedAssets)
                    {
                        if (loadedAsset is GameObject model)
                        {
                            CustomNPCDescriptor cnpcd = model.GetComponent<CustomNPCDescriptor>();
                            if (cnpcd != null)
                            {
                                if (cnpcd.UnityVersion != Application.unityVersion)
                                    LogHelper.Warn(
                                        $"NPC {cnpcd.NPCDisplayName}'s Unity Version ({cnpcd.UnityVersion})" +
                                        $" does not match {Application.unityVersion}!");
                                LoadedNPCs.Add(cnpcd);
                                LogHelper.Log($"Loaded NPC {cnpcd.NPCDisplayName}, made by: {cnpcd.Creator}");
                            }
                        }
                        else
                            LogHelper.Warn("Unknown Object " + loadedAssetBundle.name + " with type " +
                                           loadedAssetBundle.GetType().DeclaringType?.Name);
                    }
                    loadedAssetBundle.Unload(false);
                }
                else
                    LogHelper.Error("Failed to load NPC at " + file);
            }
            LogHelper.Debug("Loaded NPCs!");
        }

        /// <summary>
        /// Gets the first NPC Loaded in the list. THis will be replaced with a method of selecting
        /// which SCP you want to be used as an NPC.
        /// </summary>
        /// <param name="npcenum">
        /// The Enum of the NPC to load.
        /// </param>
        /// <returns>
        /// Returns the first CustomNPCDescriptor found; null if none are loaded.
        /// </returns>
        [CanBeNull]
        public static CustomNPCDescriptor GetFirstNPCbyNPCType(NPCEnum npcenum) =>
            LoadedNPCs.FirstOrDefault(x => x.NPCType == npcenum);

        /// <summary>
        /// Will take a CustomNPCDescriptor from a loaded AssetBundle, and Instantiate it's GameObject.
        /// </summary>
        /// <param name="npc">
        /// The CustomNPCDescriptor to get an Object from.
        /// </param>
        /// <param name="ShouldMoveToScene">
        /// Should the Object be moved to the Active Scene?
        /// </param>
        /// <returns>
        /// Returns the GameObject that was Instantiated.
        /// </returns>
        public static (CustomNPCDescriptor, GameObject) InstantiateNPC([NotNull] CustomNPCDescriptor npc, bool ShouldMoveToScene = true)
        {
            GameObject targetNPC = Object.Instantiate(npc.gameObject);
            if(ShouldMoveToScene)
                SceneManager.MoveGameObjectToScene(targetNPC, SceneManager.GetActiveScene());
            return (targetNPC.gameObject.GetComponent<CustomNPCDescriptor>(), targetNPC);
        }
    }
}