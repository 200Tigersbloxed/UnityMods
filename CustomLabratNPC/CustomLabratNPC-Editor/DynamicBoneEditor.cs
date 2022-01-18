using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CustomLabratNPC
{
    public static class AssemblyInfo
    {
        public static string DynamicBoneLocation { get; set; }

        public static void BuildDynamicBoneToAssembly()
        {
            if (UnityAssetManager.VerifyDirectories(true))
            {
                List<string> scriptPaths = new List<string>();
                foreach (string file in Directory.GetFiles(Path.Combine(DynamicBoneLocation, "Scripts")))
                {
                    if(!file.Contains(".meta"))
                        scriptPaths.Add(file);
                }

                AssemblyBuilder ab = new AssemblyBuilder("Assets/CustomLabratNPC/DynamicBone/DynamicBone.dll",
                    scriptPaths.ToArray());
                ab.buildStarted += s => Debug.Log("Starting AssemblyBuild");
                ab.buildFinished += (s, messages) =>
                {
                    bool didError = false;
                    foreach (CompilerMessage compilerMessage in messages)
                    {
                        Debug.LogError("DynamicBone Assembly Bundler Error: " + compilerMessage.message);
                        didError = true;
                    }

                    if (!didError)
                    {
                        // Success
                        if (EditorUtility.DisplayDialog("CustomLabratNPC DynamicBone Bundler",
                                "Successfully built DynamicBone Assembly", "Go to File",
                                "OK"))
                            Process.Start(Path.GetFullPath("Assets/CustomLabratNPC/DynamicBone"));
                    }
                    else
                        EditorUtility.DisplayDialog("CustomLabratNPC DynamicBone Builder",
                            "Failed to bundle DynamicBones to an assembly! Check Console for more details", "OK");
                };
                ab.Build();
            }
            else
                Debug.LogError("Failed to Build DynamicBone to Assembly! VerifyDirectories is false");
        }
    }
    
    public class DynamicBoneEditor : EditorWindow
    {
        private static DynamicBoneEditor window;
        
        [MenuItem("CustomLabratNPC/DynamicBone Bundler")]
        private static void ShowWindow()
        {
            window = GetWindow<DynamicBoneEditor>();
            window.titleContent = new GUIContent("DynamicBone");
        }
        
        private static void NewGUILine() => GUILayout.Label("", EditorStyles.largeLabel);

        private void DrawWarning(string warntitle, string description, bool hasButton = false, string buttonText = "",
            Action buttonAction = null)
        {
            GUILayout.Label(warntitle, EditorStyles.largeLabel);
            GUILayout.Label(description, EditorStyles.label);
            if (hasButton)
            {
                if (GUILayout.Button(buttonText))
                    buttonAction?.Invoke();
            }
            NewGUILine();
        }

        private bool VerifyDynamicBoneDirectory(string dir) => Directory.Exists(Path.Combine(dir, "Scripts"));

        private void CheckForIssues()
        {
            if (!string.IsNullOrEmpty(AssemblyInfo.DynamicBoneLocation) &&
                VerifyDynamicBoneDirectory(AssemblyInfo.DynamicBoneLocation))
            {
                AssemblyInfo.DynamicBoneLocation = "Assets/DynamicBone";
                if (GUILayout.Button("Build DynamicBone to Assembly"))
                    AssemblyInfo.BuildDynamicBoneToAssembly();
            }
            else
                DrawWarning("DynamicBone is not Imported!",
                    "Please Import DynamicBone from the Unity Asset Store", true,
                    "Go to DynamicBone",
                    () => Process.Start("https://assetstore.unity.com/packages/tools/animation/dynamic-bone-16743"));
        }

        private Vector2 scrollPosition;
        private void DrawAbout()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            GUILayout.Label("What is this?", EditorStyles.largeLabel);
            GUILayout.Label($"<color={EditorInfo.textcolor}>This is a tool to export the DynamicBone scripts to an assembly that can be loaded " +
                            "at runtime</color>", new GUIStyle
            {
                wordWrap = true,
                richText = true
            });
            NewGUILine();
            GUILayout.Label("Why would you want to do this?", EditorStyles.largeLabel);
            GUILayout.Label($"<color={EditorInfo.textcolor}>A lot of popular models/bases made in similar games make use of the DynamicBone " +
                            "scripts to add more realism to their models \n" +
                            "However, since this is a mod, rather than a built-in solution, and SCP Labrat doesn't" +
                            "contain DynamicBone in its files, it must be built and loaded manually. \n" +
                            "Unfortunately, this does require every content-creator themselves to purchase DynamicBone" +
                            ", it allows users to easily load DynamicBone into the game.</color>", new GUIStyle
            {
                wordWrap = true,
                richText = true
            });
            NewGUILine();
            GUILayout.Label("How do I use this?", EditorStyles.largeLabel);
            GUILayout.Label($"<color={EditorInfo.textcolor}>Here's a full step guide on how to use this tool: \n" +
                            "Step 1) Purchase and Import DynamicBone from the Unity Asset Store \n" +
                            "Step 2) Specify the root directory for DynamicBone (Example: Assets/DynamicBone) \n" +
                            "Step 3) Hit \"Build DynamicBone to Assembly\" \n" +
                            "Step 4) Copy the output file to your CustomLabratNPC/Libraries folder</color>", new GUIStyle
            {
                wordWrap = true,
                richText = true
            });
            NewGUILine();
            GUILayout.Label("Where's my CustomLabratNPC/Libraries folder?", EditorStyles.largeLabel);
            GUILayout.Label($"<color={EditorInfo.textcolor}>After installing the mod, you can find the CustomLabratNPC/Libraries folder" +
                            "in the root directory for CustomLabratNPC, which is inside the root directory of the game.</color>", 
                new GUIStyle
            {
                wordWrap = true,
                richText = true
            });
            NewGUILine();
            EditorGUILayout.EndScrollView();
        }

        private void OnGUI()
        {
            if (window != null)
            {
                EditorActions.DrawHeader(new Vector2(window.position.x, window.position.y),
                    new Vector2(window.position.width, window.position.height));
                NewGUILine();

                AssemblyInfo.DynamicBoneLocation =
                    EditorGUILayout.TextField("DynamicBone Location", AssemblyInfo.DynamicBoneLocation);
                if (GUILayout.Button("Set DynamicBone Location"))
                {
                    AssemblyInfo.DynamicBoneLocation = EditorActions.SetOutputDialog("Select DynamicBone Folder");
                }
                NewGUILine();
            
                CheckForIssues();
                NewGUILine();
                DrawAbout();
                GUILayout.EndArea();
            }
        }
    }
}