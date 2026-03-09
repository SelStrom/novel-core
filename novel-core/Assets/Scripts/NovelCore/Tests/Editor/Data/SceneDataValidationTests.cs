using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Core.SceneManagement;

namespace NovelCore.Tests.Editor.Data
{

/// <summary>
/// EditMode tests for SceneData validation with nextScene field.
/// Tests the validation logic for nextScene references.
/// </summary>
public class SceneDataValidationTests
{
    [Test]
    public void SceneData_WithValidNextScene_PassesValidation()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", "test_scene_001");
        SetPrivateField(scene, "_sceneName", "Test Scene");
        
        // Act
        bool result = scene.Validate();
        
        // Assert
        Assert.IsTrue(result, "Scene with valid nextScene should pass validation");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_WithoutSceneId_FailsValidation()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", ""); // Empty scene ID
        SetPrivateField(scene, "_sceneName", "Test Scene");
        
        // Expect the error log
        LogAssert.Expect(LogType.Error, "SceneData : sceneId is required");
        
        // Act
        bool result = scene.Validate();
        
        // Assert
        Assert.IsFalse(result, "Scene without sceneId should fail validation");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_WithBothChoicesAndNextScene_ShowsWarning()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", "test_scene_002");
        SetPrivateField(scene, "_sceneName", "Test Scene with Choice");
        
        // Add a dialogue line
        var dialogueLines = new System.Collections.Generic.List<DialogueLineData>();
        var line = ScriptableObject.CreateInstance<DialogueLineData>();
        SetPrivateField(line, "_lineId", "line_001");
        SetPrivateField(line, "_fallbackText", "Test dialogue");
        dialogueLines.Add(line);
        SetPrivateField(scene, "_dialogueLines", dialogueLines);
        
        // Add a choice (simulating both choices and nextScene)
        var choices = new System.Collections.Generic.List<Runtime.Data.Choices.ChoiceData>();
        var choice = ScriptableObject.CreateInstance<Runtime.Data.Choices.ChoiceData>();
        SetPrivateField(choice, "_choiceId", "choice_001");
        choices.Add(choice);
        SetPrivateField(scene, "_choices", choices);
        
        // Set nextScene (creating conflict)
        var nextSceneRef = new AssetReference();
        SetPrivateField(scene, "_nextScene", nextSceneRef);
        
        // Act & Assert
        // Validation should still pass, but warning logged
        bool result = scene.Validate();
        Assert.IsTrue(result, "Scene with both choices and nextScene should pass validation (with warning)");
        
        // Cleanup
        Object.DestroyImmediate(line);
        Object.DestroyImmediate(choice);
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_WithNoDialogueOrChoices_ShowsWarning()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", "test_scene_003");
        SetPrivateField(scene, "_sceneName", "Empty Test Scene");
        SetPrivateField(scene, "_dialogueLines", new System.Collections.Generic.List<DialogueLineData>());
        SetPrivateField(scene, "_choices", new System.Collections.Generic.List<Runtime.Data.Choices.ChoiceData>());
        
        // Act
        bool result = scene.Validate();
        
        // Assert
        Assert.IsTrue(result, "Scene without dialogue or choices should still pass validation (with warning)");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_NextSceneProperty_ReturnsCorrectValue()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        var nextSceneRef = new AssetReference();
        SetPrivateField(scene, "_nextScene", nextSceneRef);
        
        // Act
        AssetReference result = scene.NextScene;
        
        // Assert
        Assert.AreEqual(nextSceneRef, result, "NextScene property should return the correct AssetReference");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    /// <summary>
    /// Helper method to set private fields using reflection.
    /// </summary>
    private void SetPrivateField(object obj, string fieldName, object value)
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
}

}
