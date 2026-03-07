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
using VContainer.Unity;

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

        // Link scenes together
        LinkScenesWithChoices(scenes);

        // Save all assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Setup Unity scene with GameStarter and UIManager
        var firstScene = AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene01_Introduction.asset");
        SetupUnitySceneWithGameStarter(firstScene);
        SetupUIManager();

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

    private struct DialogueContent
    {
        public string lineId;
        public string fallbackText;
        public string emotion;
    }
}

}
