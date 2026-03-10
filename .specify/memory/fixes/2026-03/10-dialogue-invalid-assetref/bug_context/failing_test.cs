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
/// Failing test reproducing InvalidKeyException on last dialogue with empty NextScene.
/// Bug: When clicking on the last dialogue with an invalid/empty AssetReference, 
/// the system throws InvalidKeyException instead of showing "Story Complete" notification.
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
    /// This test SHOULD FAIL before the fix.
    /// Expected: Should show "End of Story" notification without exceptions.
    /// Actual: Throws InvalidKeyException when trying to load with empty AssetReference.
    /// </summary>
    [UnityTest]
    public IEnumerator EndOfStory_WithEmptyNextSceneReference_ShouldShowNotificationWithoutException()
    {
        // Arrange: Create final scene with empty (but non-null) AssetReference
        // This simulates the real bug: NextScene field is assigned but empty in Unity Inspector
        var emptySceneRef = new UnityEngine.AddressableAssets.AssetReference("");
        
        var finalSceneData = new SceneDataBuilder()
            .WithSceneId("scene_final")
            .WithSceneName("Final Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_final")
                .WithFallbackText("The end")
                .Build())
            .WithNextScene(emptySceneRef) // Empty AssetReference (invalid key)
            .Build();

        bool dialogueCompleteFired = false;
        bool navigationFired = false;
        
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;

        // Act: Start scene and advance to last dialogue
        _dialogueSystem.StartScene(finalSceneData);
        yield return null; // Wait for initialization

        // This should NOT throw InvalidKeyException
        // Instead, it should gracefully handle the end of story
        LogAssert.NoUnexpectedReceived(); // Should not log InvalidKeyException
        
        _dialogueSystem.AdvanceDialogue(); // Click on last dialogue
        yield return new WaitForSeconds(0.2f); // Wait for async operations

        // Assert: 
        // 1. No exception should be thrown
        // 2. OnDialogueComplete should fire (story ended)
        // 3. No navigation should occur (no valid next scene)
        // 4. [Future] OnStoryComplete event should fire (or UI notification shown)
        Assert.IsTrue(dialogueCompleteFired, 
            "OnDialogueComplete should fire when reaching end of story");
        Assert.IsFalse(navigationFired, 
            "OnSceneNavigationRequested should NOT fire with invalid next scene");
        Assert.IsFalse(_dialogueSystem.IsPlaying, 
            "Dialogue should stop playing at end of story");
        
        // TODO: After fix, assert that EndOfStoryPanel is shown
        // This will require new event or UI check
    }

    /// <summary>
    /// This test SHOULD FAIL before the fix.
    /// Tests the case where AssetReference is completely null.
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
            // No .WithNextScene() call - should be null
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
    /// This test verifies the expected behavior after fix:
    /// Invalid AssetReference should be validated BEFORE attempting to load.
    /// </summary>
    [UnityTest]
    public IEnumerator EndOfStory_WithInvalidAssetReference_ShouldValidateBeforeLoad()
    {
        // Arrange: Create scene with AssetReference that has invalid key
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

        // The fix should check RuntimeKeyIsValid() BEFORE calling LoadAssetAsync
        // This prevents InvalidKeyException from being thrown
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert: Should complete gracefully even with invalid reference
        Assert.IsTrue(dialogueCompleteFired, 
            "Should complete dialogue when AssetReference is invalid");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
        
        // TODO: Should show EndOfStory notification if reference is invalid
    }
}

}
