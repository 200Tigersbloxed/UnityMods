using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Assembly = System.Reflection.Assembly;
using Color = System.Drawing.Color;
using Debug = UnityEngine.Debug;

// ReSharper disable once CheckNamespace
namespace CustomLabratNPC
{
    public static class EditorInfo
    {
        public static List<CustomNPCDescriptor> descriptors;
        public static CustomNPCDescriptor descriptor;
        public static bool isBuilding;

        public static string Output { get; set; }
        public static string CreatorName { get; set; }
        public static bool AllowOverwrite { get; set; }
        public static bool ResetBundlesAndPrefabs { get; set; }
        
        public static string textcolor
        {
            get
            {
                if (EditorGUIUtility.isProSkin)
                    return "white";
                return "black";
            }
        }
    }

    public static class EditorActions
    {
        private static readonly string WorkingDirectory = "Assets/CustomLabratNPC";
        private static string PrefabDirectory => Path.Combine(WorkingDirectory, "Prefabs");
        private static string AssetBundleDirectory => Path.Combine(WorkingDirectory, "AssetBundles");
        
        public static string SetOutputDialog(string title, string folder = "", string defaultName = "") =>
            EditorUtility.OpenFolderPanel(title, folder, defaultName);

        private static System.Drawing.Bitmap image;
        private static Texture2D texture;

