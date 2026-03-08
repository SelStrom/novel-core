using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Core;
using NovelCore.Runtime.UI;
using NovelCore.Runtime.UI.NavigationControls;
using VContainer.Unity;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace NovelCore.Editor.Tools.Generators
{

/// <summary>
/// Editor tool for generating a sample visual novel project with test assets.
/// Creates a simple branching story with backgrounds, characters, and dialogue.
/// Usage: Menu → NovelCore → Generate Sample Project
/// </summary>
public static class SampleProjectGenerator
{
    private const string SAMPLE_PROJECT_DIR = "Assets/Content/Projects/Sample";
    private const string SCENES_DIR = "Assets/Content/Projects/Sample/Scenes";
    private const string BACKGROUNDS_DIR = "Assets/Content/Projects/Sample/Backgrounds";
    private const string CHARACTERS_DIR = "Assets/Content/Projects/Sample/Characters";
    private const string UNITY_SCENE_PATH = "Assets/Scenes/SampleScene.unity";

    [MenuItem("NovelCore/Generate Sample Project")]
    public static void GenerateSampleProject()
    {
        // Check if sample project already exists
        if (Directory.Exists(SAMPLE_PROJECT_DIR))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Sample Project Already Exists",
                $"A sample project already exists at {SAMPLE_PROJECT_DIR}.\n\nDo you want to overwrite it?",
                "Overwrite",
                "Cancel"
            );

            if (!overwrite)
            {
                Debug.Log("[SampleProjectGenerator] Generation cancelled by user.");
                return;
            }

            // Delete existing directory
            AssetDatabase.DeleteAsset(SAMPLE_PROJECT_DIR);
            AssetDatabase.Refresh();
        }

        // Create directory structure
        CreateDirectoryStructure();

        // Generate placeholder assets
        GeneratePlaceholderBackgrounds();
        GeneratePlaceholderCharacters();

        // Generate story data
        var scenes = GenerateStoryScenes();

        // Mark all scenes as Addressable
        foreach (var scene in scenes.Values)
        {
            MarkAssetAsAddressable(scene);
        }

        // Link scenes together
        LinkScenesWithChoices(scenes);

        // Save all assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Setup Unity scene with GameStarter and UIManager
        var firstScene = AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene01_Introduction.asset");
        SetupUnitySceneWithGameStarter(firstScene);
        SetupUIManager();
        SetupNavigationUI();

        // Display success message
        Debug.Log($"[SampleProjectGenerator] ✅ Successfully created sample project at {SAMPLE_PROJECT_DIR}");
        EditorUtility.DisplayDialog(
            "Sample Project Created",
            $"Sample project has been successfully created!\n\n" +
            $"Location: {SAMPLE_PROJECT_DIR}\n\n" +
            $"Story Structure:\n" +
            $"• Scene 1: Introduction (3 dialogue lines)\n" +
            $"• Scene 2: Choice Point (2 options)\n" +
            $"• Scene 3a: Path A (Happy Ending)\n" +
            $"• Scene 3b: Path B (Neutral Ending)\n\n" +
            $"Unity Scene Setup:\n" +
            $"• GameLifetimeScope configured\n" +
            $"• GameStarter configured with starting scene\n\n" +
            $"To test:\n" +
            $"1. Press Play ▶️ in Unity Editor\n" +
            $"2. Game will auto-start after 0.5s\n" +
            $"3. Click to advance dialogue\n" +
            $"4. Make your choice!",
            "OK"
        );

        // Select first scene
        Selection.activeObject = firstScene;
        EditorGUIUtility.PingObject(firstScene);
    }

    private static void CreateDirectoryStructure()
    {
        Directory.CreateDirectory(SAMPLE_PROJECT_DIR);
        Directory.CreateDirectory(SCENES_DIR);
        Directory.CreateDirectory(BACKGROUNDS_DIR);
        Directory.CreateDirectory(CHARACTERS_DIR);
        AssetDatabase.Refresh();
    }

    private static void GeneratePlaceholderBackgrounds()
    {
        // Generate simple colored textures as placeholder backgrounds
        CreateColoredTexture("bg_room", new Color(0.8f, 0.7f, 0.6f), BACKGROUNDS_DIR); // Beige (room)
        CreateColoredTexture("bg_street", new Color(0.6f, 0.7f, 0.8f), BACKGROUNDS_DIR); // Light blue (street)
        CreateColoredTexture("bg_home", new Color(0.7f, 0.8f, 0.7f), BACKGROUNDS_DIR); // Light green (home)
    }

    private static void GeneratePlaceholderCharacters()
    {
        // Generate simple colored textures as placeholder characters
        CreateColoredTexture("char_protagonist", new Color(1f, 0.8f, 0.6f), CHARACTERS_DIR); // Skin tone
    }

    private static void CreateColoredTexture(string name, Color color, string directory)
    {
        // Create a simple 256x256 texture with a colored rectangle
        int width = 256;
        int height = 256;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Fill with color (add some simple gradient for visual interest)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float gradient = 1f - (y / (float)height * 0.2f); // Subtle vertical gradient
                Color pixelColor = color * gradient;
                pixelColor.a = 1f;
                texture.SetPixel(x, y, pixelColor);
            }
        }
        texture.Apply();

        // Save as PNG
        byte[] bytes = texture.EncodeToPNG();
        string path = $"{directory}/{name}.png";
        File.WriteAllBytes(path, bytes);
        
        AssetDatabase.Refresh();

        // Configure texture import settings
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        Debug.Log($"[SampleProjectGenerator] Created placeholder texture: {path}");
    }

    private static Dictionary<string, SceneData> GenerateStoryScenes()
    {
        var scenes = new Dictionary<string, SceneData>();

        // Scene 1: Introduction
        scenes["scene1"] = CreateScene(
            "Scene01_Introduction",
            "scene_intro_001",
            "Введение",
            new List<DialogueContent>
            {
                new DialogueContent
                {
                    lineId = "line_intro_001",
                    fallbackText = "Привет! Это демонстрационная визуальная новелла.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_intro_002",
                    fallbackText = "Сейчас ты увидишь, как работает система диалогов и выборов.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_intro_003",
                    fallbackText = "Нажимай на экран, чтобы продолжить...",
                    emotion = "neutral"
                }
            },
            "bg_room"
        );

        // Scene 2: Choice Point
        scenes["scene2"] = CreateScene(
            "Scene02_ChoicePoint",
            "scene_choice_001",
            "Точка выбора",
            new List<DialogueContent>
            {
                new DialogueContent
                {
                    lineId = "line_choice_001",
                    fallbackText = "Пришло время сделать выбор.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_choice_002",
                    fallbackText = "Куда ты хочешь пойти?",
                    emotion = "neutral"
                }
            },
            "bg_room"
        );

        // Scene 3a: Path A (Going outside)
        scenes["scene3a"] = CreateScene(
            "Scene03a_PathA",
            "scene_path_a_001",
            "Путь A: Прогулка",
            new List<DialogueContent>
            {
                new DialogueContent
                {
                    lineId = "line_path_a_001",
                    fallbackText = "Ты решил выйти на улицу.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_path_a_002",
                    fallbackText = "Свежий воздух и солнечный день подняли твоё настроение!",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_path_a_003",
                    fallbackText = "[Счастливая концовка - Path A]",
                    emotion = "neutral"
                }
            },
            "bg_street"
        );

        // Scene 3b: Path B (Staying home)
        scenes["scene3b"] = CreateScene(
            "Scene03b_PathB",
            "scene_path_b_001",
            "Путь B: Дома",
            new List<DialogueContent>
            {
                new DialogueContent
                {
                    lineId = "line_path_b_001",
                    fallbackText = "Ты решил остаться дома.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_path_b_002",
                    fallbackText = "Уютный вечер с книгой - тоже неплохой вариант.",
                    emotion = "neutral"
                },
                new DialogueContent
                {
                    lineId = "line_path_b_003",
                    fallbackText = "[Нейтральная концовка - Path B]",
                    emotion = "neutral"
                }
            },
            "bg_home"
        );

        return scenes;
    }

    private static SceneData CreateScene(
        string fileName,
        string sceneId,
        string sceneName,
        List<DialogueContent> dialogueContents,
        string backgroundName)
    {
        // Create SceneData asset
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        
        // Use reflection to set private fields (since they're serialized but have no public setters)
        var sceneType = typeof(SceneData);
        
        SetPrivateField(scene, "_sceneId", sceneId);
        SetPrivateField(scene, "_sceneName", sceneName);
        SetPrivateField(scene, "_transitionType", TransitionType.Fade);
        SetPrivateField(scene, "_transitionDuration", 0.5f);
        SetPrivateField(scene, "_autoAdvance", false);
        SetPrivateField(scene, "_autoAdvanceDelay", 2.0f);

        // Save scene asset FIRST (required before adding sub-assets)
        string path = $"{SCENES_DIR}/{fileName}.asset";
        AssetDatabase.CreateAsset(scene, path);

        // Create dialogue lines as sub-assets
        var dialogueLines = new List<DialogueLineData>();
        int lineIndex = 0;
        foreach (var content in dialogueContents)
        {
            var line = CreateDialogueLine(content, fileName, lineIndex);
            dialogueLines.Add(line);
            
            // Add as sub-asset to the SceneData asset
            AssetDatabase.AddObjectToAsset(line, scene);
            lineIndex++;
        }

        SetPrivateField(scene, "_dialogueLines", dialogueLines);
        SetPrivateField(scene, "_characters", new List<CharacterPlacement>());
        SetPrivateField(scene, "_choices", new List<ChoiceData>());

        // Mark scene as dirty and save
        EditorUtility.SetDirty(scene);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"[SampleProjectGenerator] Created scene: {path} with {dialogueLines.Count} dialogue lines");
        
        return scene;
    }

    private static DialogueLineData CreateDialogueLine(DialogueContent content, string sceneFileName, int lineIndex)
    {
        DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
        line.name = $"{sceneFileName}_Line{lineIndex:D2}";
        
        SetPrivateField(line, "_lineId", content.lineId);
        SetPrivateField(line, "_emotion", content.emotion);
        SetPrivateField(line, "_textKey", content.lineId);
        SetPrivateField(line, "_fallbackText", content.fallbackText);
        SetPrivateField(line, "_displayDuration", -1f);
        SetPrivateField(line, "_characterAction", CharacterAction.None);
        
        return line;
    }

    private static void LinkScenesWithChoices(Dictionary<string, SceneData> scenes)
    {
        // Link Scene01 -> Scene02 (linear progression)
        LinkScenesLinear(scenes["scene1"], scenes["scene2"]);

        // Create choice for Scene 2
        ChoiceData choice = ScriptableObject.CreateInstance<ChoiceData>();
        choice.name = "Choice_MainDecision";
        
        SetPrivateField(choice, "_choiceId", "choice_main_001");
        SetPrivateField(choice, "_promptTextKey", "choice_prompt_001");
        SetPrivateField(choice, "_fallbackPromptText", "Выбери свой путь:");
        
        var options = new List<ChoiceOption>
        {
            new ChoiceOption
            {
                optionId = "option_a",
                textKey = "option_a_text",
                fallbackText = "Выйти на улицу",
                targetScene = null, // Will be set manually after Addressables setup
                requiredChoices = new List<string>(),
                isAvailable = true,
                icon = null
            },
            new ChoiceOption
            {
                optionId = "option_b",
                textKey = "option_b_text",
                fallbackText = "Остаться дома",
                targetScene = null, // Will be set manually after Addressables setup
                requiredChoices = new List<string>(),
                isAvailable = true,
                icon = null
            }
        };
        
        SetPrivateField(choice, "_options", options);
        SetPrivateField(choice, "_timerSeconds", 0f);
        SetPrivateField(choice, "_defaultOptionIndex", 0);

        // Save choice as a separate asset (ChoiceData can be reused across scenes)
        string choicePath = $"{SCENES_DIR}/Choice_MainDecision.asset";
        AssetDatabase.CreateAsset(choice, choicePath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"[SampleProjectGenerator] Created choice: {choicePath}");

        // Add choice to Scene 2
        var scene2Choices = new List<ChoiceData> { choice };
        SetPrivateField(scenes["scene2"], "_choices", scene2Choices);
        
        EditorUtility.SetDirty(scenes["scene2"]);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Links two scenes together using nextScene field for linear progression
    /// </summary>
    private static void LinkScenesLinear(SceneData fromScene, SceneData toScene)
    {
        if (fromScene == null || toScene == null)
        {
            Debug.LogWarning("[SampleProjectGenerator] Cannot link null scenes");
            return;
        }

        // Create AssetReference to the target scene
        var assetPath = AssetDatabase.GetAssetPath(toScene);
        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        
        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference(guid);
        SetPrivateField(fromScene, "_nextScene", nextSceneRef);
        
        EditorUtility.SetDirty(fromScene);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"[SampleProjectGenerator] Linked {fromScene.SceneName} -> {toScene.SceneName}");
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"[SampleProjectGenerator] Field '{fieldName}' not found on {obj.GetType().Name}");
        }
    }

    /// <summary>
    /// T040.3: Automatically configures Unity scene with GameLifetimeScope and GameStarter
    /// </summary>
    private static void SetupUnitySceneWithGameStarter(SceneData startingScene)
    {
        Debug.Log("[SampleProjectGenerator] Setting up Unity scene with GameStarter...");

        // Check if Unity scene exists
        if (!File.Exists(UNITY_SCENE_PATH))
        {
            Debug.LogError($"[SampleProjectGenerator] Unity scene not found at {UNITY_SCENE_PATH}. Please create it manually.");
            return;
        }

        // Open the scene
        var scene = EditorSceneManager.OpenScene(UNITY_SCENE_PATH, OpenSceneMode.Single);
        if (!scene.IsValid())
        {
            Debug.LogError($"[SampleProjectGenerator] Failed to open Unity scene at {UNITY_SCENE_PATH}");
            return;
        }

        // Find or create GameLifetimeScope
        var lifetimeScope = GameObject.FindFirstObjectByType<GameLifetimeScope>();
        if (lifetimeScope == null)
        {
            Debug.Log("[SampleProjectGenerator] Creating GameLifetimeScope GameObject...");
            var lifetimeScopeObj = new GameObject("GameLifetimeScope");
            lifetimeScope = lifetimeScopeObj.AddComponent<GameLifetimeScope>();
            Undo.RegisterCreatedObjectUndo(lifetimeScopeObj, "Create GameLifetimeScope");
        }
        else
        {
            Debug.Log("[SampleProjectGenerator] GameLifetimeScope already exists, skipping creation.");
        }

        // Find or create GameStarter
        var gameStarter = GameObject.FindFirstObjectByType<GameStarter>();
        GameObject gameStarterObj;

        if (gameStarter == null)
        {
            Debug.Log("[SampleProjectGenerator] Creating GameStarter GameObject...");
            gameStarterObj = new GameObject("GameStarter");
            gameStarter = gameStarterObj.AddComponent<GameStarter>();
            Undo.RegisterCreatedObjectUndo(gameStarterObj, "Create GameStarter");
        }
        else
        {
            Debug.Log("[SampleProjectGenerator] GameStarter already exists, updating configuration...");
            gameStarterObj = gameStarter.gameObject;
        }

        // Configure GameStarter using SerializedObject to set private fields
        var serializedObject = new UnityEditor.SerializedObject(gameStarter);
        
        var startingSceneProperty = serializedObject.FindProperty("_startingScene");
        if (startingSceneProperty != null)
        {
            startingSceneProperty.objectReferenceValue = startingScene;
            Debug.Log($"[SampleProjectGenerator] Assigned starting scene: {startingScene.SceneName}");
        }
        else
        {
            Debug.LogWarning("[SampleProjectGenerator] Could not find _startingScene property on GameStarter");
        }

        var autoStartProperty = serializedObject.FindProperty("_autoStart");
        if (autoStartProperty != null)
        {
            autoStartProperty.boolValue = true;
        }

        var startDelayProperty = serializedObject.FindProperty("_startDelay");
        if (startDelayProperty != null)
        {
            startDelayProperty.floatValue = 0.5f;
        }

        serializedObject.ApplyModifiedProperties();

        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[SampleProjectGenerator] ✅ Unity scene setup complete!");
        Debug.Log($"[SampleProjectGenerator]    • GameLifetimeScope: {(lifetimeScope != null ? "✓" : "✗")}");
        Debug.Log($"[SampleProjectGenerator]    • GameStarter: {(gameStarter != null ? "✓" : "✗")}");
        Debug.Log($"[SampleProjectGenerator]    • Starting scene assigned: {(startingScene != null ? startingScene.SceneName : "None")}");
        Debug.Log($"[SampleProjectGenerator]    • Auto-start enabled: True");
        Debug.Log($"[SampleProjectGenerator]    • Start delay: 0.5s");
    }

    /// <summary>
    /// Automatically configures Unity scene with UIManager for DialogueBox initialization
    /// </summary>
    private static void SetupUIManager()
    {
        Debug.Log("[SampleProjectGenerator] Setting up UIManager...");

        // Scene should already be open from SetupUnitySceneWithGameStarter
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("[SampleProjectGenerator] No active scene found");
            return;
        }

        // Find or create UIManager
        var uiManager = GameObject.FindFirstObjectByType<UIManager>();
        GameObject uiManagerObj;

        if (uiManager == null)
        {
            Debug.Log("[SampleProjectGenerator] Creating UIManager GameObject...");
            uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<UIManager>();
            Undo.RegisterCreatedObjectUndo(uiManagerObj, "Create UIManager");
        }
        else
        {
            Debug.Log("[SampleProjectGenerator] UIManager already exists, skipping creation.");
            uiManagerObj = uiManager.gameObject;
        }

        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[SampleProjectGenerator] ✅ UIManager setup complete!");
        Debug.Log($"[SampleProjectGenerator]    • UIManager: {(uiManager != null ? "✓" : "✗")}");
    }

    /// <summary>
    /// Creates navigation UI with back/forward buttons for scene history navigation
    /// </summary>
    private static void SetupNavigationUI()
    {
        Debug.Log("[SampleProjectGenerator] Setting up Navigation UI...");

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("[SampleProjectGenerator] No active scene found");
            return;
        }

        // Find or create Canvas
        Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("[SampleProjectGenerator] Creating Canvas for Navigation UI...");
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
        }

        // Check if NavigationUI already exists
        var existingNavUI = GameObject.FindFirstObjectByType<SceneNavigationUI>();
        if (existingNavUI != null)
        {
            Debug.Log("[SampleProjectGenerator] SceneNavigationUI already exists, skipping creation.");
            return;
        }

        // Create NavigationUI container
        GameObject navUIContainer = new GameObject("NavigationUI");
        navUIContainer.transform.SetParent(canvas.transform, false);
        
        var navUIRect = navUIContainer.AddComponent<RectTransform>();
        navUIRect.anchorMin = new Vector2(0, 0);
        navUIRect.anchorMax = new Vector2(1, 0);
        navUIRect.pivot = new Vector2(0.5f, 0);
        navUIRect.anchoredPosition = new Vector2(0, 20);
        navUIRect.sizeDelta = new Vector2(-40, 80);

        // Add HorizontalLayoutGroup for button arrangement
        var layoutGroup = navUIContainer.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 20;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Create Back Button
        GameObject backButtonObj = CreateNavigationButton("BackButton", "← Назад", navUIContainer.transform);
        Button backButton = backButtonObj.GetComponent<Button>();

        // Create Forward Button
        GameObject forwardButtonObj = CreateNavigationButton("ForwardButton", "Вперёд →", navUIContainer.transform);
        Button forwardButton = forwardButtonObj.GetComponent<Button>();

        // Add SceneNavigationUI component
        var navUI = navUIContainer.AddComponent<SceneNavigationUI>();
        
        // Use SerializedObject to set private fields
        var serializedObject = new SerializedObject(navUI);
        
        var backButtonProperty = serializedObject.FindProperty("_backButton");
        if (backButtonProperty != null)
        {
            backButtonProperty.objectReferenceValue = backButton;
        }

        var forwardButtonProperty = serializedObject.FindProperty("_forwardButton");
        if (forwardButtonProperty != null)
        {
            forwardButtonProperty.objectReferenceValue = forwardButton;
        }

        serializedObject.ApplyModifiedProperties();

        // Create NavigationUIManager to handle initialization
        GameObject navManagerObj = new GameObject("NavigationUIManager");
        var navManager = navManagerObj.AddComponent<NavigationUIManager>();
        
        var navManagerSerialized = new SerializedObject(navManager);
        var navUIProperty = navManagerSerialized.FindProperty("_navigationUI");
        if (navUIProperty != null)
        {
            navUIProperty.objectReferenceValue = navUI;
        }
        navManagerSerialized.ApplyModifiedProperties();

        Undo.RegisterCreatedObjectUndo(navUIContainer, "Create Navigation UI");
        Undo.RegisterCreatedObjectUndo(navManagerObj, "Create Navigation UI Manager");

        // Mark scene as dirty and save
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[SampleProjectGenerator] ✅ Navigation UI setup complete!");
        Debug.Log("[SampleProjectGenerator]    • Back Button: ✓");
        Debug.Log("[SampleProjectGenerator]    • Forward Button: ✓");
        Debug.Log("[SampleProjectGenerator]    • SceneNavigationUI: ✓");
        Debug.Log("[SampleProjectGenerator]    • NavigationUIManager: ✓");
    }

    /// <summary>
    /// Helper method to create a navigation button with consistent styling
    /// </summary>
    private static GameObject CreateNavigationButton(string name, string text, Transform parent)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        // Add RectTransform
        var rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 60);

        // Add Image component (button background)
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        image.type = Image.Type.Sliced;

        // Add Button component
        var button = buttonObj.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        button.colors = colors;

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 20;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return buttonObj;
    }

    /// <summary>
    /// Marks a SceneData asset as Addressable in the default group.
    /// </summary>
    private static void MarkAssetAsAddressable(SceneData scene)
    {
        if (scene == null)
        {
            Debug.LogError("[SampleProjectGenerator] Cannot mark null scene as Addressable");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(scene);
        string guid = AssetDatabase.AssetPathToGUID(assetPath);

        // Get Addressables settings
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("[SampleProjectGenerator] Addressables settings not found. Please initialize Addressables.");
            return;
        }

        // Check if already addressable
        var entry = settings.FindAssetEntry(guid);
        if (entry != null)
        {
            Debug.Log($"[SampleProjectGenerator] ✓ Already Addressable: {scene.SceneName}");
            return;
        }

        // Get default group (or create if needed)
        var defaultGroup = settings.DefaultGroup;
        if (defaultGroup == null)
        {
            Debug.LogError("[SampleProjectGenerator] No default Addressables group found");
            return;
        }

        // Add asset to Addressables
        entry = settings.CreateOrMoveEntry(guid, defaultGroup, false, false);
        entry.address = scene.SceneName; // Use scene name as address

        // Mark settings as dirty
        EditorUtility.SetDirty(settings);
        
        Debug.Log($"[SampleProjectGenerator] ✅ Marked as Addressable: {scene.SceneName} (GUID: {guid})");
    }

    private struct DialogueContent
    {
        public string lineId;
        public string fallbackText;
        public string emotion;
    }
}

}
