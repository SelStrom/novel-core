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
/// PlayMode tests for DialogueSystem auto-advance with nextScene transition.
/// Tests US1 requirement: automatic progression should work with auto-advance enabled.
/// </summary>
public class DialogueSystemAutoAdvanceNextSceneTests : BaseTestFixture
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
    public IEnumerator AutoAdvance_WithNextScene_TransitionsAutomatically()
    {
        // Arrange: Create next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Auto-advanced to next scene")
                .Build())
            .Build();

        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        _mockAssetManager.AddAsset(nextSceneRef.RuntimeKey?.ToString() ?? "scene_002", nextSceneData);

        // Create scene with auto-advance enabled (short delay for testing)
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Auto Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("This will auto-advance")
                .Build())
            .WithAutoAdvance(true, delay: 0.5f) // 500ms delay
            .WithNextScene(nextSceneRef)
            .Build();

        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        // Act: Start scene and simulate auto-advance via Update()
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Simulate time passing for auto-advance (0.5s delay + buffer)
        float elapsed = 0f;
        while (elapsed < 1.0f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Wait for async navigation to complete
        yield return new WaitForSeconds(0.2f);

        // Assert: Scene auto-transitioned to next scene
        Assert.IsNotNull(navigatedScene, "Auto-advance should trigger nextScene navigation");
        Assert.AreEqual("scene_002", navigatedScene.SceneId, "Should navigate to correct next scene");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop after auto-navigation");
    }

    [UnityTest]
    public IEnumerator AutoAdvance_WithoutNextScene_CompletesDialogue()
    {
        // Arrange: Create scene with auto-advance but NO nextScene
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Auto Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("Auto-advance without next scene")
                .Build())
            .WithAutoAdvance(true, delay: 0.3f)
            .Build(); // No nextScene

        bool navigationFired = false;
        bool dialogueCompleteFired = false;

        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigationFired = true;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act: Start scene and simulate auto-advance via Update()
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Simulate time passing for auto-advance
        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Assert: Dialogue completes without navigation
        Assert.IsFalse(navigationFired, "Should not navigate when no nextScene");
        Assert.IsTrue(dialogueCompleteFired, "OnDialogueComplete should fire");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop");
    }

    [UnityTest]
    public IEnumerator AutoAdvance_MultipleLines_TransitionsAfterLastLine()
    {
        // Arrange: Create next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Next")
                .Build())
            .Build();

        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        _mockAssetManager.AddAsset(nextSceneRef.RuntimeKey?.ToString() ?? "scene_002", nextSceneData);

        // Create scene with 2 dialogue lines and auto-advance
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Multi-line Auto Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("First line")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_002")
                .WithFallbackText("Second line")
                .Build())
            .WithAutoAdvance(true, delay: 0.4f)
            .WithNextScene(nextSceneRef)
            .Build();

        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        // Act: Start scene and simulate auto-advance through both lines
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Simulate first auto-advance (line 1 -> line 2)
        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Assert.IsNull(navigatedScene, "Should not navigate after first line");
        Assert.IsTrue(_dialogueSystem.IsPlaying, "Should still be playing second line");

        // Simulate second auto-advance (line 2 -> complete -> navigate)
        elapsed = 0f;
        while (elapsed < 0.6f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Wait for async navigation
        yield return new WaitForSeconds(0.2f);

        // Assert: Navigation happens after last line
        Assert.IsNotNull(navigatedScene, "Should navigate after all lines complete");
        Assert.AreEqual("scene_002", navigatedScene.SceneId);
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop");
    }

    [UnityTest]
    public IEnumerator AutoAdvance_DisabledMidScene_StopsBeforeNextScene()
    {
        // Arrange: Create next scene
        var nextSceneData = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Next Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_next")
                .WithFallbackText("Next")
                .Build())
            .Build();

        var nextSceneRef = new UnityEngine.AddressableAssets.AssetReference("scene_002");
        _mockAssetManager.AddAsset(nextSceneRef.RuntimeKey?.ToString() ?? "scene_002", nextSceneData);

        // Create scene with auto-advance
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Cancellable Auto Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_001")
                .WithFallbackText("This should stop")
                .Build())
            .WithAutoAdvance(true, delay: 1.0f) // Longer delay to allow cancellation
            .WithNextScene(nextSceneRef)
            .Build();

        SceneData navigatedScene = null;
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScene = scene;

        // Act: Start scene, then stop dialogue before auto-advance triggers
        _dialogueSystem.StartScene(sceneData);
        yield return null;

        // Simulate partial auto-advance time (less than full delay)
        float elapsed = 0f;
        while (elapsed < 0.3f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _dialogueSystem.StopDialogue(); // Cancel auto-advance

        // Simulate more time passing (auto-advance should NOT fire now)
        elapsed = 0f;
        while (elapsed < 1.0f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Assert: No navigation occurred (auto-advance was cancelled)
        Assert.IsNull(navigatedScene, "Navigation should not occur after StopDialogue");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should be stopped");
    }
}

}
