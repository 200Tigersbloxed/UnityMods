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
    public static class ScriptInfo
    {
        public static string AssemblyName { get; set; }
        public static List<MonoScript> ScriptsToCompile = new List<MonoScript>();

        public static void CompileScripts(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                List<string> scripts = new List<string>();
                foreach (MonoScript monoScript in ScriptsToCompile)
                    scripts.Add(AssetDatabase.GetAssetPath(monoScript));
                AssemblyBuilder ab =
                    new AssemblyBuilder($"Assets/CustomLabratNPC/ScriptCompiler/{name}.dll", scripts.ToArray());
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
                        if (EditorUtility.DisplayDialog("CustomLabratNPC ScriptCompiler",
                                "Successfully Compiled Assembly", "Go to File", "OK"))
                            Process.Start(Path.GetFullPath("Assets/CustomLabratNPC/ScriptCompiler"));
                    }
                    else
                        EditorUtility.DisplayDialog("CustomLabratNPC ScriptCompiler",
                            "Failed to Compile Assembly! Check Console for more details", "OK");
                };
                ab.Build();
            }
            else
                Debug.LogWarning("AssemblyName cannot be null!");
        }
    }
    
    public class ScriptCompiler : EditorWindow
    {
        private static ScriptCompiler window;
        
        [MenuItem("CustomLabratNPC/ScriptCompiler")]
        private static void ShowWindow()
        {
            window = GetWindow<ScriptCompiler>();
            window.titleContent = new GUIContent("ScriptCompiler");
        }
        
        private static void NewGUILine() => GUILayout.Label("", EditorStyles.largeLabel);

        private Vector2 scrollPosition;
        private void DrawList()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            foreach (MonoScript monoScript in ScriptInfo.ScriptsToCompile)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(monoScript.name);
                if (GUILayout.Button("Remove"))
                    try
                    {
                        ScriptInfo.ScriptsToCompile.Remove(monoScript);
                    }
                    catch(Exception){}
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        
        private MonoScript currentScript;
        private void OnGUI()
        {
            if (window != null)
            {
                EditorActions.DrawHeader(new Vector2(window.position.x, window.position.y),
                    new Vector2(window.position.width, window.position.height));
                NewGUILine();
                GUILayout.Label("Drag and drop a Script below to compile it");
                currentScript = (MonoScript) EditorGUILayout.ObjectField(currentScript, typeof(MonoScript), false, null);
                if(GUILayout.Button("Add"))
                    ScriptInfo.ScriptsToCompile.Add(currentScript);
                NewGUILine();
                DrawList();
                NewGUILine();
                ScriptInfo.AssemblyName = EditorGUILayout.TextField("Assembly Name", ScriptInfo.AssemblyName);
                if(GUILayout.Button("Compile Scripts!"))
                    ScriptInfo.CompileScripts(ScriptInfo.AssemblyName);
                NewGUILine();
                GUILayout.EndArea();
            }
        }
    }
}