using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace CustomLabratNPC
{
    public static class UnityAssetManager
    {
        private static Dictionary<string, Func<string, bool>> CustomFolders =
            new Dictionary<string, Func<string, bool>>();
        private static Dictionary<string, string[]> DontDeleteFolders = new Dictionary<string, string[]>
        {
            {"Animations", new [] {"NPCTPose.controller", "T-Pose.anim"}},
            {"DynamicBone", null},
            {"ScriptCompiler", null}
        };
        /// <summary>
        /// Checks if a Directory should be deleted.
        /// </summary>
        /// <param name="folderName">Directory to check</param>
        /// <returns></returns>
        public static bool ShouldDeleteFolder(string folderName) => !DontDeleteFolders.ContainsKey(folderName);

        /// <summary>
        /// Defines whether the Builder should delete your folder in CustomLabratNPC
        /// </summary>
        /// <param name="folderName">Folder name to delete (caps sensitive!)</param>
        /// <param name="requiredFiles">Files to be loaded from EmbeddedResource and Saved</param>
        public static void AddFolderToNoDelete(string folderName, string[] requiredFiles) =>
            DontDeleteFolders.Add(folderName, requiredFiles);

        /// <summary>
        /// Add a Custom Folder for SDK initialization. AddFolderToNoDelete must be setup before this!
        /// </summary>
        /// <param name="folderName">Name of folder. Must be same as AddFolderToNoDelete's folderName!</param>
        /// <param name="initFunction">
        /// Function to be called when initializing.
        /// string is WorkingDirectory (save everything there)
        /// bool is if the function worked (dont throw an exception, return false)
        /// </param>
        public static void AddCustomFolder(string folderName, Func<string, bool> initFunction)
        {
            if(!ShouldDeleteFolder(folderName))
                CustomFolders.Add(folderName, initFunction);
            else
                Debug.LogWarning("Failed to find folderName registered! Have you setup AddFolderToNoDelete?");
        }
        
        /// <summary>
        /// Clear any data leftover in the CustomLabratNPC folder. Respects ResetBundlesAndPrefabs.
        /// </summary>
        /// <param name="WorkingDirectory">The CustomLabratNPC folder's location</param>
        /// <param name="IgnoreSafeFolders">
        /// Should we delete folders that are marked as safe too?
        /// (Ignore ShouldDeleteFolder(string))
        /// </param>
        public static void ClearData(string WorkingDirectory, bool IgnoreSafeFolders = false)
        {
            if (EditorInfo.ResetBundlesAndPrefabs && Directory.Exists(WorkingDirectory))
            {
                foreach (string directory in Directory.GetDirectories(WorkingDirectory))
                {
                    if(ShouldDeleteFolder(new DirectoryInfo(directory).Name) || IgnoreSafeFolders)
                        Directory.Delete(directory, true);
                }
            }
        }
        
        /// <summary>
        /// Checks to see if required Build Directories exist
        /// </summary>
        /// <param name="WorkingDirectory">The CustomLabratNPC folder</param>
        /// <param name="PrefabDirectory">The Prefabs location required for building</param>
        /// <param name="AssetBundleDirectory">The AssetBundle location required for building</param>
        /// <param name="createIfNull">Create the folder if it doesn't exist</param>
        /// <returns></returns>
        public static bool VerifyBuildDirectories(string WorkingDirectory, string PrefabDirectory, 
            string AssetBundleDirectory, bool createIfNull = false)
        {
            if (createIfNull)
            {
                if (!Directory.Exists(WorkingDirectory))
                    Directory.CreateDirectory(WorkingDirectory);
                if (!Directory.Exists(PrefabDirectory))
                    Directory.CreateDirectory(PrefabDirectory);
                if (!Directory.Exists(AssetBundleDirectory))
                    Directory.CreateDirectory(AssetBundleDirectory);
            }
            else
            {
                if (!Directory.Exists(WorkingDirectory))
                    return false;
                if (!Directory.Exists(PrefabDirectory))
                    return false;
                if (!Directory.Exists(AssetBundleDirectory))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks to see if all specified directories exist
        /// </summary>
        /// <param name="WorkingDirectory">CustomLabratNPC folder</param>
        /// <param name="RunDefaults">Check the default folders</param>
        /// <param name="RunCustomFolders">Check the CustomFolders</param>
        /// <returns></returns>
        public static bool VerifyDirectories(bool RunDefaults = false, bool RunCustomFolders = false, 
            string WorkingDirectory = "Assets/CustomLabratNPC")
        {
            if (RunDefaults)
            {
                foreach (KeyValuePair<string,string[]> dontDeleteFolder in DontDeleteFolders)
                {
                    // Check Directory
                    string directory = Path.Combine(WorkingDirectory, dontDeleteFolder.Key);
                    if (!Directory.Exists(directory))
                        return false;
                    // Check File
                    if (dontDeleteFolder.Value != null)
                    {
                        foreach (string file in dontDeleteFolder.Value)
                        {
                            if (!File.Exists(Path.Combine(directory, file)))
                                return false;
                        }
                    }
                }
            }
            if (RunCustomFolders)
            {
                foreach (var customFolder in CustomFolders)
                {
                    string directory = Path.Combine(WorkingDirectory, customFolder.Key);
                    if (!Directory.Exists(directory))
                        return false;
                }
            }
            return true;
        }
        
        // https://stackoverflow.com/a/10412442
        private static byte[] GetEmbeddedResource(string @namespace, string filename)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(@namespace + "." +filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        /// <summary>
        /// Extracts any required files by both an SDK and/or Mod
        /// </summary>
        /// <param name="WorkingDirectory">CustomLabratNPC folder</param>
        /// <param name="OverrideNamespace">Namespace to extract assemblies from (CustomFolders only)</param>
        /// <param name="RunCustomFolders">Extract files from CustomFolders</param>
        /// <param name="RunDefaults">Extract files required by this plugin</param>
        public static void ExtractRequiredFiles(string WorkingDirectory = "Assets/CustomLabratNPC", 
            string OverrideNamespace = "CustomLabratNPC_Editor.UnityAssets", bool RunCustomFolders = false, 
            bool RunDefaults = false)
        {
            foreach (KeyValuePair<string, string[]> dontDeleteFolder in DontDeleteFolders)
            {
                // Run default namespaces
                if (RunCustomFolders)
                {
                    if (OverrideNamespace == "CustomLabratNPC_Editor.UnityAssets")
                    {
                        string directory = Path.Combine(WorkingDirectory, dontDeleteFolder.Key);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        switch (dontDeleteFolder.Key)
                        {
                            case "Animations":
                                foreach (string fileToExtract in dontDeleteFolder.Value)
                                {
                                    byte[] file = GetEmbeddedResource(OverrideNamespace, fileToExtract);
                                    if (file != null)
                                    {
                                        File.WriteAllBytes(Path.Combine(directory, fileToExtract), file);
                                        Debug.Log("Extracted Required File " + fileToExtract + " to " + directory);
                                    }
                                    else
                                        Debug.LogError("Failed to get bytes[] for file " + fileToExtract);
                                }

                                break;
                        }
                    }
                }
                // Run CustomFolders
                if (RunDefaults)
                {
                    foreach (KeyValuePair<string, Func<string, bool>> customFolder in CustomFolders)
                    {
                        string directory = Path.Combine(WorkingDirectory, customFolder.Key);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        bool didWork = customFolder.Value.Invoke(directory);
                        if(!didWork)
                            Debug.LogError("Failed to init custom folder " + customFolder.Key);
                    }
                }
            }
        }
    }

    public class AnimatorPlayables
    {
        public AnimatorControllerPlayable AnimatorControllerPlayable;
        public PlayableGraph PlayableGraph;
        public RuntimeAnimatorController TargetAnimatorController;
    }
}