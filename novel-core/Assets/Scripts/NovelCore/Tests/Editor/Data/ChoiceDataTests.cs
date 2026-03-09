using NUnit.Framework;
using UnityEngine;
using NovelCore.Runtime.Data.Choices;
using NovelCore.Tests.Editor.Builders;
using UnityEngine.TestTools;
using System.Collections.Generic;

namespace NovelCore.Tests.Editor.Data
{

/// <summary>
/// Builder for creating test ChoiceData instances.
/// </summary>
public class ChoiceDataBuilder
{
    private string _choiceId = "test_choice";
    private string _promptTextKey = "";
    private string _fallbackPromptText = "Choose an option:";
    private List<ChoiceOption> _options = new();
    private float _timerSeconds = 0f;
    private int _defaultOptionIndex = 0;

    public ChoiceDataBuilder WithChoiceId(string choiceId)
    {
        _choiceId = choiceId;
        return this;
    }

    public ChoiceDataBuilder WithPromptTextKey(string promptTextKey)
    {
        _promptTextKey = promptTextKey;
        return this;
    }

    public ChoiceDataBuilder WithFallbackPromptText(string fallbackPromptText)
    {
        _fallbackPromptText = fallbackPromptText;
        return this;
    }

    public ChoiceDataBuilder WithOption(ChoiceOption option)
    {
        _options.Add(option);
        return this;
    }

    public ChoiceDataBuilder WithTimer(float timerSeconds, int defaultOptionIndex)
    {
        _timerSeconds = timerSeconds;
        _defaultOptionIndex = defaultOptionIndex;
        return this;
    }

    public ChoiceData Build()
    {
        var choiceData = ScriptableObject.CreateInstance<ChoiceData>();
        
        typeof(ChoiceData).GetField("_choiceId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _choiceId);
        typeof(ChoiceData).GetField("_promptTextKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _promptTextKey);
        typeof(ChoiceData).GetField("_fallbackPromptText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _fallbackPromptText);
        typeof(ChoiceData).GetField("_options", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _options);
        typeof(ChoiceData).GetField("_timerSeconds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _timerSeconds);
        typeof(ChoiceData).GetField("_defaultOptionIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(choiceData, _defaultOptionIndex);

        return choiceData;
    }
}

/// <summary>
/// Unit tests for ChoiceData ScriptableObject.
/// </summary>
public class ChoiceDataTests : BaseTestFixture
{
    [Test]
    public void ChoiceData_CreatedWithTwoOptions_IsValid()
    {
        var option1 = new ChoiceOption
        {
            optionId = "option_1",
            fallbackText = "Option 1",
            isAvailable = true
        };

        var option2 = new ChoiceOption
        {
            optionId = "option_2",
            fallbackText = "Option 2",
            isAvailable = true
        };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithFallbackPromptText("Choose wisely:")
            .WithOption(option1)
            .WithOption(option2)
            .Build();

        Assert.IsNotNull(choiceData);
        Assert.AreEqual("choice_001", choiceData.ChoiceId);
        Assert.AreEqual(2, choiceData.Options.Count);
        Assert.IsTrue(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_WithEmptyChoiceId_ValidateFails()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };
        var option2 = new ChoiceOption { optionId = "option_2", fallbackText = "Option 2" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("")
            .WithOption(option1)
            .WithOption(option2)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("ChoiceData.*choiceId is required"));
        Assert.IsFalse(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_WithOneOption_ValidateFails()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithOption(option1)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("ChoiceData.*Must have between 2 and 6 options"));
        Assert.IsFalse(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_WithSevenOptions_ValidateFails()
    {
        var choiceDataBuilder = new ChoiceDataBuilder()
            .WithChoiceId("choice_001");

        for (int i = 1; i <= 7; i++)
        {
            choiceDataBuilder.WithOption(new ChoiceOption
            {
                optionId = $"option_{i}",
                fallbackText = $"Option {i}"
            });
        }

        var choiceData = choiceDataBuilder.Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("ChoiceData.*Must have between 2 and 6 options"));
        Assert.IsFalse(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_WithDuplicateOptionIds_ValidateFails()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };
        var option2 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 2" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithOption(option1)
            .WithOption(option2)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("ChoiceData.*Duplicate option IDs found"));
        Assert.IsFalse(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_WithTimer_StoresTimerValues()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };
        var option2 = new ChoiceOption { optionId = "option_2", fallbackText = "Option 2" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithOption(option1)
            .WithOption(option2)
            .WithTimer(10.0f, 1)
            .Build();

        Assert.AreEqual(10.0f, choiceData.TimerSeconds);
        Assert.AreEqual(1, choiceData.DefaultOptionIndex);
    }

    [Test]
    public void ChoiceData_WithTimerAndInvalidDefaultIndex_ValidateFails()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };
        var option2 = new ChoiceOption { optionId = "option_2", fallbackText = "Option 2" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithOption(option1)
            .WithOption(option2)
            .WithTimer(10.0f, 5)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("ChoiceData.*Invalid defaultOptionIndex for timed choice"));
        Assert.IsFalse(choiceData.Validate());
    }

    [Test]
    public void ChoiceData_DefaultTimerSeconds_IsZero()
    {
        var option1 = new ChoiceOption { optionId = "option_1", fallbackText = "Option 1" };
        var option2 = new ChoiceOption { optionId = "option_2", fallbackText = "Option 2" };

        var choiceData = new ChoiceDataBuilder()
            .WithChoiceId("choice_001")
            .WithOption(option1)
            .WithOption(option2)
            .Build();

        Assert.AreEqual(0f, choiceData.TimerSeconds);
    }
}

}
