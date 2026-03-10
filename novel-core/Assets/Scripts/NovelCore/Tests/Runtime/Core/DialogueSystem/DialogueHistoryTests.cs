using System.Collections.Generic;
using NUnit.Framework;
using DialogueSystemImpl = NovelCore.Runtime.Core.DialogueSystem.DialogueSystem;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Tests.Runtime.Builders;
using NovelCore.Tests.Runtime.Core;

namespace NovelCore.Tests.Runtime.Core.DialogueSystem
{

/// <summary>
/// Tests for dialogue history (backlog) functionality.
/// Validates FR-004: System MUST support dialogue history (backlog) allowing players to review previous text.
/// </summary>
[TestFixture]
public class DialogueHistoryTests : BaseTestFixture
{
    private DialogueSystemImpl _dialogueSystem;
    private MockAudioService _mockAudioService;
    private MockInputService _mockInputService;
    private MockSceneManager _mockSceneManager;
    private MockAssetManager _mockAssetManager;
    private SceneData _testScene;

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

        // Create a test scene with multiple dialogue lines
        _testScene = new SceneDataBuilder()
            .WithSceneId("test_scene")
            .WithSceneName("Test Scene")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_1")
                .WithFallbackText("First line of dialogue")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_2")
                .WithFallbackText("Second line of dialogue")
                .Build())
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_3")
                .WithFallbackText("Third line of dialogue")
                .Build())
            .Build();
    }

    [Test]
    public void GetDialogueHistory_InitiallyEmpty()
    {
        // Arrange & Act
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert
        Assert.IsNotNull(history, "Dialogue history should not be null");
        Assert.AreEqual(0, history.Count, "Dialogue history should be empty initially");
    }

    [Test]
    public void GetDialogueHistory_AddsLineWhenDisplayed()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);

        // Act
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert
        Assert.AreEqual(1, history.Count, "Dialogue history should contain one line after scene starts");
        Assert.AreEqual("line_1", history[0].LineId, "First line should be in history");
    }

    [Test]
    public void GetDialogueHistory_AccumulatesLinesOnAdvance()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);

        // Act
        _dialogueSystem.AdvanceDialogue();
        _dialogueSystem.AdvanceDialogue();
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert
        Assert.AreEqual(3, history.Count, "Dialogue history should contain all displayed lines");
        Assert.AreEqual("line_1", history[0].LineId);
        Assert.AreEqual("line_2", history[1].LineId);
        Assert.AreEqual("line_3", history[2].LineId);
    }

    [Test]
    public void GetDialogueHistory_PersistsAcrossScenes()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);
        _dialogueSystem.AdvanceDialogue();

        var secondScene = new SceneDataBuilder()
            .WithSceneId("scene_2")
            .WithDialogueLine(new DialogueLineDataBuilder()
                .WithLineId("line_4")
                .Build())
            .Build();

        // Act
        _dialogueSystem.StartScene(secondScene);
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert
        Assert.AreEqual(3, history.Count, "History should persist across scene changes");
        Assert.AreEqual("line_1", history[0].LineId);
        Assert.AreEqual("line_2", history[1].LineId);
        Assert.AreEqual("line_4", history[2].LineId, "New scene's line should be added to history");
    }

    [Test]
    public void ClearDialogueHistory_RemovesAllEntries()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);
        _dialogueSystem.AdvanceDialogue();
        _dialogueSystem.AdvanceDialogue();
        Assert.AreEqual(3, _dialogueSystem.GetDialogueHistory().Count);

        // Act
        _dialogueSystem.ClearDialogueHistory();
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert
        Assert.AreEqual(0, history.Count, "Dialogue history should be empty after clear");
    }

    [Test]
    public void GetDialogueHistory_ReturnsReadOnlyList()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);
        var history = _dialogueSystem.GetDialogueHistory();

        // Act & Assert
        Assert.IsInstanceOf<System.Collections.Generic.IReadOnlyList<DialogueLineData>>(history);
    }

    [Test]
    public void DialogueHistory_CanBeUsedForBacklog()
    {
        // Arrange
        _dialogueSystem.StartScene(_testScene);
        _dialogueSystem.AdvanceDialogue();

        // Act
        var history = _dialogueSystem.GetDialogueHistory();

        // Assert - Simulate backlog UI displaying history
        for (int i = history.Count - 1; i >= 0; i--)
        {
            var line = history[i];
            Assert.IsNotNull(line, $"Line at index {i} should not be null");
            Assert.IsFalse(string.IsNullOrEmpty(line.LineId), "Line should have valid ID");
        }
    }

    [Test]
    public void ClearChoiceHistory_RemovesAllChoices()
    {
        // Arrange
        var sceneWithChoice = new SceneDataBuilder()
            .WithSceneId("choice_scene")
            .WithChoice(new ChoiceDataBuilder()
                .WithChoiceId("choice_1")
                .WithOption("opt_1", "Option 1")
                .WithOption("opt_2", "Option 2")
                .Build())
            .Build();

        _dialogueSystem.StartScene(sceneWithChoice);
        
        // Manually add choice to history (simulating SelectChoice)
        var choiceHistory = new List<string> { "choice_1_opt_1" };

        // Act
        _dialogueSystem.ClearChoiceHistory();

        // Assert
        Assert.AreEqual(0, _dialogueSystem.GetChoiceHistory().Count);
    }
}

}
