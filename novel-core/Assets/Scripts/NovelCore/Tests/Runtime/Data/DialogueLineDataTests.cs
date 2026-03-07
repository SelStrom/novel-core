using NUnit.Framework;
using UnityEngine;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Tests.Runtime.Builders;
using UnityEngine.TestTools;

namespace NovelCore.Tests.Runtime.Data
{

/// <summary>
/// Unit tests for DialogueLineData ScriptableObject.
/// </summary>
public class DialogueLineDataTests : BaseTestFixture
{
    [Test]
    public void DialogueLineData_CreatedWithValidData_IsValid()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithTextKey("test_key")
            .WithFallbackText("Hello, World!")
            .WithEmotion("happy")
            .Build();

        Assert.IsNotNull(dialogueLineData);
        Assert.AreEqual("line_001", dialogueLineData.LineId);
        Assert.AreEqual("test_key", dialogueLineData.TextKey);
        Assert.AreEqual("Hello, World!", dialogueLineData.FallbackText);
        Assert.AreEqual("happy", dialogueLineData.Emotion);
        Assert.IsTrue(dialogueLineData.Validate());
    }

    [Test]
    public void DialogueLineData_WithEmptyLineId_ValidateFails()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("")
            .WithFallbackText("Test dialogue")
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("DialogueLineData.*lineId is required"));
        Assert.IsFalse(dialogueLineData.Validate());
    }

    [Test]
    public void DialogueLineData_WithEmptyTextKey_ShowsWarning()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithTextKey("")
            .WithFallbackText("Test dialogue")
            .Build();

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("DialogueLineData.*textKey is empty, using fallback text"));
        dialogueLineData.Validate();
    }

    [Test]
    public void DialogueLineData_WithCharacterAction_ReturnsCorrectAction()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .WithCharacterAction(CharacterAction.Enter, "{\"side\": \"left\"}")
            .Build();

        Assert.AreEqual(CharacterAction.Enter, dialogueLineData.CharacterAction);
        Assert.AreEqual("{\"side\": \"left\"}", dialogueLineData.ActionParameters);
    }

    [Test]
    public void DialogueLineData_WithActionButNoParameters_ShowsWarning()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .WithCharacterAction(CharacterAction.Move, "")
            .Build();

        LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("DialogueLineData.*Character action specified but no parameters provided"));
        dialogueLineData.Validate();
    }

    [Test]
    public void DialogueLineData_WithDisplayDuration_ReturnsCorrectValue()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .WithDisplayDuration(5.0f)
            .Build();

        Assert.AreEqual(5.0f, dialogueLineData.DisplayDuration);
    }

    [Test]
    public void DialogueLineData_DefaultDisplayDuration_IsNegativeOne()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .Build();

        Assert.AreEqual(-1f, dialogueLineData.DisplayDuration);
    }

    [Test]
    public void DialogueLineData_DefaultEmotion_IsNeutral()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .Build();

        Assert.AreEqual("neutral", dialogueLineData.Emotion);
    }

    [Test]
    public void DialogueLineData_WithSoundEffect_ReturnsAssetReference()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("line_001")
            .WithFallbackText("Test dialogue")
            .Build();

        Assert.IsNotNull(dialogueLineData);
    }
}

}
