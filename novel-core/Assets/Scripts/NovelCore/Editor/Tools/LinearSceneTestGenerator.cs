using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Core.SceneManagement;
using UnityEngine.AddressableAssets;

namespace NovelCore.Editor.Tools
{

/// <summary>
/// Editor utility to create test scenes for validating linear scene progression.
/// Menu: NovelCore → Testing → Create Linear Test Scenes
/// </summary>
public static class LinearSceneTestGenerator
{
    private const string TEST_SCENES_DIR = "Assets/Content/Projects/Test/Scenes";

    [MenuItem("NovelCore/Testing/Create Linear Test Scenes")]
    public static void CreateLinearTestScenes()
    {
        // Create directory if it doesn't exist
        if (!System.IO.Directory.Exists(TEST_SCENES_DIR))
        {
            System.IO.Directory.CreateDirectory(TEST_SCENES_DIR);
            AssetDatabase.Refresh();
        }

        // Create three test scenes
        var scene1 = CreateTestScene("TestScene01_Start", "test_scene_01", "Test Scene 1: Start", 
            new List<string> {
                "Welcome to the linear progression test!",
                "This is scene 1 of 3.",
                "Click to advance to the next scene..."
            });

        var scene2 = CreateTestScene("TestScene02_Middle", "test_scene_02", "Test Scene 2: Middle",
            new List<string> {
                "Great! You've reached scene 2.",
                "The nextScene field is working correctly.",
                "One more scene to go..."
            });

        var scene3 = CreateTestScene("TestScene03_End", "test_scene_03", "Test Scene 3: End",
            new List<string> {
                "Excellent! You've reached the final scene.",
                "Linear scene progression is working!",
                "[End of Test]"
            });

        // Link scenes: Scene1 → Scene2 → Scene3 → null
        LinkScenes(scene1, scene2);
        LinkScenes(scene2, scene3);
        // Scene3 has no nextScene (ends the test)

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Display success message
        EditorUtility.DisplayDialog(
            "Test Scenes Created",
            $"Linear progression test scenes created successfully!\n\n" +
            $"Location: {TEST_SCENES_DIR}\n\n" +
            $"Scenes:\n" +
            $"• TestScene01_Start → TestScene02_Middle\n" +
            $"• TestScene02_Middle → TestScene03_End\n" +
            $"• TestScene03_End (no nextScene - ends here)\n\n" +
            $"To Test:\n" +
            $"1. Select TestScene01_Start in Project window\n" +
            $"2. Verify nextScene field in Inspector\n" +
            $"3. Set as Starting Scene in GameStarter\n" +
            $"4. Press Play and click through dialogue\n" +
            $"5. Verify automatic transitions work!",
            "OK"
        );

        // Select first scene
        Selection.activeObject = scene1;
        EditorGUIUtility.PingObject(scene1);

        Debug.Log($"✅ Linear test scenes created at {TEST_SCENES_DIR}");
    }

    private static SceneData CreateTestScene(string fileName, string sceneId, string sceneName, List<string> dialogueTexts)
    {
        // Create SceneData asset
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        
        // Set basic properties using reflection
        SetPrivateField(scene, "_sceneId", sceneId);
        SetPrivateField(scene, "_sceneName", sceneName);
        SetPrivateField(scene, "_transitionType", TransitionType.Fade);
        SetPrivateField(scene, "_transitionDuration", 0.5f);
        SetPrivateField(scene, "_autoAdvance", false);
        SetPrivateField(scene, "_autoAdvanceDelay", 2.0f);

        // Save scene asset FIRST (required before adding sub-assets)
        string path = $"{TEST_SCENES_DIR}/{fileName}.asset";
        AssetDatabase.CreateAsset(scene, path);

        // Create dialogue lines as sub-assets
        var dialogueLines = new List<DialogueLineData>();
        for (int i = 0; i < dialogueTexts.Count; i++)
        {
            var line = CreateDialogueLine(dialogueTexts[i], fileName, i);
            dialogueLines.Add(line);
            AssetDatabase.AddObjectToAsset(line, scene);
        }

        SetPrivateField(scene, "_dialogueLines", dialogueLines);
        SetPrivateField(scene, "_characters", new List<CharacterPlacement>());
        SetPrivateField(scene, "_choices", new List<Runtime.Data.Choices.ChoiceData>());

        // Mark scene as dirty and save
        EditorUtility.SetDirty(scene);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"✓ Created test scene: {path} with {dialogueLines.Count} lines");
        
        return scene;
    }

    private static DialogueLineData CreateDialogueLine(string text, string sceneFileName, int lineIndex)
    {
        DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
        line.name = $"{sceneFileName}_Line{lineIndex:D2}";
        
        string lineId = $"test_line_{sceneFileName}_{lineIndex:D3}";
        SetPrivateField(line, "_lineId", lineId);
        SetPrivateField(line, "_emotion", "neutral");
        SetPrivateField(line, "_textKey", lineId);
        SetPrivateField(line, "_fallbackText", text);
        SetPrivateField(line, "_displayDuration", -1f);
        SetPrivateField(line, "_characterAction", CharacterAction.None);
        
        return line;
    }

    private static void LinkScenes(SceneData sourceScene, SceneData targetScene)
    {
        if (sourceScene == null || targetScene == null)
        {
            Debug.LogError("Cannot link scenes: one or both are null");
            return;
        }

        // Create AssetReference to target scene
        string targetPath = AssetDatabase.GetAssetPath(targetScene);
        string guid = AssetDatabase.AssetPathToGUID(targetPath);
        
        var nextSceneRef = new AssetReference(guid);
        SetPrivateField(sourceScene, "_nextScene", nextSceneRef);
        
        EditorUtility.SetDirty(sourceScene);
        
        Debug.Log($"✓ Linked: {sourceScene.SceneName} → {targetScene.SceneName}");
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
            Debug.LogWarning($"Field '{fieldName}' not found on {obj.GetType().Name}");
        }
    }

    [MenuItem("NovelCore/Testing/Delete Linear Test Scenes")]
    public static void DeleteLinearTestScenes()
    {
        if (System.IO.Directory.Exists(TEST_SCENES_DIR))
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Delete Test Scenes",
                $"Are you sure you want to delete all test scenes in:\n{TEST_SCENES_DIR}",
                "Delete",
                "Cancel"
            );

            if (confirm)
            {
                AssetDatabase.DeleteAsset("Assets/Content/Projects/Test");
                AssetDatabase.Refresh();
                Debug.Log("✓ Test scenes deleted");
            }
        }
        else
        {
            EditorUtility.DisplayDialog(
                "No Test Scenes Found",
                $"No test scenes directory exists at:\n{TEST_SCENES_DIR}",
                "OK"
            );
        }
    }
}

}
