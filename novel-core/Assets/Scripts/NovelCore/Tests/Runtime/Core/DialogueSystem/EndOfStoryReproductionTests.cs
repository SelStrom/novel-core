using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using DialogueSystemImpl = NovelCore.Runtime.Core.DialogueSystem.DialogueSystem;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Tests.Runtime.Builders;
using NovelCore.Tests.Runtime.Core;

namespace NovelCore.Tests.Runtime.Core.DialogueSystem
{

/// <summary>
/// Tests for end-of-story scenario (last dialogue with no valid next scene).
/// Regression tests for InvalidKeyException bug when NextScene AssetReference is invalid.
/// </summary>
public class EndOfStoryReproductionTests : BaseTestFixture
{
    private DialogueSystemImpl _dialogueSystem;
    private MockAudioService _mockAudioService;
    private MockInputService _mockInputService;
    private MockSceneManager _mockSceneManager;
    private MockAssetManager _mockAssetManager;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _mockAudioService = new MockAudioService();
        _mockInputService = new MockInputService();
        _mockSceneManager = new MockSceneManager();
        _mockAssetManager = new MockAssetManager();

        _dialogueSystem = new DialogueSystemImpl(
            _mockAudioService,
            _mockInputService,
            _mockSceneManager,
            _mockAssetManager
        );
    }

    /// <summary>
    /// Test that verifies graceful handling of empty AssetReference in NextScene.
    /// This was the original bug: InvalidKeyException when AssetReference is non-null but has empty RuntimeKey.
    /// </summary>
    [UnityTest]
    public IEnumerator EndOfStory_WithEmptyNextSceneReference_ShouldCompleteGracefullyWithoutException()
    {
        // Arrange: Create final scene with empty (but non-null) AssetReference
        var emptySceneRef = new UnityEngine.AddressableAssets.AssetReference("");
        
        var finalSceneData = new SceneDataBuilder()
            .WithSceneId("scene_final")
            .WithSceneName("Final Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_final")
                .WithFallbackText("The end")
                .Build())
            .WithNextScene(emptySceneRef)
            .Build();

        bool dialogueCompleteFired = false;
        bool navigationFired = false;
        
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;

        // Act: Start scene and advance to last dialogue
        _dialogueSystem.StartScene(finalSceneData);
        yield return null;

        // Expect warning log (not error)
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert: Should complete gracefully without InvalidKeyException
        Assert.IsTrue(dialogueCompleteFired, 
            "OnDialogueComplete should fire when reaching end of story with invalid NextScene");
        Assert.IsFalse(navigationFired, 
            "OnSceneNavigationRequested should NOT fire with invalid next scene");
        Assert.IsFalse(_dialogueSystem.IsPlaying, 
            "Dialogue should stop playing at end of story");
    }

    /// <summary>
    /// Test that null NextScene is handled correctly (baseline test).
    /// </summary>
    [UnityTest]
    public IEnumerator EndOfStory_WithNullNextScene_ShouldCompleteGracefully()
    {
        // Arrange: Create final scene with null NextScene
        var finalSceneData = new SceneDataBuilder()
            .WithSceneId("scene_final_null")
            .WithSceneName("Final Scene Null")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_final_null")
                .WithFallbackText("The end (null nextScene)")
                .Build())
            .Build();

        bool dialogueCompleteFired = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act
        _dialogueSystem.StartScene(finalSceneData);
        yield return null;

        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.IsTrue(dialogueCompleteFired, 
            "OnDialogueComplete should fire when NextScene is null");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test that verifies graceful handling when AssetReference has invalid GUID.
    /// Invalid GUID (non-empty but not registered) behaves same as missing asset - load fails with ERROR.
    /// </summary>
    [UnityTest]
    public IEnumerator EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad()
    {
        // Arrange: Create scene with AssetReference that has invalid GUID
        var invalidRef = new UnityEngine.AddressableAssets.AssetReference("some_invalid_guid");
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_invalid_ref")
            .WithSceneName("Scene Invalid Ref")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test")
                .Build())
            .WithNextScene(invalidRef)
            .Build();

        bool dialogueCompleteFired = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Invalid GUID (non-empty) cannot be distinguished from missing asset without load attempt
        // Expect ERROR log (same as missing asset scenario)
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Failed to load target scene"));
        
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert: Should complete gracefully despite error
        Assert.IsTrue(dialogueCompleteFired, 
            "Should complete dialogue when AssetReference load fails");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }
}

}
