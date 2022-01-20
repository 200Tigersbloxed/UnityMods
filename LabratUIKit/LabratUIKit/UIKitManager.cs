using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LabratUIKit
{
    public static class UIKitManager
    {
        private static readonly List<UIKitAPI.ModMenu> ModMenus = new List<UIKitAPI.ModMenu>();
        public static bool IsModListVisible { get; private set; }

        public static void AddModMenu(UIKitAPI.ModMenu modMenu) => ModMenus.Add(modMenu);
        public static void RemoveModMenu(UIKitAPI.ModMenu modMenu) => ModMenus.Remove(modMenu);

        private static Transform ModContent;
        
        public static void CreateModList(GameObject mainModel)
        {
            //mainModel.GetComponent<Canvas>().worldCamera = Camera.current;
            mainModel.transform.position = new Vector3(-6, 5, 1.5f);
            mainModel.transform.localRotation *= Quaternion.AngleAxis(-36, Vector3.up);
            bool didDupeFirstButton = false;
            int currentButton = 0;
            GameObject ModList_Content =
                mainModel.transform.GetChild(0).Find("ModList").Find("Viewport").GetChild(0).gameObject;
            foreach (UIKitAPI.ModMenu modMenu in ModMenus)
            {
                currentButton++;
                if (!didDupeFirstButton)
                {
                    // Overwrite the first button
                    GameObject targetButton = ModList_Content.transform.GetChild(0).gameObject;
                    targetButton.GetComponent<Button>().onClick.AddListener(() => ShowModMenu(mainModel, modMenu));
                    targetButton.transform.GetChild(0).GetComponent<Text>().text = modMenu.ModName;
                    didDupeFirstButton = true;
                }
                else
                {
                    // Create a new button
                    GameObject targetButton = Object.Instantiate(ModList_Content.transform.GetChild(0).gameObject,
                        ModList_Content.transform);
                    targetButton.GetComponent<Button>().onClick.AddListener(() => ShowModMenu(mainModel, modMenu));
                    targetButton.transform.GetChild(0).GetComponent<Text>().text = modMenu.ModName;
                    RectTransform buttonRect = targetButton.GetComponent<RectTransform>();
                    buttonRect.localPosition = new Vector3(buttonRect.localPosition.x,
                        buttonRect.localPosition.y - (buttonRect.rect.height * currentButton),
                        buttonRect.localPosition.z);
                }
            }
            mainModel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                IsModListVisible = false;
                Object.Destroy(mainModel);
            });
            // Scroll Buttons
            mainModel.transform.GetChild(0).Find("ModList").Find("Up").gameObject
                .AddComponent<MonoHelpers.ScrollButton>().MoveAmount = 1f;
            mainModel.transform.GetChild(0).Find("ModList").Find("Down").gameObject
                .AddComponent<MonoHelpers.ScrollButton>().MoveAmount = 1f;
            mainModel.transform.GetChild(0).Find("ModContent").Find("Up").gameObject
                .AddComponent<MonoHelpers.ScrollButton>().MoveAmount = 1f;
            mainModel.transform.GetChild(0).Find("ModContent").Find("Down").gameObject
                .AddComponent<MonoHelpers.ScrollButton>().MoveAmount = 1f;
            // Hide values
            mainModel.transform.GetChild(0).Find("ModContent").Find("Viewport").Find("Content").Find("ModInfo")
                .GetChild(0).GetComponent<Text>().text = "Please select a mod from the list to the left";
            ModContent = mainModel.transform.GetChild(0).Find("ModContent").Find("Viewport").Find("Content")
                .Find("ModContent");
            ModContent.gameObject.SetActive(false);
            // Done
            IsModListVisible = true;
        }
        
        private static void CreateModContent(GameObject ModContent, UIKitAPI.ModContent modContent, int ypos)
        {
            LogHelper.Debug("Creating ModContent for " + modContent.Title);
            // Work on the template
            ModContent.transform.GetChild(0).GetComponent<Text>().text = $"{modContent.Title} \n" +
                $"{modContent.Description}";
            GameObject ModValue = ModContent.transform.GetChild(2).gameObject;
            GameObject NotModValue = ModContent.transform.GetChild(3).gameObject;
            switch (modContent.ModContentType)
            {
                case UIKitAPI.ModContentType.Value:
                    ModValue.SetActive(true);
                    MonoHelpers.ModValueDriver driver = ModValue.AddComponent<MonoHelpers.ModValueDriver>();
                    driver.targetText = ModValue.transform.GetChild(0).GetComponent<Text>();
                    driver.ModContent = modContent;
                    ModValue.transform.GetChild(1).GetComponent<Button>().onClick
                        .AddListener(modContent.OnModContentButtonPressed);
                    NotModValue.SetActive(false);
                    break;
                case UIKitAPI.ModContentType.Invoke:
                    NotModValue.SetActive(true);
                    NotModValue.transform.GetChild(0).GetComponent<Button>().onClick
                        .AddListener(modContent.OnModContentButtonPressed);
                    NotModValue.transform.GetChild(0).GetChild(0).GetComponent<Text>().text =
                        modContent.InvokeButtonText;
                    NotModValue.transform.GetChild(0).gameObject.SetActive(!modContent.HideInvokeButton);
                    if (modContent.HideInvokeButton && modContent.ExtendModContentText)
                    {
                        Transform ModContentText = ModContent.transform.GetChild(0);
                        RectTransform ModContentTextRect = ModContentText.GetComponent<RectTransform>();
                        if (ModContentTextRect.sizeDelta.y != 30)
                        {
                            ModContentTextRect.sizeDelta = new Vector2(173.7582f, 30);
                            ModContentText.localPosition = new Vector3(ModContentText.localPosition.x,
                                ModContentText.localPosition.y - 7, ModContentText.localPosition.z);
                        }
                    }
                    else
                    {
                        // It's the template, we can reset it's scale and position
                        Transform ModContentText = ModContent.transform.GetChild(0);
                        RectTransform ModContentTextRect = ModContentText.GetComponent<RectTransform>();
                        if (ModContentTextRect.sizeDelta.y != 15)
                        {
                            ModContentTextRect.sizeDelta = new Vector2(173.7582f, 15);
                            ModContentText.localPosition = new Vector3(ModContentText.localPosition.x,
                                ModContentText.localPosition.y + 7, ModContentText.localPosition.z);
                        }
                    }
                    ModValue.SetActive(false);
                    break;
                default:
                    LogHelper.Warn("Unknown ModType " + modContent.ModContentType);
                    break;
            }

            if (modContent.Image != null)
            {
                Texture2D t2d = modContent.Image;
                ModContent.transform.GetChild(1).GetComponent<Image>().sprite =
                    Sprite.Create(t2d, new Rect(0.0f, 0.0f, t2d.width, t2d.height), 
                        new Vector2(0.5f, 0.5f), 100.0f);
            }
            else
                ModContent.transform.GetChild(1).GetComponent<Image>().sprite =
                    Sprite.Create(new Texture2D(2, 2), new Rect(0.0f, 0.0f, 2, 2), 
                        new Vector2(0.5f, 0.5f), 100.0f);

            RectTransform ModContentRectTransform = ModContent.GetComponent<RectTransform>();
            ModContent.transform.localPosition = new Vector3(ModContent.transform.localPosition.x,
                ModContent.transform.localPosition.y + ModContentRectTransform.rect.height * ((ypos - 2) * -1),
                ModContent.transform.localPosition.z);
            modContent.IsModContentVisible = true;
            modContent.ModContent_Container = ModContent;
            LogHelper.Debug("Finished Creating ModContent for " + modContent.Title);
        }

        private static void ShowModMenu(GameObject mainModel, UIKitAPI.ModMenu modMenu)
        {
            ModContent.gameObject.SetActive(true);
            GameObject ModContent_Content =
                mainModel.transform.GetChild(0).Find("ModContent").Find("Viewport").GetChild(0).gameObject;
            // Clear old Children
            for (int i = 0; i < ModContent_Content.transform.childCount; i++)
            {
                GameObject currentChild = ModContent_Content.transform.GetChild(i).gameObject;
                if(currentChild.name != "ModInfo" && currentChild.name != "ModContent")
                    Object.Destroy(currentChild);
            }
            // Setup Header
            GameObject header = ModContent_Content.transform.GetChild(0).gameObject;
            header.transform.GetChild(0).gameObject.GetComponent<Text>().text = $"{modMenu.ModName} \n" +
                $"{modMenu.ModVersion} \n" +
                $"{modMenu.ModCreator} \n";
            // Create new Children
            int currenty = 1;
            foreach (UIKitAPI.ModContent modContent in modMenu.modContent)
            {
                modContent.IsModContentVisible = false;
                modContent.ModContent_Container = null;
                currenty++;
                modContent.OnBeingCreated();
                GameObject correctModContent;
                if (currenty > 2)
                    correctModContent = Object.Instantiate(ModContent_Content.transform.GetChild(1).gameObject,
                        ModContent_Content.transform);
                else
                    correctModContent = ModContent_Content.transform.GetChild(1).gameObject;
                CreateModContent(correctModContent, modContent, currenty);
            }
        }

        public static void UpdateUIText(GameObject ModContent, UIKitAPI.ModContent modContent)
        {
            ModContent.transform.GetChild(0).GetComponent<Text>().text = $"{modContent.Title} \n" +
                                                                         $"{modContent.Description}";
            GameObject NotModValue = ModContent.transform.GetChild(3).gameObject;
            switch (modContent.ModContentType)
            {
                case UIKitAPI.ModContentType.Invoke:
                    NotModValue.transform.GetChild(0).GetChild(0).GetComponent<Text>().text =
                        modContent.InvokeButtonText;
                    break;
            }
        }
        
        [CanBeNull]
        public static Button ResetButton(GameObject buttonContainer, ColorBlock colors,
            UnityEngine.Events.UnityAction listenerCallback = null)
        {
            GameObject menu = SceneManager.GetActiveScene().GetRootGameObjects()
                .FirstOrDefault(x => x.name == "Menu")?.gameObject;
            if (menu != null)
            {
                Object.DestroyImmediate(buttonContainer.GetComponent<Button>());
                Button newButton = buttonContainer.AddComponent<Button>();
                // assume it's a normal button
                newButton.colors = colors;
                if (listenerCallback != null) { newButton.onClick.AddListener(listenerCallback); }
                newButton.gameObject.GetComponent<Image>().enabled = true;
                return newButton;
            }
            return null;
        }
    }
}