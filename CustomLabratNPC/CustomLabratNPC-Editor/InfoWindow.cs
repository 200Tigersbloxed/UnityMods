using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace CustomLabratNPC
{
    public class InfoWindow : EditorWindow
    {
        private static InfoWindow window;
        
        [MenuItem("CustomLabratNPC/Info")]
        private static void ShowWindow()
        {
            window = GetWindow<InfoWindow>();
            window.titleContent = new GUIContent("Info");
        }
        
        private static void NewGUILine() => GUILayout.Label("", EditorStyles.largeLabel);

        [InitializeOnLoadMethod]
        static void ForceShowWindow()
        {
            if (FindObjectsOfType<CustomNPCDescriptor>().Length == 0)
            {
                ShowWindow();
                window.Show();
            }
        }

        private Vector2 scrollpos1;
        private void OnGUI()
        {
            if (window != null)
            {
                EditorActions.DrawHeader(new Vector2(window.position.x, window.position.y),
                    new Vector2(window.position.width, window.position.height));
                scrollpos1 = GUILayout.BeginScrollView(scrollpos1);
                NewGUILine();
                GUILayout.Label("My Info", EditorStyles.largeLabel);
                if (GUILayout.Button("GitHub"))
                    Process.Start("https://github.com/200Tigersbloxed/UnityMods");
                if (GUILayout.Button("Tiger's Contraptions Discord"))
                    Process.Start("https://fortnite.lol/discord");
                if (GUILayout.Button("Main Website"))
                    Process.Start("https://fortnite.lol");
                NewGUILine();
                GUILayout.Label("Help and Info", EditorStyles.largeLabel);
                if (GUILayout.Button("Docs"))
                    Process.Start("https://github.com/200Tigersbloxed/UnityMods/tree/main/CustomLabratNPC/docs");
                if (GUILayout.Button("Issues"))
                    Process.Start("https://github.com/200Tigersbloxed/UnityMods/issues");
                if (GUILayout.Button("Discussions"))
                    Process.Start("https://github.com/200Tigersbloxed/UnityMods/discussions");
                GUILayout.Label("or you could use the Tiger's Contraptions discord...", EditorStyles.miniLabel);
                NewGUILine();
                GUILayout.Label("Bezbro Games", EditorStyles.largeLabel);
                if (GUILayout.Button("Bezbro Games Discord"))
                    Process.Start("https://discord.gg/WVtw46ztfR");
                GUILayout.Label($"<color={EditorInfo.textcolor}>PLEASE DO NOT ASK FOR SUPPORT IN THE BEZBRO GAMES DISCORD! \n" +
                                "USE THE DISCUSSIONS OR DISCORD SERVER... (please)</color>", new GUIStyle
                {
                    wordWrap = true,
                    richText = true,
                    fontSize = 24
                });
                NewGUILine();
                GUILayout.Label("Useful Links", EditorStyles.largeLabel);
                if (GUILayout.Button("MelonLoader")) Process.Start("https://melonwiki.xyz/");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }
    }
}