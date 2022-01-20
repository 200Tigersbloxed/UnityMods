using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LabratUIKit;
using LabratUIKit.images;
using UnityEngine;

namespace CustomLabratNPC.ModSupport
{
    public static class LabratUIKitHelper
    {
        private static string GetNPCConfigIdentifier(string creator, int NPCType, string NPCName) =>
            $"{creator}-{NPCType}-{NPCName}";

        /// <summary>
        /// Parses the NPCConfigIdentifier
        /// Item1 is Creator
        /// Item2 is NPCType
        /// Item3 is NPCName
        /// </summary>
        /// <param name="identifier">The Config Value</param>
        /// <returns></returns>
        public static (string, NPCEnum, string) ParseNPCConfigIdentifier(string identifier)
        {
            string creator;
            NPCEnum NPCType;
            string NPCName;
            string[] indentities = identifier.Split('-');
            creator = indentities[0];
            NPCType = (NPCEnum)Convert.ToInt32(indentities[1]);
            NPCName = indentities[2];
            return (creator, NPCType, NPCName);
        }

        private static string npci_desc;
        private class NPCInfo : UIKitAPI.ModContent
        {
            public override UIKitAPI.ModContentType ModContentType { get; set; } = UIKitAPI.ModContentType.Invoke;
            public override string Title { get; set; } = "Selected SCPs";
            public override string Description
            {
                get
                {
                    return npci_desc;
                }
                set { }
            }

            public override bool HideInvokeButton => true;
            public override bool ExtendModContentText => true;

            public static void UpdateSelectedSCPs()
            {
                CustomNPCDescriptor npc173_selected = NPCLoader.SelectedNPC[NPCEnum.SCP173];
                string npc173_name;
                if (npc173_selected != null)
                    npc173_name = npc173_selected.NPCDisplayName ?? npc173_selected.gameObject.name;
                else
                    npc173_name = "random";
                npci_desc = $"SCP-173: {npc173_name}";
                npcinfo.RequestTextChange();
            }

            public static void RequestButtonChanges()
            {
                LogHelper.Debug("Requested ButtonChanges");
                foreach (NPCUI npcui in _npcuis)
                {
                    npcui.UpdateButtonText();
                }
            }

            public override void OnBeingCreated()
            {
                UpdateSelectedSCPs();
                RequestButtonChanges();
            }
        }

        private static Texture2D cached_nonpcicon;
        
        private class NPCUI : UIKitAPI.ModContent
        {
            public CustomNPCDescriptor Descriptor;
            public ConfigHelper.ConfigValue<string> TargetConfig;

            public override UIKitAPI.ModContentType ModContentType { get; set; }
            public override string Title { get; set; }
            public override string Description { get; set; }

            public override void OnModContentButtonPressed()
            {
                // update config
                CustomNPCDescriptor npcDescriptor = NPCLoader.SelectedNPC[Descriptor.NPCType];
                if (npcDescriptor == null || (npcDescriptor != null && npcDescriptor != Descriptor))
                {
                    NPCLoader.SelectedNPC[Descriptor.NPCType] = NPCLoader.FindLoadedNPCByName(Descriptor.name);
                    TargetConfig.SetValue(GetNPCConfigIdentifier(Descriptor.Creator,
                        (int)Descriptor.NPCType, Descriptor.gameObject.name));
                }
                else if(npcDescriptor == Descriptor)
                {
                    NPCLoader.SelectedNPC[Descriptor.NPCType] = null;
                    TargetConfig.SetValue("random");
                }
                NPCInfo.RequestButtonChanges();
                NPCInfo.UpdateSelectedSCPs();
                RequestTextChange();
            }

            public void UpdateButtonText()
            {
                // button
                CustomNPCDescriptor selectedscp = NPCLoader.SelectedNPC[Descriptor.NPCType];
                if(selectedscp == null)
                    InvokeButtonText = $"Set as {Descriptor.NPCType.ToString()}";
                else if(selectedscp == Descriptor)
                    InvokeButtonText = "Set Random " + Descriptor.NPCType;
                else
                    InvokeButtonText = $"Set as {Descriptor.NPCType.ToString()}";
            }

            public override string InvokeButtonText { get; set; }

            public Texture2D _image;

            public override Texture2D Image
            {
                get
                {
                    return _image;
                }
                set
                {
                    _image = value;
                }
            }

            public override void OnBeingCreated()
            {
                // Image
                try
                {
                    FieldInfo image_field = Descriptor.GetType()
                        .GetField("NPCIcon", BindingFlags.Public | BindingFlags.Instance);
                    if (image_field != null)
                    {
                        object image_ = image_field.GetValue(Descriptor);
                        if (image_ != null)
                            _image = (Texture2D) Convert.ChangeType(image_, typeof(Texture2D));
                        else
                            LogHelper.Warn($"-NPC {Descriptor.NPCDisplayName} does not contain an icon!");
                    }
                    else
                        LogHelper.Warn($"| NPC {Descriptor.NPCDisplayName} does not contain an icon!");
                }
                catch (Exception e)
                {
                    LogHelper.Error($"NPC {Descriptor.NPCDisplayName} does not contain an icon!", e);
                }
                if (_image == null)
                {
                    if (cached_nonpcicon != null)
                        _image = cached_nonpcicon;
                    else
                    {
                        // Replace it with the default
                        using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomLabratNPC.unknown.png"))
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (stream != null)
                            {
                                stream.CopyTo(ms);
                                Texture2D texture = new Texture2DConvert(ms.ToArray()).ToTexture2D();
                                cached_nonpcicon = texture;
                                _image = texture;
                            }
                        }
                    }
                }
            }
        }

        private static ConfigHelper.ConfigValue<string> FindTargetedNPCSave(NPCEnum npcType)
        {
            switch (npcType)
            {
                case NPCEnum.SCP173:
                    return ConfigHelper.LoadedConfig.selectedNPC173;
            }
            return null;
        }

        private static NPCInfo npcinfo;
        private static readonly List<NPCUI> _npcuis = new List<NPCUI>();

        /// <summary>
        /// Sets up the LabratUIKit.
        /// Must come AFTER Config Loading and NPC Loading
        /// </summary>
        /// <param name="loadedNPCs"></param>
        public static void SetupLabratUI(List<CustomNPCDescriptor> loadedNPCs)
        {
            UIKitAPI.ModMenu modMenu = new UIKitAPI.ModMenu("CustomLabratNPC", "v0.2.0-beta", "200Tigersbloxed");
            modMenu.modContent = new List<UIKitAPI.ModContent>();
            npcinfo = new NPCInfo();
            modMenu.modContent.Add(npcinfo);
            foreach (CustomNPCDescriptor customNpcDescriptor in loadedNPCs)
            {
                NPCUI npcui = new NPCUI
                {
                    Descriptor = customNpcDescriptor,
                    TargetConfig = FindTargetedNPCSave(customNpcDescriptor.NPCType),
                    ModContentType = UIKitAPI.ModContentType.Invoke,
                    Title = $"{customNpcDescriptor.NPCDisplayName} | ({customNpcDescriptor.gameObject.name})",
                    Description = $"NPC: {customNpcDescriptor.NPCType} | Created By: {customNpcDescriptor.Creator}",
                    HideInvokeButton = false
                };
                modMenu.modContent.Add(npcui);
                _npcuis.Add(npcui);
            }
            UIKitAPI.AddModToMenu(modMenu);
        }
    }
}