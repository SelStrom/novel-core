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
/// Adversarial and edge case tests attempting to break the InvalidKeyException fix.
/// Tests boundary conditions and potential regressions.
/// </summary>
[TestFixture]
public class EndOfStoryBreakTests : BaseTestFixture
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
    /// Test rapid clicking on last dialogue - should not cause multiple exceptions.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_RapidClickOnLastDialogue_ShouldHandleGracefully()
    {
        // Arrange
        var emptySceneRef = new UnityEngine.AddressableAssets.AssetReference("");
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_rapid")
            .WithSceneName("Rapid Click Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Last line")
                .Build())
            .WithNextScene(emptySceneRef)
            .Build();

        int completeCount = 0;
        _dialogueSystem.OnDialogueComplete += () => completeCount++;

        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Act: Rapid click 10 times
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        
        for (int i = 0; i < 10; i++)
        {
            _dialogueSystem.AdvanceDialogue();
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        // Assert: Should only complete once, no exceptions
        Assert.AreEqual(1, completeCount, "OnDialogueComplete should fire only once despite rapid clicks");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test whitespace-only GUID in AssetReference.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_WhitespaceGUID_ShouldHandleAsInvalid()
    {
        // Arrange
        var whitespaceRef = new UnityEngine.AddressableAssets.AssetReference("   ");
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_whitespace")
            .WithSceneName("Whitespace GUID Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test")
                .Build())
            .WithNextScene(whitespaceRef)
            .Build();

        bool dialogueComplete = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueComplete = true;

        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Act
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert
        Assert.IsTrue(dialogueComplete, "Should complete gracefully with whitespace GUID");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test extremely long invalid GUID string.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_VeryLongInvalidGUID_ShouldHandleGracefully()
    {
        // Arrange
        var longGuid = new string('x', 10000); // 10k character GUID
        var longRef = new UnityEngine.AddressableAssets.AssetReference(longGuid);
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_long_guid")
            .WithSceneName("Long GUID Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test")
                .Build())
            .WithNextScene(longRef)
            .Build();

        bool dialogueComplete = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueComplete = true;

        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Act
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert
        Assert.IsTrue(dialogueComplete, "Should handle very long invalid GUID gracefully");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test special characters in GUID (attempt to inject code).
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_SpecialCharactersInGUID_ShouldNotCauseInjection()
    {
        // Arrange
        var maliciousGuid = "<script>alert('xss')</script>"; // Simulate injection attempt
        var maliciousRef = new UnityEngine.AddressableAssets.AssetReference(maliciousGuid);
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_malicious")
            .WithSceneName("Malicious GUID Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test")
                .Build())
            .WithNextScene(maliciousRef)
            .Build();

        bool dialogueComplete = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueComplete = true;

        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Act
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert
        Assert.IsTrue(dialogueComplete, "Should handle special characters safely");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test null reference passed to AddressablesAssetManager directly.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_NullKeyToAssetManager_ShouldReturnNullGracefully()
    {
        // Arrange
        var assetManager = new NovelCore.Runtime.Core.AssetManagement.AddressablesAssetManager();

        // Act
        var result = await assetManager.LoadAssetAsync<SceneData>(null);

        // Assert
        Assert.IsNull(result, "LoadAssetAsync should return null for null key");
        
        yield return null;
    }

    /// <summary>
    /// Test empty string key to AddressablesAssetManager.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_EmptyStringKeyToAssetManager_ShouldHandleGracefully()
    {
        // Arrange
        var assetManager = new NovelCore.Runtime.Core.AssetManagement.AddressablesAssetManager();

        // Act
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("String key is null or empty"));
        var result = await assetManager.LoadAssetAsync<SceneData>("");

        // Assert
        Assert.IsNull(result, "LoadAssetAsync should return null for empty string key");
        
        yield return null;
    }

    /// <summary>
    /// Test scene with multiple dialogue lines ending with invalid NextScene.
    /// </summary>
    [UnityTest]
    public IEnumerator BreakTest_MultipleDialogueLinesWithInvalidNextScene_ShouldHandleCorrectly()
    {
        // Arrange
        var invalidRef = new UnityEngine.AddressableAssets.AssetReference("");
        
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_multi")
            .WithSceneName("Multi Line Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Line 1")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_002")
                .WithFallbackText("Line 2")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_003")
                .WithFallbackText("Final line")
                .Build())
            .WithNextScene(invalidRef)
            .Build();

        int lineChangedCount = 0;
        bool dialogueComplete = false;
        
        _dialogueSystem.OnDialogueLineChanged += (line) => lineChangedCount++;
        _dialogueSystem.OnDialogueComplete += () => dialogueComplete = true;

        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Act: Advance through all lines
        _dialogueSystem.AdvanceDialogue(); // Line 2
        yield return null;
        _dialogueSystem.AdvanceDialogue(); // Line 3
        yield return null;
        
        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("NextScene AssetReference is invalid"));
        _dialogueSystem.AdvanceDialogue(); // Complete
        yield return new WaitForSeconds(0.2f);

        // Assert
        Assert.AreEqual(3, lineChangedCount, "Should fire line changed for each dialogue line");
        Assert.IsTrue(dialogueComplete, "Should complete after last line");
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    /// <summary>
    /// Test that valid scenes still work after the fix (regression test).
    /// </summary>
    [UnityTest]
    public IEnumerator RegressionTest_ValidNextScene_ShouldStillWorkCorrectly()
    {
        // Arrange: Create valid next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Valid Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Next scene dialogue")
                .Build())
            .Build();

        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        _mockAssetManager.AddAsset("scene_002", nextSceneData);

        var currentSceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Current Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Current dialogue")
                .Build())
            .WithNextScene(nextSceneRef)
            .Build();

        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        _dialogueSystem.StartScene(currentSceneData);
        yield return null;

        // Act
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.3f);

        // Assert: Valid scene loading should NOT be affected by the fix
        Assert.IsNotNull(navigatedScene, "Valid scene navigation should still work");
        Assert.AreEqual("scene_002", navigatedScene.SceneId);
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }
}

}
