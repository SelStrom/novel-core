using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace NovelCore.Editor.Tools.Generators
{
    /// <summary>
    /// Editor tool for generating the DialogueBox UI prefab programmatically.
    /// Usage: Menu → NovelCore → Generate UI Prefabs → Dialogue Box
    /// </summary>
    public static class DialogueBoxPrefabGenerator
    {
        private const string PREFAB_PATH = "Assets/Resources/NovelCore/UI/DialogueBox.prefab";
        private const string RESOURCES_DIR = "Assets/Resources/NovelCore/UI";

        [MenuItem("NovelCore/Generate UI Prefabs/Dialogue Box")]
        public static void GenerateDialogueBoxPrefab()
        {
            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH) != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "DialogueBox Prefab Already Exists",
                    $"A prefab already exists at {PREFAB_PATH}.\n\nDo you want to overwrite it?",
                    "Overwrite",
                    "Cancel"
                );

                if (!overwrite)
                {
                    Debug.Log("[DialogueBoxGenerator] Generation cancelled by user.");
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
            GameObject dialogueBox = CreateDialogueBoxHierarchy();

            // Save as prefab
            bool success;
            PrefabUtility.SaveAsPrefabAsset(dialogueBox, PREFAB_PATH, out success);

            if (success)
            {
                Debug.Log($"[DialogueBoxGenerator] ✅ Successfully created DialogueBox prefab at {PREFAB_PATH}");
                EditorUtility.DisplayDialog(
                    "Dialogue Box Prefab Created",
                    $"DialogueBox prefab has been successfully created at:\n{PREFAB_PATH}\n\nYou can now use it in your scenes.",
                    "OK"
                );

                // Select the prefab in Project window
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            else
            {
                Debug.LogError($"[DialogueBoxGenerator] ❌ Failed to create DialogueBox prefab at {PREFAB_PATH}");
            }

            // Clean up scene GameObject
            Object.DestroyImmediate(dialogueBox);
        }

        private static GameObject CreateDialogueBoxHierarchy()
        {
            // Root GameObject with RectTransform
            GameObject dialogueBox = new GameObject("DialogueBox");
            RectTransform rootRect = dialogueBox.AddComponent<RectTransform>();

            // Configure root RectTransform - anchored to bottom of screen
            rootRect.anchorMin = new Vector2(0f, 0f);
            rootRect.anchorMax = new Vector2(1f, 0f);
            rootRect.pivot = new Vector2(0.5f, 0f);
            rootRect.anchoredPosition = Vector2.zero;
            rootRect.sizeDelta = new Vector2(0f, 200f); // Full width, 200px height

            // Add CanvasGroup for fade in/out control
            CanvasGroup canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            // Create Background Panel
            GameObject background = CreateBackgroundPanel(dialogueBox.transform);

            // Create SpeakerName Text
            GameObject speakerName = CreateSpeakerNameText(dialogueBox.transform);

            // Create DialogueText
            GameObject dialogueText = CreateDialogueText(dialogueBox.transform);

            // Create ContinueIndicator
            GameObject continueIndicator = CreateContinueIndicator(dialogueBox.transform);

            // Add DialogueBoxController component and assign references
            var controller = dialogueBox.AddComponent<NovelCore.Runtime.UI.DialogueBox.DialogueBoxController>();
            
            // Use reflection to set private fields (SerializeField fields are private)
            var controllerType = controller.GetType();
            
            var dialogueTextField = controllerType.GetField("_dialogueText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var speakerNameField = controllerType.GetField("_speakerNameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dialoguePanelField = controllerType.GetField("_dialoguePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var continueIndicatorField = controllerType.GetField("_continueIndicator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (dialogueTextField != null)
                dialogueTextField.SetValue(controller, dialogueText.GetComponent<TextMeshProUGUI>());
            
            if (speakerNameField != null)
                speakerNameField.SetValue(controller, speakerName.GetComponent<TextMeshProUGUI>());
            
            if (dialoguePanelField != null)
                dialoguePanelField.SetValue(controller, background);
            
            if (continueIndicatorField != null)
                continueIndicatorField.SetValue(controller, continueIndicator);

            return dialogueBox;
        }

        private static GameObject CreateBackgroundPanel(Transform parent)
        {
            GameObject background = new GameObject("Background");
            background.transform.SetParent(parent, false);

            RectTransform rect = background.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image image = background.AddComponent<Image>();
            image.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark semi-transparent background
            image.raycastTarget = true;

            return background;
        }

        private static GameObject CreateSpeakerNameText(Transform parent)
        {
            GameObject speakerName = new GameObject("SpeakerName");
            speakerName.transform.SetParent(parent, false);

            RectTransform rect = speakerName.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -10f); // 20px from left, 10px from top
            rect.sizeDelta = new Vector2(300f, 40f); // 300px wide, 40px tall

            TextMeshProUGUI text = speakerName.AddComponent<TextMeshProUGUI>();
            text.text = "Speaker Name";
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Left;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Truncate;

            return speakerName;
        }

        private static GameObject CreateDialogueText(Transform parent)
        {
            GameObject dialogueText = new GameObject("DialogueText");
            dialogueText.transform.SetParent(parent, false);

            RectTransform rect = dialogueText.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.offsetMin = new Vector2(20f, 60f); // 20px left, 60px bottom padding
            rect.offsetMax = new Vector2(-20f, -60f); // 20px right, 60px top padding

            TextMeshProUGUI text = dialogueText.AddComponent<TextMeshProUGUI>();
            text.text = "Dialogue text appears here. Click or tap to continue.";
            text.fontSize = 20;
            text.fontStyle = FontStyles.Normal;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Page;

            return dialogueText;
        }

        private static GameObject CreateContinueIndicator(Transform parent)
        {
            GameObject indicator = new GameObject("ContinueIndicator");
            indicator.transform.SetParent(parent, false);

            RectTransform rect = indicator.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-20f, 20f); // 20px from right, 20px from bottom
            rect.sizeDelta = new Vector2(30f, 30f); // 30x30px

            TextMeshProUGUI text = indicator.AddComponent<TextMeshProUGUI>();
            text.text = "▼"; // Down arrow indicator
            text.fontSize = 24;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            // Add Animation component placeholder (user can add Animation later)
            indicator.AddComponent<Animation>();

            return indicator;
        }
    }
}
