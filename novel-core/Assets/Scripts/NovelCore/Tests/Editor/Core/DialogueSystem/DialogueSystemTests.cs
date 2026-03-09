using NUnit.Framework;
using UnityEngine;
using DialogueSystemImpl = NovelCore.Runtime.Core.DialogueSystem.DialogueSystem;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;
using NovelCore.Tests.Editor.Builders;
using NovelCore.Tests.Editor.Core;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace NovelCore.Tests.Editor.Core.DialogueSystem
{

/// <summary>
/// Unit tests for DialogueSystem initialization and state management.
/// </summary>
public class DialogueSystemTests : BaseTestFixture
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

    [Test]
    public void DialogueSystem_Constructor_ThrowsOnNullAudioService()
    {
        Assert.Throws<System.ArgumentNullException>(() =>
        {
            new DialogueSystemImpl(null, _mockInputService, _mockSceneManager, _mockAssetManager);
        });
    }

    [Test]
    public void DialogueSystem_Constructor_ThrowsOnNullInputService()
    {
        Assert.Throws<System.ArgumentNullException>(() =>
        {
            new DialogueSystemImpl(_mockAudioService, null, _mockSceneManager, _mockAssetManager);
        });
    }

    [Test]
    public void DialogueSystem_Constructor_ThrowsOnNullSceneManager()
    {
        Assert.Throws<System.ArgumentNullException>(() =>
        {
            new DialogueSystemImpl(_mockAudioService, _mockInputService, null, _mockAssetManager);
        });
    }

    [Test]
    public void DialogueSystem_Constructor_ThrowsOnNullAssetManager()
    {
        Assert.Throws<System.ArgumentNullException>(() =>
        {
            new DialogueSystemImpl(_mockAudioService, _mockInputService, _mockSceneManager, null);
        });
    }

    [Test]
    public void DialogueSystem_StartScene_WithValidScene_SetsPlayingState()
    {
        var dialogueLine = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine)
            .Build();

        _dialogueSystem.StartScene(sceneData);

        Assert.IsTrue(_dialogueSystem.IsPlaying);
        Assert.AreEqual(sceneData, _dialogueSystem.CurrentScene);
        Assert.AreEqual(0, _dialogueSystem.CurrentLineIndex);
    }

    [Test]
    public void DialogueSystem_StartScene_WithNullScene_LogsError()
    {
        LogAssert.Expect(LogType.Error, "DialogueSystem: Cannot start null scene");
        _dialogueSystem.StartScene(null);

        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    [Test]
    public void DialogueSystem_StartScene_WithInvalidScene_LogsError()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("")
            .WithSceneName("Invalid Scene")
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("SceneData.*sceneId is required"));
        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("DialogueSystem.*Scene validation failed"));
        _dialogueSystem.StartScene(sceneData);

        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    [Test]
    public void DialogueSystem_AdvanceDialogue_IncrementsLineIndex()
    {
        var dialogueLine1 = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("First line")
            .Build();

        var dialogueLine2 = new DialogueLineDataBuilder()
            .WithLineId("line_002")
            .WithFallbackText("Second line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine1)
            .WithDialogueLine(dialogueLine2)
            .Build();

        _dialogueSystem.StartScene(sceneData);
        Assert.AreEqual(0, _dialogueSystem.CurrentLineIndex);

        _dialogueSystem.AdvanceDialogue();
        Assert.AreEqual(1, _dialogueSystem.CurrentLineIndex);
    }

    [Test]
    public void DialogueSystem_AdvanceDialogue_AtLastLine_CompletesDialogue()
    {
        bool dialogueCompleted = false;
        _dialogueSystem.OnDialogueComplete += () => dialogueCompleted = true;

        var dialogueLine = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Only line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine)
            .Build();

        _dialogueSystem.StartScene(sceneData);
        _dialogueSystem.AdvanceDialogue();

        Assert.IsTrue(dialogueCompleted);
        Assert.IsFalse(_dialogueSystem.IsPlaying);
    }

    [Test]
    public void DialogueSystem_GetCurrentLine_ReturnsCorrectLine()
    {
        var dialogueLine = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine)
            .Build();

        _dialogueSystem.StartScene(sceneData);
        var currentLine = _dialogueSystem.GetCurrentLine();

        Assert.IsNotNull(currentLine);
        Assert.AreEqual("line_001", currentLine.LineId);
        Assert.AreEqual("Test line", currentLine.FallbackText);
    }

    [Test]
    public void DialogueSystem_HasNextLine_WithMultipleLines_ReturnsTrue()
    {
        var dialogueLine1 = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("First line")
            .Build();

        var dialogueLine2 = new DialogueLineDataBuilder()
            .WithLineId("line_002")
            .WithFallbackText("Second line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine1)
            .WithDialogueLine(dialogueLine2)
            .Build();

        _dialogueSystem.StartScene(sceneData);

        Assert.IsTrue(_dialogueSystem.HasNextLine());
    }

    [Test]
    public void DialogueSystem_HasNextLine_AtLastLine_ReturnsFalse()
    {
        var dialogueLine = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Only line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine)
            .Build();

        _dialogueSystem.StartScene(sceneData);

        Assert.IsFalse(_dialogueSystem.HasNextLine());
    }

    [Test]
    public void DialogueSystem_StopDialogue_ResetsState()
    {
        var dialogueLine = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine)
            .Build();

        _dialogueSystem.StartScene(sceneData);
        Assert.IsTrue(_dialogueSystem.IsPlaying);

        _dialogueSystem.StopDialogue();

        Assert.IsFalse(_dialogueSystem.IsPlaying);
        Assert.IsNull(_dialogueSystem.CurrentScene);
        Assert.AreEqual(0, _dialogueSystem.CurrentLineIndex);
    }

    [Test]
    public void DialogueSystem_SkipCurrentLine_CallsAdvanceDialogue()
    {
        var dialogueLine1 = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("First line")
            .Build();

        var dialogueLine2 = new DialogueLineDataBuilder()
            .WithLineId("line_002")
            .WithFallbackText("Second line")
            .Build();

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithDialogueLine(dialogueLine1)
            .WithDialogueLine(dialogueLine2)
            .Build();

        _dialogueSystem.StartScene(sceneData);
        Assert.AreEqual(0, _dialogueSystem.CurrentLineIndex);

        _dialogueSystem.SkipCurrentLine();

        Assert.AreEqual(1, _dialogueSystem.CurrentLineIndex);
    }
}

}
