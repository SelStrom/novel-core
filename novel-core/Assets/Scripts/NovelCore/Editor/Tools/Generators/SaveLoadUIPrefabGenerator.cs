using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace NovelCore.Editor.Tools.Generators
{
    /// <summary>
    /// Editor tool for generating the SaveLoadUI prefab programmatically.
    /// Usage: Menu → NovelCore → Generate UI Prefabs → Save/Load UI
    /// </summary>
    public static class SaveLoadUIPrefabGenerator
    {
        private const string PREFAB_PATH = "Assets/Resources/NovelCore/UI/SaveLoadUI.prefab";
        private const string RESOURCES_DIR = "Assets/Resources/NovelCore/UI";

        [MenuItem("NovelCore/Generate UI Prefabs/Save Load UI")]
        public static void GenerateSaveLoadUIPrefab()
        {
            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH) != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "SaveLoadUI Prefab Already Exists",
                    $"A prefab already exists at {PREFAB_PATH}.\n\nDo you want to overwrite it?",
                    "Overwrite",
                    "Cancel"
                );

                if (!overwrite)
                {
                    Debug.Log("[SaveLoadUIGenerator] Generation cancelled by user.");
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
            GameObject saveLoadUI = CreateSaveLoadUIHierarchy();

            // Save as prefab
            bool success;
            PrefabUtility.SaveAsPrefabAsset(saveLoadUI, PREFAB_PATH, out success);

            if (success)
            {
                Debug.Log($"[SaveLoadUIGenerator] ✅ Successfully created SaveLoadUI prefab at {PREFAB_PATH}");
                EditorUtility.DisplayDialog(
                    "Save/Load UI Prefab Created",
                    $"SaveLoadUI prefab has been successfully created at:\n{PREFAB_PATH}\n\nYou can now use it in your scenes.",
                    "OK"
                );

                // Select the prefab in Project window
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(PREFAB_PATH);
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            else
            {
                Debug.LogError($"[SaveLoadUIGenerator] ❌ Failed to create SaveLoadUI prefab at {PREFAB_PATH}");
            }

            // Clean up scene GameObject
            Object.DestroyImmediate(saveLoadUI);
        }

        private static GameObject CreateSaveLoadUIHierarchy()
        {
            // Root GameObject - fullscreen overlay
            GameObject saveLoadUI = new GameObject("SaveLoadUI");
            RectTransform rootRect = saveLoadUI.AddComponent<RectTransform>();

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            rootRect.anchoredPosition = Vector2.zero;

            // Add CanvasGroup for fade in/out
            CanvasGroup canvasGroup = saveLoadUI.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Create Background Overlay (semi-transparent black)
            GameObject backgroundOverlay = CreateBackgroundOverlay(saveLoadUI.transform);

            // Create Panel Container
            GameObject panel = CreatePanelContainer(saveLoadUI.transform);

            // Create Title Text
            GameObject title = CreateTitleText(panel.transform);

            // Create Save/Load Tabs
            GameObject tabContainer = CreateTabContainer(panel.transform);

            // Create Scroll View for save slots
            GameObject scrollView = CreateScrollView(panel.transform);

            // Create Close Button
            GameObject closeButton = CreateCloseButton(panel.transform);

            return saveLoadUI;
        }

        private static GameObject CreateBackgroundOverlay(Transform parent)
        {
            GameObject overlay = new GameObject("BackgroundOverlay");
            overlay.transform.SetParent(parent, false);

            RectTransform rect = overlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;

            Image image = overlay.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.8f); // Semi-transparent black
            image.raycastTarget = true;

            Button button = overlay.AddComponent<Button>();
            button.targetGraphic = image;
            // User can wire up close action to this button click

            return overlay;
        }

        private static GameObject CreatePanelContainer(Transform parent)
        {
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(800f, 600f); // 800x600 panel

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.15f, 0.15f, 0.2f, 1f); // Dark panel background

            return panel;
        }

        private static GameObject CreateTitleText(Transform parent)
        {
            GameObject title = new GameObject("Title");
            title.transform.SetParent(parent, false);

            RectTransform rect = title.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -20f);
            rect.sizeDelta = new Vector2(-40f, 60f);

            TextMeshProUGUI text = title.AddComponent<TextMeshProUGUI>();
            text.text = "Save / Load Game";
            text.fontSize = 32;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            return title;
        }

        private static GameObject CreateTabContainer(Transform parent)
        {
            GameObject tabContainer = new GameObject("TabContainer");
            tabContainer.transform.SetParent(parent, false);

            RectTransform rect = tabContainer.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -90f);
            rect.sizeDelta = new Vector2(-40f, 50f);

            // Create Save Tab Button
            GameObject saveTab = CreateTabButton(tabContainer.transform, "Save", new Vector2(0f, 0f));

            // Create Load Tab Button
            GameObject loadTab = CreateTabButton(tabContainer.transform, "Load", new Vector2(200f, 0f));

            return tabContainer;
        }

        private static GameObject CreateTabButton(Transform parent, string tabName, Vector2 position)
        {
            GameObject tab = new GameObject($"{tabName}Tab");
            tab.transform.SetParent(parent, false);

            RectTransform rect = tab.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(180f, 50f);

            Image image = tab.AddComponent<Image>();
            image.color = new Color(0.25f, 0.25f, 0.3f, 1f);

            Button button = tab.AddComponent<Button>();
            button.targetGraphic = image;

            // Tab Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(tab.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = tabName;
            text.fontSize = 20;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            return tab;
        }

        private static GameObject CreateScrollView(Transform parent)
        {
            GameObject scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(parent, false);

            RectTransform rect = scrollView.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(20f, 80f); // 20px left, 80px bottom
            rect.offsetMax = new Vector2(-20f, -160f); // 20px right, 160px from top

            Image scrollImage = scrollView.AddComponent<Image>();
            scrollImage.color = new Color(0.1f, 0.1f, 0.15f, 1f);

            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            // Create Viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);

            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;

            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            // Create Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, 1000f); // Will be resized by content

            VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Wire up ScrollRect references
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;

            return scrollView;
        }

        private static GameObject CreateCloseButton(Transform parent)
        {
            GameObject closeButton = new GameObject("CloseButton");
            closeButton.transform.SetParent(parent, false);

            RectTransform rect = closeButton.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-10f, -10f);
            rect.sizeDelta = new Vector2(50f, 50f);

            Image image = closeButton.AddComponent<Image>();
            image.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Red button

            Button button = closeButton.AddComponent<Button>();
            button.targetGraphic = image;

            // X Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(closeButton.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "✕";
            text.fontSize = 30;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;

            return closeButton;
        }
    }
}
