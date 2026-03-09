using NUnit.Framework;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Tests.Runtime.Builders;
using UnityEngine.TestTools;

namespace NovelCore.Tests.Runtime.Data
{

/// <summary>
/// Unit tests for SceneData ScriptableObject.
/// </summary>
public class SceneDataTests : BaseTestFixture
{
    [Test]
    public void SceneData_CreatedWithValidData_IsValid()
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

        Assert.IsNotNull(sceneData);
        Assert.AreEqual("scene_001", sceneData.SceneId);
        Assert.AreEqual("Test Scene", sceneData.SceneName);
        Assert.AreEqual(1, sceneData.DialogueLines.Count);
        Assert.IsTrue(sceneData.Validate());
    }

    [Test]
    public void SceneData_WithEmptySceneId_ValidateFails()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("")
            .WithSceneName("Test Scene")
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("SceneData.*sceneId is required"));
        Assert.IsFalse(sceneData.Validate());
    }

    [Test]
    public void SceneData_WithoutDialogueOrChoices_ShowsWarning()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Empty Scene")
            .Build();

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("SceneData.*No dialogue or choices defined"));
        sceneData.Validate();
    }

    [Test]
    public void SceneData_WithTransitionType_ReturnsCorrectValue()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithTransitionType(TransitionType.Slide)
            .Build();

        Assert.AreEqual(TransitionType.Slide, sceneData.TransitionType);
    }

    [Test]
    public void SceneData_WithAutoAdvance_ReturnsCorrectValues()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithAutoAdvance(true, 3.0f)
            .Build();

        Assert.IsTrue(sceneData.AutoAdvance);
        Assert.AreEqual(3.0f, sceneData.AutoAdvanceDelay);
    }

    [Test]
    public void SceneData_WithCharacters_ReturnsCharactersList()
    {
        var character = new CharacterPlacement
        {
            position = new Vector2(0.5f, 0.5f),
            initialEmotion = "happy",
            sortingOrder = 1
        };

        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .WithCharacter(character)
            .Build();

        Assert.AreEqual(1, sceneData.Characters.Count);
        Assert.AreEqual(new Vector2(0.5f, 0.5f), sceneData.Characters[0].position);
        Assert.AreEqual("happy", sceneData.Characters[0].initialEmotion);
    }

    [Test]
    public void SceneData_DefaultTransitionDuration_IsHalfSecond()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .Build();

        Assert.AreEqual(0.5f, sceneData.TransitionDuration);
    }

    [Test]
    public void SceneData_DefaultAutoAdvance_IsFalse()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("scene_001")
            .WithSceneName("Test Scene")
            .Build();

        Assert.IsFalse(sceneData.AutoAdvance);
    }
}

}
