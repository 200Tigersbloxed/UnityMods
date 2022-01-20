using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CustomLabratNPC
{
    public class InvalidBuildWindow : EditorWindow
    {
        private static InvalidBuildWindow window;
        
        private static void ShowWindow()
        {
            window = GetWindow<InvalidBuildWindow>();
            window.titleContent = new GUIContent("Info");
        }

        [InitializeOnLoadMethod]
        private static void Check()
        {
            string assemblyName = Path.GetFileName(Assembly.GetAssembly(typeof(CustomNPCDescriptor)).Location);
            if (assemblyName != "CustomLabratNPC_Editor.dll")
            {
                ShowWindow();
                window.Show();
            }
        }

        private void OnGUI()
        {
            if (window != null)
            {
                string assemblyName = Path.GetFileName(Assembly.GetAssembly(typeof(CustomNPCDescriptor)).Location);
                EditorActions.DrawHeader(new Vector2(window.position.x, window.position.y),
                    new Vector2(window.position.width, window.position.height));
                GUILayout.Label("THE ASSEMBLY/FILE NAME IS INCORRECT!", EditorStyles.largeLabel);
                GUILayout.Label("This will cause numerous issues and prevent you from \n" +
                                "using the SDK! Please make sure the file name is: \n" +
                                "CustomLabratNPC_Editor.dll");
                GUILayout.Label("Current File Name: " + assemblyName);
                GUILayout.EndArea();
            }
        }
    }
}