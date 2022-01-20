using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace LabratUIKit
{
    public static class UIKitAPI
    {
        public class ModMenu
        {
            public readonly string ModName;
            public readonly string ModVersion;
            public readonly string ModCreator;
            public List<ModContent> modContent;

            public ModMenu(string ModName, string ModVersion, string ModCreator)
            {
                this.ModName = ModName;
                this.ModVersion = ModVersion;
                this.ModCreator = ModCreator;
            }
        }

        public abstract class ModContent
        {
            public abstract ModContentType ModContentType { get; set; }
            public abstract string Title { get; set; }
            public abstract string Description { get; set; }
            [CanBeNull] public virtual Texture2D Image { get; set; }

            public virtual void OnModContentButtonPressed(){}
            public virtual string ModValue { get; set; }
            public virtual bool HideInvokeButton { get; set; } = false;
            public virtual bool ExtendModContentText { get; set; } = false;
            public virtual string InvokeButtonText { get; set; } = "Invoke";

            public virtual void OnBeingCreated(){}

            public bool IsModContentVisible;
            public GameObject ModContent_Container;
            public void RequestTextChange()
            {
                if(IsModContentVisible && ModContent_Container != null)
                    UIKitManager.UpdateUIText(ModContent_Container, this);
            }
        }

        public enum ModContentType
        {
            Invoke,
            Value
        }

        public static void AddModToMenu(ModMenu modToAdd) => UIKitManager.AddModMenu(modToAdd);

        /// <summary>
        /// The base for a Reflected ModContent Container
        /// This should not be used outside of reflection
        /// </summary>
        private class CreatedModContent : ModContent
        {
            public override ModContentType ModContentType { get; set; }
            public override string Title { get; set; }
            public override string Description { get; set; }
            public override Texture2D Image { get; set; }

            public Action OnModContentButtonPressed_Action = () => { };

            public override void OnModContentButtonPressed()
            {
                OnModContentButtonPressed_Action.Invoke();
            }
            public override string ModValue { get; set; }
            public override bool HideInvokeButton => false;
            public override bool ExtendModContentText => false;
            public override string InvokeButtonText => "Invoke";
            
            public Action OnBeingCreated_Action = () => { };

            public override void OnBeingCreated()
            {
                OnBeingCreated_Action.Invoke();
            }
        }

        /// <summary>
        /// Adds a Reflected ModMenu to the UIKitManager
        /// </summary>
        /// <param name="modmenu_reflected">The reflected ModMenu</param>
        public static void AddReflectedModToMenu(object modmenu_reflected)
        {
            ModMenu modMenu = (ModMenu) Convert.ChangeType(modmenu_reflected, typeof(ModMenu));
            UIKitManager.AddModMenu(modMenu);
        }
        
        /// <summary>
        /// Removes a Reflected ModMenu from the UIKitManager
        /// </summary>
        /// <param name="modmenu_reflected">The reflected ModMenu</param>
        public static void RemoveReflectedModToMenu(object modmenu_reflected)
        {
            ModMenu modMenu = (ModMenu) Convert.ChangeType(modmenu_reflected, typeof(ModMenu));
            UIKitManager.RemoveModMenu(modMenu);
        }

        /// <summary>
        /// Creates a reflected mod menu with ModContents. 
        /// This should be used in reflection only.
        /// </summary>
        /// <param name="modName">Name of the mod</param>
        /// <param name="modVersion">Version of the mod (does not have to follow semver)</param>
        /// <param name="modCreator">The creator of the mod</param>
        /// <param name="modContents_object">A list of reflected ModContent(s)</param>
        public static object CreateModMenu(string modName, string modVersion, string modCreator,
            List<object> modContents_object)
        {
            ModMenu modMenu = new ModMenu(modName, modVersion, modCreator);
            List<ModContent> modContents = new List<ModContent>();
            foreach (object o in modContents_object)
            {
                CreatedModContent createdModContent =
                    (CreatedModContent) Convert.ChangeType(o, typeof(CreatedModContent));
                modContents.Add(createdModContent);
            }
            AddModToMenu(modMenu);
            return modMenu;
        }
        
        /// <summary>
        /// Creates a new reflected ModContent.
        /// Should only be used with reflection!
        /// </summary>
        /// <param name="modContentType">The type of ModContent.
        /// 0 is Invoke, 1 is Value</param>
        /// <param name="title">Title of ModContent</param>
        /// <param name="description">Description of ModContent</param>
        /// <param name="image">A display image for the ModContent (not required)</param>
        /// <param name="onModContentButtonPressed">Invoked when the ModContent button is hit</param>
        /// <param name="GetModValue">Function to get the value of the ModValue (modContentType 1 only)</param>
        /// <param name="hideInvokeButton">Hides the Invoke button (modContentType 0 only)</param>
        /// <param name="extendModContentText">Extends the ModContentText
        /// (modContentType 0 and hideInvokeButton true only)</param>
        /// <param name="invokeButtonText">Text of the ModContentButton</param>
        /// <param name="onBeingCreated">Invokes before the ModContent is being created</param>
        /// <returns></returns>
        public static object CreateModContent(ModContentType modContentType, string title, string description,
            Texture2D image, Action onModContentButtonPressed = null, Func<string> GetModValue = null,
            bool hideInvokeButton = false, bool extendModContentText = false, string invokeButtonText = "Invoke",
            Action onBeingCreated = null)
        {
            CreatedModContent createdModContent = new CreatedModContent
            {
                ModContentType = modContentType,
                Title = title,
                Description = description,
                Image = image,
                ModValue = GetModValue?.Invoke(),
                HideInvokeButton = hideInvokeButton,
                ExtendModContentText = extendModContentText,
                InvokeButtonText = invokeButtonText
            };
            createdModContent.OnModContentButtonPressed_Action += () => onModContentButtonPressed?.Invoke();
            createdModContent.OnBeingCreated_Action += () => onBeingCreated?.Invoke();
            return createdModContent;
        }
    }
}