        public static void DrawHeader(Vector2 windowPosition, Vector2 windowSize)
        {
            // Get image and draw texture
            if (image == null)
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("CustomLabratNPC_Editor.UnityAssets.scpcnlogo.png"))
                {
                    if (stream != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            image = new System.Drawing.Bitmap(ms);
                        }
                    }
                }
            }
            // Check if we need to convert the bytes to a texture
            if (texture == null && image != null)
            {
                texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, false)
                {
                    filterMode = FilterMode.Trilinear
                };
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color pixelColor = image.GetPixel(x, y);
                        UnityEngine.Color unity_pixelColor =
                            new UnityEngine.Color(pixelColor.R / 255.0f, pixelColor.G / 255.0f, 
                                pixelColor.B / 255.0f, pixelColor.A / 255.0f);
                        texture.SetPixel(x, image.Height - y, unity_pixelColor);
                    }
                }
                texture.Apply();
            }
            // Image Scaling
            float maxXsize = 400;
            float maxYsize = 200;
            float imagesizeX = windowSize.x - 20;
            float imagesizeY = windowSize.x / 2 - 20;
            if (imagesizeX > maxXsize)
                imagesizeX = maxXsize;
            if (imagesizeY > maxYsize)
                imagesizeY = maxYsize;
            Rect imageLayout = new Rect((windowSize.x - imagesizeX) / 2, 10, imagesizeX, imagesizeY);
            Rect area = new Rect(0, imagesizeY + 20, windowSize.x, windowSize.y - imagesizeY - 20);
            // Draw the Image
            EditorGUI.DrawPreviewTexture(imageLayout, texture);
            // Set an area
            GUILayout.BeginArea(area);
            // Draw everything
            GUILayout.Label("SCP Labrat", EditorStyles.miniLabel);
            GUILayout.Label("CustomLabratNPC by 200Tigersbloxed", EditorStyles.miniLabel);
        }

        public static bool BuildAssetBundle(string output)
        {
            bool didSavePrefab;
            Debug.Log("Setting up environment!");
            UnityAssetManager.ClearData(WorkingDirectory);
            UnityAssetManager.VerifyBuildDirectories(WorkingDirectory, PrefabDirectory, AssetBundleDirectory, true);
            Debug.Log("Pushing Editor Window Values!");
            EditorInfo.descriptor.Creator = EditorInfo.CreatorName;
            if (string.IsNullOrEmpty(EditorInfo.descriptor.NPCDisplayName))
                EditorInfo.descriptor.NPCDisplayName = EditorInfo.descriptor.gameObject.name;
            EditorInfo.descriptor.UnityVersion = Application.unityVersion;
            Debug.Log("Saving as Prefab!");
            string assetToSave = Path.Combine(PrefabDirectory, EditorInfo.descriptor.gameObject.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(EditorInfo.descriptor.gameObject, assetToSave, out didSavePrefab);
            if (didSavePrefab && File.Exists(assetToSave))
            {
                Debug.Log("Building assetBundle!");
                string bundleName = EditorInfo.descriptor.gameObject.name;
                string[] assets = { assetToSave };
                AssetBundleBuild[] builds = new AssetBundleBuild[1];
                builds[0].assetBundleName = bundleName;
                builds[0].assetNames = assets;
                builds[0].assetBundleVariant = "lnpc";
                string OutputAssetBundleDirectory =
                    Path.Combine(AssetBundleDirectory, EditorInfo.descriptor.gameObject.name);
                if (!Directory.Exists(OutputAssetBundleDirectory))
                    Directory.CreateDirectory(OutputAssetBundleDirectory);
                BuildPipeline.BuildAssetBundles(OutputAssetBundleDirectory, builds,
                    BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
                string OutputFileCopy = String.Empty;
                foreach (string assetBundle in Directory.GetFiles(OutputAssetBundleDirectory))
                {
                    string assetBundleName = Path.GetFileName(assetBundle);
                    if (assetBundleName.Contains(bundleName) && assetBundleName.Contains(".lnpc") &&
                        !assetBundleName.Contains(".manifest") && !assetBundleName.Contains(".meta"))
                    {
                        Debug.Log("Found Output AssetBundle! Copying to Output...");
                        string outcopy = Path.Combine(output, assetBundleName);
                        OutputFileCopy = outcopy;
                        if(!File.Exists(outcopy))
                            File.Copy(assetBundle, outcopy);
                        else if (EditorInfo.AllowOverwrite)
                        {
                            File.Delete(outcopy);
                            File.Copy(assetBundle, outcopy);
                        }
                        else
                        {
                            string[] newoutcopy = assetBundleName.Split('.');
                            newoutcopy[0] = newoutcopy[0] + Directory.GetFiles(EditorInfo.Output).Length;
                            File.Copy(assetBundle, EditorInfo.Output + "\\" + newoutcopy[0] + 
                                                   '.' + newoutcopy[1]);
                            OutputFileCopy = EditorInfo.Output + "/" + newoutcopy[0] + '.' + newoutcopy[1];
                        }
                    }
                }
                string OutputFileCopyDirectory = Path.GetDirectoryName(OutputFileCopy) ?? String.Empty;
                if (!string.IsNullOrEmpty(OutputFileCopy))
                {
                    Debug.Log("Completed Operation!");
                    if (EditorUtility.DisplayDialog("CustomLabratNPC Builder",
                        $"Successfully built NPC {EditorInfo.descriptor.gameObject.name} \n {OutputFileCopy}", "Go to File",
                        "OK"))
                        Process.Start(OutputFileCopyDirectory);
                    return true;
                }
                Debug.LogError("Failed to Copy Assetbundle!");
            }
            else
                Debug.LogError("Prefab failed to save or prefab does not exist!");
            // Failed to Copy File
            EditorUtility.DisplayDialog("CustomLabratNPC Builder",
                "Failed to build avatar! Please see console for more information.", "OK");
            return false;
        }
    }
    
    public class EditorBuilderSDK : EditorWindow
    {
        private static EditorBuilderSDK window;
        private static void NewGUILine() => GUILayout.Label("", EditorStyles.largeLabel);
        
        [MenuItem("CustomLabratNPC/Builder")]
        private static void ShowWindow()
        {
            window = GetWindow<EditorBuilderSDK>();
            window.titleContent = new GUIContent("CustomLabratNPC Builder");
        }

        private void DrawBuilder()
        {
            EditorActions.DrawHeader(new Vector2(window.position.x, window.position.y),
                new Vector2(window.position.width, window.position.height));
            NewGUILine();
            if (!UnityAssetManager.VerifyDirectories(true))
            {
                GUILayout.Label("Missing files!", EditorStyles.largeLabel);
                GUILayout.Label("These files may be crucial to the building process of an NPC. " +
                                "Please hit the button below to import",
                    EditorStyles.label);
                if (GUILayout.Button("Import Missing Files"))
                {
                    UnityAssetManager.ExtractRequiredFiles(RunCustomFolders: true);
                }
                NewGUILine();
            }
            if (!PlayerSettings.virtualRealitySupported)
            {
                GUILayout.Label("Virtual Reality Support is not enabled!", EditorStyles.largeLabel);
                GUILayout.Label("Having this disabled will prompt some odd rendering results! Please enable it below.",
                    EditorStyles.label);
                if (GUILayout.Button("Enable Virtual Reality Support"))
                {
                    PlayerSettings.virtualRealitySupported = true;
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new string[] {"OpenVR"});
                }
                NewGUILine();
            }
            if (PlayerSettings.virtualRealitySupported && (PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone).Length > 1 ||
                PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone).Length == 0))
            {
                List<string> invalidSDKs = new List<string>();
                foreach (string virtualRealitySDK in PlayerSettings.GetAvailableVirtualRealitySDKs(BuildTargetGroup.Standalone))
                {
                    if (!virtualRealitySDK.Contains("OpenVR"))
                    {
                        invalidSDKs.Add(virtualRealitySDK);
                    }
                }

                string invalidsdk()
                {
                    string bruh = String.Empty;
                    foreach (string invalidSDK in invalidSDKs)
                    {
                        if(!invalidSDK.Contains("None") && !invalidSDK.Contains("stereo"))
                            bruh = bruh + invalidSDK + " ";
                    }

                    return bruh;
                }
                GUILayout.Label("You have invalid Virtual Reality SDKs enabled!", EditorStyles.largeLabel);
                GUILayout.Label("This may cause odd results, please reset your SDKs below.", EditorStyles.label);
                GUILayout.Label($"Invalid SDKs: {invalidsdk()}", EditorStyles.miniLabel);
                if (GUILayout.Button("Reset Virtual Reality SDKs"))
                {
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, new string[] {"OpenVR"});
                }
                NewGUILine();
            }
            if (PlayerSettings.virtualRealitySupported &&
                PlayerSettings.stereoRenderingPath != StereoRenderingPath.SinglePass)
            {
                GUILayout.Label("SinglePass rendering is not enabled!", EditorStyles.largeLabel);
                GUILayout.Label("This will cause significant rendering issues, please enable this below!",
                    EditorStyles.label);
                if (GUILayout.Button("Enable SinglePass Rendering"))
                {
                    PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
                }
                NewGUILine();
            }
            if (EditorInfo.descriptor != null)
            {
                GUILayout.Label("Avatar Info", EditorStyles.largeLabel);
                GUILayout.Label("Descriptor Avatar: " + EditorInfo.descriptor.gameObject.name, EditorStyles.boldLabel);
                // Warnings
                Animator _animator = EditorInfo.descriptor.gameObject.GetComponent<Animator>();
                if (_animator != null)
                {
                    // Check if the Animators Controller is the T-Pose
                    RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;
                    if (runtimeAnimatorController != null && runtimeAnimatorController.name != "NPCTPose")
                    {
                        NewGUILine();
                        GUILayout.Label("Incorrect Animator Controller!", EditorStyles.largeLabel);
                        GUILayout.Label("Your NPC may have an odd armature animation if you don't know what " +
                                        "you're doing!",
                            EditorStyles.label);
                        GUILayout.Label($"Controller Name: {runtimeAnimatorController.name}",
                            EditorStyles.miniLabel);
                        NewGUILine();
                    }
                }
                // Info
                EditorInfo.CreatorName = EditorGUILayout.TextField("Creator Name", EditorInfo.CreatorName);
                NewGUILine();
                // Output
                GUILayout.Label("Build Options", EditorStyles.largeLabel);
                EditorInfo.Output = EditorGUILayout.TextField("Builder Output", EditorInfo.Output);
                if (GUILayout.Button("Select Output Directory"))
                {
                    if(!EditorInfo.isBuilding)
                        EditorInfo.Output = EditorActions.SetOutputDialog("Select Output Directory");
                }
                EditorInfo.AllowOverwrite = GUILayout.Toggle(EditorInfo.AllowOverwrite, "Allow Overwriting of Files");
                EditorInfo.ResetBundlesAndPrefabs = GUILayout.Toggle(EditorInfo.ResetBundlesAndPrefabs,
                    "Reset All AssetBundles and Prefabs");
                if (GUILayout.Button("Build!"))
                {
                    if (!EditorInfo.isBuilding)
                    {
                        if(EditorActions.BuildAssetBundle(EditorInfo.Output))
                            Debug.Log("BuildAssetBundle Returned True!");
                        else
                            Debug.LogWarning("BuildAssetBundle failed!");
                    }
                }
                NewGUILine();
                if (GUILayout.Button("Go Back"))
                    EditorInfo.descriptor = null;
            }
            else if (EditorInfo.descriptors.Count >= 1)
            {
                GUILayout.Label("Please select a Descriptor");
                foreach (CustomNPCDescriptor customNpcDescriptor in EditorInfo.descriptors)
                {
                    if (GUILayout.Button(customNpcDescriptor.gameObject.name))
                        EditorInfo.descriptor = customNpcDescriptor;
                }
            }
            else
                GUILayout.Label("Please add a CustomNPCDescriptor to an object!");
        }

        private void OnGUI()
        {
            if (window != null)
            {
                // Find the info
                EditorInfo.descriptors = FindObjectsOfType<CustomNPCDescriptor>().ToList();
                EditorInfo.descriptors.Reverse();
                // Draw
                DrawBuilder();
            }
            GUILayout.EndArea();
        }
    }
}