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
/// PlayMode tests for DialogueSystem nextScene transition functionality.
/// Tests US1 (Linear Scene Progression) requirement.
/// </summary>
public class DialogueSystemNextSceneTests : BaseTestFixture
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

    [UnityTest]
    public IEnumerator NextScene_WhenDialogueCompletes_FiresNavigationEvent()
    {
        // Arrange: Create next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Next scene dialogue")
                .Build())
            .Build();

        // Create AssetReference for next scene (mock)
        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        
        // Register next scene in mock asset manager using RuntimeKey
        // Note: In tests, AssetReference.RuntimeKey returns the GUID passed to constructor
        _mockAssetManager.AddAsset(nextSceneRef.RuntimeKey?.ToString() ?? "scene_002", nextSceneData);

        // Create current scene with nextScene reference
        var currentSceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Current Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test dialogue")
                .Build())
            .WithNextScene(nextSceneRef)
            .Build();

        // Track navigation event
        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        // Act: Start scene and advance to completion
        _dialogueSystem.StartScene(currentSceneData);
        yield return null; // Wait for initialization

        _dialogueSystem.AdvanceDialogue(); // Complete dialogue
        yield return new WaitForSeconds(0.2f); // Wait for async navigation

        // Assert: Navigation event fired with next scene
        Assert.IsNotNull(navigatedScene, "OnSceneNavigationRequested should fire when nextScene is set");
        Assert.AreEqual("scene_002", navigatedScene.SceneId, "Navigation should load the next scene");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop playing after navigation");
    }

    [UnityTest]
    public IEnumerator NextScene_WhenNoNextScene_CompletesWithoutNavigation()
    {
        // Arrange: Create scene without nextScene
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Final Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Final dialogue")
                .Build())
            .Build(); // No nextScene set

        bool navigationFired = false;
        bool dialogueCompleteFired = false;

        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act: Start scene and advance to completion
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.1f);

        // Assert: Only OnDialogueComplete fires, no navigation
        Assert.IsFalse(navigationFired, "OnSceneNavigationRequested should NOT fire when nextScene is null");
        Assert.IsTrue(dialogueCompleteFired, "OnDialogueComplete should fire when no nextScene");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop playing");
    }

    [UnityTest]
    public IEnumerator NextScene_WhenLoadFails_CompletesDialogueGracefully()
    {
        // Arrange: Create scene with invalid nextScene reference
        var invalidSceneRef = new UnityEngine.AddressableAssets.AssetReference("invalid_scene_id");

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Current Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Test dialogue")
                .Build())
            .WithNextScene(invalidSceneRef)
            .Build();

        // Don't register the scene in mock asset manager - simulate load failure

        bool navigationFired = false;
        bool dialogueCompleteFired = false;

        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act: Start scene and advance to completion
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Failed to load target scene"));
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert: Graceful handling - no navigation, dialogue completes
        Assert.IsFalse(navigationFired, "OnSceneNavigationRequested should NOT fire when load fails");
        Assert.IsTrue(dialogueCompleteFired, "OnDialogueComplete should fire on load failure");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop playing");
    }

    [UnityTest]
    public IEnumerator NextScene_WithMultipleDialogueLines_TransitionsAfterAll()
    {
        // Arrange: Create next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Next scene")
                .Build())
            .Build();

        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        _mockAssetManager.AddAsset(nextSceneRef.RuntimeKey?.ToString() ?? "scene_002", nextSceneData);

        // Create scene with 3 dialogue lines
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Current Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("First line")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_002")
                .WithFallbackText("Second line")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_003")
                .WithFallbackText("Third line")
                .Build())
            .WithNextScene(nextSceneRef)
            .Build();

        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        // Act: Start scene and advance through all lines
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        _dialogueSystem.AdvanceDialogue(); // Line 1 -> 2
        yield return null;
        Assert.IsNull(navigatedScene, "Should not navigate after line 1");

        _dialogueSystem.AdvanceDialogue(); // Line 2 -> 3
        yield return null;
        Assert.IsNull(navigatedScene, "Should not navigate after line 2");

        _dialogueSystem.AdvanceDialogue(); // Line 3 -> complete -> navigate
        yield return new WaitForSeconds(0.2f);

        // Assert: Navigation happens only after ALL lines complete
        Assert.IsNotNull(navigatedScene, "Should navigate after final line");
        Assert.AreEqual("scene_002", navigatedScene.SceneId);
    }
}

}
