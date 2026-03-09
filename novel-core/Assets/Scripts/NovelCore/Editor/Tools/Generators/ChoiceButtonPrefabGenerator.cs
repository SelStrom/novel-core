using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace NovelCore.Editor.Tools.Generators
{
    /// <summary>
    /// Editor tool for generating the ChoiceButton UI prefab programmatically.
    /// Usage: Menu → NovelCore → Generate UI Prefabs → Choice Button
    /// </summary>
    public static class ChoiceButtonPrefabGenerator
    {
        private const string PREFAB_PATH = "Assets/Resources/NovelCore/UI/ChoiceButton.prefab";
        private const string RESOURCES_DIR = "Assets/Resources/NovelCore/UI";

        [MenuItem("NovelCore/Generate UI Prefabs/Choice Button")]
        public static void GenerateChoiceButtonPrefab()
        {
            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH) != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "ChoiceButton Prefab Already Exists",
                    $"A prefab already exists at {PREFAB_PATH}.\n\nDo you want to overwrite it?",
                    "Overwrite",
                    "Cancel"
                );

                if (!overwrite)
                {
                    Debug.Log("[ChoiceButtonGenerator] Generation cancelled by user.");
                    return;
                }

                AssetDatabase.DeleteAsset(PREFAB_PATH);
            }

            // Ensure directory exists
            if (!Directory.Exists(RESOURCES_DIR))
            {
                Directory.CreateDirectory(RESOURCES_DIR);
                AssetDatabase.Refresh();
            }

            // Create GameObject hierarchy
            GameObject choiceButton = CreateChoiceButtonHierarchy();

            // Save as prefab
            bool success;
            PrefabUtility.SaveAsPrefabAsset(choiceButton, PREFAB_PATH, out success);

            if (success)
            {
                Debug.Log($"[ChoiceButtonGenerator] ✅ Successfully created ChoiceButton prefab at {PREFAB_PATH}");
                EditorUtility.DisplayDialog(
                    "Choice Button Prefab Created",
                    $"ChoiceButton prefab has been successfully created at:\n{PREFAB_PATH}\n\nYou can now use it in your scenes.",
                    "OK"
                );

                // Select the prefab in Project window
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            else
            {
                Debug.LogError($"[ChoiceButtonGenerator] ❌ Failed to create ChoiceButton prefab at {PREFAB_PATH}");
            }

            // Clean up scene GameObject
            Object.DestroyImmediate(choiceButton);
        }

        private static GameObject CreateChoiceButtonHierarchy()
        {
            // Root GameObject with Button component
            GameObject choiceButton = new GameObject("ChoiceButton");
            RectTransform rootRect = choiceButton.AddComponent<RectTransform>();

            // Configure root RectTransform - centered, 600px wide, 60px tall
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Vector2.zero;
            rootRect.sizeDelta = new Vector2(600f, 60f);

            // Add Image component for background
            Image buttonImage = choiceButton.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.3f, 1f); // Dark blue-ish background
            buttonImage.raycastTarget = true;

            // Add Button component
            Button button = choiceButton.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            // Configure button colors
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.3f, 1f);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.4f, 1f);
            colors.pressedColor = new Color(0.15f, 0.15f, 0.25f, 1f);
            colors.selectedColor = new Color(0.25f, 0.25f, 0.35f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            button.colors = colors;

            // Add ChoiceUIController component (will be added when implementing choice system)
            // choiceButton.AddComponent<NovelCore.Runtime.UI.ChoiceButtons.ChoiceUIController>();

            // Create Text child
            GameObject textObject = CreateChoiceText(choiceButton.transform);

            return choiceButton;
        }

        private static GameObject CreateChoiceText(Transform parent)
        {
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = new Vector2(20f, 10f); // 20px left, 10px bottom padding
            rect.offsetMax = new Vector2(-20f, -10f); // 20px right, 10px top padding

            TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = "Choice option text";
            text.fontSize = 18;
            text.fontStyle = FontStyles.Normal;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Truncate;

            return textObject;
        }
    }
}
