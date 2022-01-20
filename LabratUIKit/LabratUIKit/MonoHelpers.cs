using System;
using UnityEngine;
using UnityEngine.UI;

namespace LabratUIKit
{
    public static class MonoHelpers
    {
        public class ModValueDriver : MonoBehaviour
        {
            public UIKitAPI.ModContent ModContent;
            public Text targetText;

            private void Update()
            {
                if (ModContent != null && targetText != null)
                    targetText.text = ModContent.ModValue;
            }
        }

        [RequireComponent(typeof(Button))]
        public class ScrollButton : MonoBehaviour
        {
            public float MoveAmount;
            private RectTransform Content;
            private ScrollDirection ScrollDirection;
            private Button Button;

            private void AddToY(RectTransform rectTransform, float y)
            {
                Vector3 v3 = new Vector3(rectTransform.position.x, rectTransform.position.y, rectTransform.position.z);
                v3.y = v3.y + y;
                rectTransform.position = v3;
            }

            private void OnEnable()
            {
                Content = transform.parent.Find("Viewport").GetChild(0).GetComponent<RectTransform>();
                if (gameObject.name.Contains("Up"))
                    ScrollDirection = ScrollDirection.Up;
                else if (gameObject.name.Contains("Down"))
                    ScrollDirection = ScrollDirection.Down;
                Button = gameObject.GetComponent<Button>();
                Button.onClick.AddListener(() =>
                {
                    if (Content != null)
                    {
                        switch (ScrollDirection)
                        {
                            case ScrollDirection.Up:
                                AddToY(Content, MoveAmount * -1f);
                                break;
                            case ScrollDirection.Down:
                                AddToY(Content, MoveAmount);
                                break;
                            default:
                                LogHelper.Warn("No ScrollDirection found for " + gameObject.name);
                                break;
                        }
                    }
                    else
                        LogHelper.Warn("No Content found for " + gameObject.name);
                });
            }
        }

        private enum ScrollDirection
        {
            Unknown,
            Up,
            Down
        }
    }
}