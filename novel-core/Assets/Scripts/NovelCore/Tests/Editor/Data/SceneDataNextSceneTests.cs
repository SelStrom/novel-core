using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Tests.Editor.Data
{

/// <summary>
/// EditMode tests specifically for nextScene field functionality.
/// Tests that the nextScene field works correctly in SceneData.
/// </summary>
public class SceneDataNextSceneTests
{
    [Test]
    public void NextScene_WhenSet_ReturnsCorrectReference()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        var nextSceneRef = new AssetReference("test-guid-123");
        SetPrivateField(scene, "_nextScene", nextSceneRef);
        
        // Act
        AssetReference result = scene.NextScene;
        
        // Assert
        Assert.IsNotNull(result, "NextScene should not be null when set");
        Assert.AreEqual(nextSceneRef, result, "NextScene should return the exact AssetReference that was set");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void NextScene_WhenNotSet_ReturnsNull()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        // Don't set nextScene - leave it as default (null)
        
        // Act
        AssetReference result = scene.NextScene;
        
        // Assert
        Assert.IsNull(result, "NextScene should be null when not explicitly set");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_WithNextScene_HasCorrectProperty()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", "scene_001");
        SetPrivateField(scene, "_sceneName", "Test Scene");
        
        var nextSceneRef = new AssetReference();
        SetPrivateField(scene, "_nextScene", nextSceneRef);
        
        // Act
        bool hasNextScene = scene.NextScene != null;
        
        // Assert
        Assert.IsTrue(hasNextScene, "Scene with nextScene field set should expose it via property");
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_CanHaveNullNextScene_WithoutErrors()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        SetPrivateField(scene, "_sceneId", "scene_002");
        SetPrivateField(scene, "_sceneName", "Scene Without Next");
        SetPrivateField(scene, "_nextScene", null);
        
        // Act & Assert - should not throw exception
        Assert.DoesNotThrow(() => {
            var nextScene = scene.NextScene;
            Assert.IsNull(nextScene, "Explicitly null nextScene should return null");
        });
        
        // Cleanup
        Object.DestroyImmediate(scene);
    }

    [Test]
    public void SceneData_NextSceneProperty_IsReadOnly()
    {
        // Arrange
        SceneData scene = ScriptableObject.CreateInstance<SceneData>();
        var property = typeof(SceneData).GetProperty("NextScene");
        
        // Act & Assert
        Assert.IsTrue(property.CanRead, "NextScene property should be readable");
        Assert.IsFalse(property.CanWrite, "NextScene property should be read-only (no public setter)");
        
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
