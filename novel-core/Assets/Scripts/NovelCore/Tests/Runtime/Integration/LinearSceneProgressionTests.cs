using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using DialogueSystemImpl = NovelCore.Runtime.Core.DialogueSystem.DialogueSystem;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Tests.Runtime.Builders;
using NovelCore.Tests.Runtime.Core;

namespace NovelCore.Tests.Runtime.Integration
{

/// <summary>
/// End-to-end integration tests for linear scene progression.
/// Tests complete flow: Scene1 -> Scene2 -> Scene3 via nextScene.
/// </summary>
public class LinearSceneProgressionTests : BaseTestFixture
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
    public IEnumerator ThreeSceneLinearFlow_ProgressesCorrectly()
    {
        // Arrange: Create 3-scene linear story
        var scene3 = new SceneDataBuilder()
            .WithSceneId("scene_003")
            .WithSceneName("Ending")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_3_1")
                .WithFallbackText("The end.")
                .Build())
            .Build(); // No nextScene - this is the end

        var scene3Ref = new UnityEngine.AddressableAssets.AssetReference("scene_003");

        var scene2 = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Middle")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_2_1")
                .WithFallbackText("Middle of story")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_2_2")
                .WithFallbackText("Almost done")
                .Build())
            .WithNextScene(scene3Ref)
            .Build();

        var scene2Ref = new UnityEngine.AddressableAssets.AssetReference("scene_002");

        var scene1 = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Introduction")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_1_1")
                .WithFallbackText("Welcome to the story")
                .Build())
            .WithNextScene(scene2Ref)
            .Build();

        // Register all scenes in asset manager using RuntimeKey
        _mockAssetManager.AddAsset(scene2Ref.RuntimeKey?.ToString() ?? "scene_002", scene2);
        _mockAssetManager.AddAsset(scene3Ref.RuntimeKey?.ToString() ?? "scene_003", scene3);

        // Track navigation history
        var navigatedScenes = new List<string>();
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScenes.Add(scene.SceneId);

        bool finalDialogueCompleted = false;
        _dialogueSystem.OnDialogueComplete += () => finalDialogueCompleted = true;

        // Act & Assert: Scene 1
        _dialogueSystem.StartScene(scene1);
        yield return null;

        Assert.AreEqual("scene_001", _dialogueSystem.CurrentScene.SceneId);
        Assert.IsTrue(_dialogueSystem.IsPlaying);

        _dialogueSystem.AdvanceDialogue(); // Complete scene 1
        yield return new WaitForSeconds(0.2f);

        // Verify transition to Scene 2
        Assert.AreEqual(1, navigatedScenes.Count, "Should navigate to scene 2");
        Assert.AreEqual("scene_002", navigatedScenes[0]);

        // Simulate scene manager loading scene 2
        _dialogueSystem.StartScene(scene2);
        yield return null;

        Assert.AreEqual("scene_002", _dialogueSystem.CurrentScene.SceneId);
        Assert.AreEqual(0, _dialogueSystem.CurrentLineIndex);

        _dialogueSystem.AdvanceDialogue(); // Line 1 -> 2
        yield return null;

        _dialogueSystem.AdvanceDialogue(); // Line 2 -> complete
        yield return new WaitForSeconds(0.2f);

        // Verify transition to Scene 3
        Assert.AreEqual(2, navigatedScenes.Count, "Should navigate to scene 3");
        Assert.AreEqual("scene_003", navigatedScenes[1]);

        // Simulate scene manager loading scene 3
        _dialogueSystem.StartScene(scene3);
        yield return null;

        Assert.AreEqual("scene_003", _dialogueSystem.CurrentScene.SceneId);

        _dialogueSystem.AdvanceDialogue(); // Complete final scene
        yield return new WaitForSeconds(0.1f);

        // Verify story completes (no more navigation)
        Assert.AreEqual(2, navigatedScenes.Count, "Should not navigate beyond scene 3");
        Assert.IsTrue(finalDialogueCompleted, "Final scene should trigger OnDialogueComplete");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Story should be complete");
    }

    [UnityTest]
    public IEnumerator LinearFlow_WithAutoAdvance_ProgressesWithoutInput()
    {
        // Arrange: Create 2-scene linear story with auto-advance
        var scene2 = new SceneDataBuilder()
            .WithSceneId("scene_002")
            .WithSceneName("Auto Scene 2")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_2_1")
                .WithFallbackText("Auto second scene")
                .Build())
            .WithAutoAdvance(true, delay: 0.3f)
            .Build(); // No nextScene - end here

        var scene2Ref = new UnityEngine.AddressableAssets.AssetReference("scene_002");

        var scene1 = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Auto Scene 1")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_1_1")
                .WithFallbackText("Auto first scene")
                .Build())
            .WithAutoAdvance(true, delay: 0.3f)
            .WithNextScene(scene2Ref)
            .Build();

        _mockAssetManager.AddAsset(scene2Ref.RuntimeKey?.ToString() ?? "scene_002", scene2);

        var navigatedScenes = new List<string>();
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScenes.Add(scene.SceneId);

        // Act: Start scene 1 and simulate auto-progression
        _dialogueSystem.StartScene(scene1);
        yield return null;

        // Simulate auto-advance for scene 1 (0.3s delay + buffer)
        float elapsed = 0f;
        while (elapsed < 0.8f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Wait for async navigation
        yield return new WaitForSeconds(0.1f);

        // Assert: Automatically navigated to scene 2
        Assert.AreEqual(1, navigatedScenes.Count, "Should auto-navigate to scene 2");
        Assert.AreEqual("scene_002", navigatedScenes[0]);

        // Simulate scene manager loading scene 2
        _dialogueSystem.StartScene(scene2);
        yield return null;

        // Simulate auto-advance for scene 2
        elapsed = 0f;
        while (elapsed < 0.6f)
        {
            _dialogueSystem.Update(Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Verify no further navigation (scene 2 has no nextScene)
        Assert.AreEqual(1, navigatedScenes.Count, "Should not navigate beyond scene 2");
    }

    [UnityTest]
    public IEnumerator LinearFlow_LongChain_HandlesMultipleTransitions()
    {
        // Arrange: Create 5-scene linear chain
        var scenes = new SceneData[5];
        var sceneRefs = new UnityEngine.AddressableAssets.AssetReference[5];

        // Build scenes from end to start (so we can reference nextScene)
        for (int i = 4; i >= 0; i--)
        {
            var sceneId = $"scene_{i:D3}";
            sceneRefs[i] = new UnityEngine.AddressableAssets.AssetReference(sceneId);

            var builder = new SceneDataBuilder()
                .WithSceneId(sceneId)
                .WithSceneName($"Scene {i + 1}")
                .WithDialogueLine(new DialogueLineDataBuilder()
                    .WithLineId($"line_{i}_1")
                    .WithFallbackText($"Scene {i + 1} dialogue")
                    .Build());

            // Add nextScene reference (except for last scene)
            if (i < 4)
            {
                builder.WithNextScene(sceneRefs[i + 1]);
            }

            scenes[i] = builder.Build();
            // Register using RuntimeKey for proper AssetReference resolution
            _mockAssetManager.AddAsset(sceneRefs[i].RuntimeKey?.ToString() ?? sceneId, scenes[i]);
        }

        var navigatedScenes = new List<string>();
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScenes.Add(scene.SceneId);

        // Act: Progress through all 5 scenes
        for (int i = 0; i < 5; i++)
        {
            _dialogueSystem.StartScene(scenes[i]);
            yield return null;

            Assert.AreEqual($"scene_{i:D3}", _dialogueSystem.CurrentScene.SceneId, $"Should be on scene {i}");

            _dialogueSystem.AdvanceDialogue();
            yield return new WaitForSeconds(0.2f);
        }

        // Assert: Navigated through all scenes correctly
        Assert.AreEqual(4, navigatedScenes.Count, "Should navigate 4 times (scene 0 -> 1 -> 2 -> 3 -> 4)");
        
        for (int i = 0; i < 4; i++)
        {
            Assert.AreEqual($"scene_{i + 1:D3}", navigatedScenes[i], $"Navigation {i} should be to scene {i + 1}");
        }
    }

    [UnityTest]
    public IEnumerator LinearFlow_WithMissingScene_StopsGracefully()
    {
        // Arrange: Create scene chain with broken link
        var scene2Ref = new UnityEngine.AddressableAssets.AssetReference("scene_002");

        var scene1 = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Scene 1")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_1_1")
                .WithFallbackText("First scene")
                .Build())
            .WithNextScene(scene2Ref) // References scene_002, but we won't register it
            .Build();

        _mockAssetManager.AddAsset("scene_001", scene1);
        // Intentionally NOT registering scene_002 to simulate load failure

        var navigatedScenes = new List<string>();
        _dialogueSystem.OnSceneNavigationRequested += (scene) => navigatedScenes.Add(scene.SceneId);

        bool dialogueCompleteFired = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleteFired = true;

        // Act: Start scene 1 and try to progress
        _dialogueSystem.StartScene(scene1);
        yield return null;

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("Failed to load target scene"));
        _dialogueSystem.AdvanceDialogue();
        yield return new WaitForSeconds(0.2f);

        // Assert: Graceful failure - no navigation, dialogue completes
        Assert.AreEqual(0, navigatedScenes.Count, "Should not navigate when scene load fails");
        Assert.IsTrue(dialogueCompleteFired, "Should fire OnDialogueComplete on failure");
        Assert.IsFalse(_dialogueSystem.IsPlaying, "Dialogue should stop");
    }
}

}
