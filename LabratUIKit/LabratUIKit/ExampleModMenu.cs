using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using ModMenu = LabratUIKit.UIKitAPI.ModMenu;
using ModContent = LabratUIKit.UIKitAPI.ModContent;

namespace LabratUIKit
{
    public static class ExampleModMenu
    {
        // First, create any ModContents you need
        private class MelonLoaderInfoMenu : ModContent
        {
            public override UIKitAPI.ModContentType ModContentType { get; set; } = UIKitAPI.ModContentType.Invoke;
            public override string Title { get; set; }
            public override string Description { get; set; }
            
            private Texture2D _image;
            public override Texture2D Image => _image;

            public override bool HideInvokeButton => true;

            public override void OnBeingCreated()
            {
                // images
                _image = MainMod.melonloaderImage;
                // text
                Title = $"Melonloader v{BuildInfo.Version}";
                Description = BuildInfo.Company;
            }
        }
        
        private class ModListerMenu : ModContent
        {
            public override UIKitAPI.ModContentType ModContentType { get; set; } = UIKitAPI.ModContentType.Invoke;
            public override string Title { get; set; } = "Loaded Mods";
            public override string Description { get; set; } = "Please Wait...";

            private static Texture2D _image;
            public override Texture2D Image => _image;

            public override bool HideInvokeButton => true;
            public override bool ExtendModContentText => true;

            public override void OnBeingCreated()
            {
                // images
                _image = MainMod.modificationImage;
                // text
                string mods = String.Empty;
                for (int i = 0; i < MelonHandler.Mods.Count; i++)
                {
                    MelonMod melonMod = MelonHandler.Mods[i];
                    string name = melonMod.Assembly.GetName().Name;
                    if (i == 0)
                        mods = $"{name}";
                    else if (i == MelonHandler.Mods.Count - 1)
                        mods = $"{mods}, {name}";
                    else
                        mods = $"{mods}, {name}";
                }
                Description = mods;
            }
        }

        // Now create your ModMenu
        public static void CreateModMenu()
        {
            // Strings can be whatever, semver is not required
            ModMenu modMenu = new ModMenu("LabratUIKit", "v1.0.0", "200Tigersbloxed");
            // Create a new ModContent list and add your ModContent(s) to it
            modMenu.modContent = new List<ModContent>();
            modMenu.modContent.Add(new MelonLoaderInfoMenu());
            modMenu.modContent.Add(new ModListerMenu());
            // Push the ModMenu
            UIKitAPI.AddModToMenu(modMenu);
        }
    }
}