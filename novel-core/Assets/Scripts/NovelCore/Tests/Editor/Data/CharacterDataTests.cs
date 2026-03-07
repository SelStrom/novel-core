using NUnit.Framework;
using UnityEngine;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Tests.Editor.Builders;
using UnityEngine.TestTools;

namespace NovelCore.Tests.Editor.Data
{

/// <summary>
/// Unit tests for CharacterData ScriptableObject.
/// </summary>
public class CharacterDataTests : BaseTestFixture
{
    [Test]
    public void CharacterData_CreatedWithValidData_IsValid()
    {
        var emotion = new CharacterEmotion
        {
            emotionName = "neutral",
            spineSkin = "default",
            spineAnimation = "idle"
        };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("neutral")
            .WithEmotion(emotion)
            .Build();

        Assert.IsNotNull(characterData);
        Assert.AreEqual("char_001", characterData.CharacterId);
        Assert.AreEqual("Test Character", characterData.CharacterName);
        Assert.AreEqual("neutral", characterData.DefaultEmotion);
        Assert.AreEqual(1, characterData.Emotions.Count);
        Assert.IsTrue(characterData.Validate());
    }

    [Test]
    public void CharacterData_WithEmptyCharacterId_ValidateFails()
    {
        var emotion = new CharacterEmotion { emotionName = "neutral" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("")
            .WithCharacterName("Test Character")
            .WithEmotion(emotion)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("CharacterData.*characterId is required"));
        Assert.IsFalse(characterData.Validate());
    }

    [Test]
    public void CharacterData_WithEmptyCharacterName_ValidateFails()
    {
        var emotion = new CharacterEmotion { emotionName = "neutral" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("")
            .WithEmotion(emotion)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("CharacterData.*characterName is required"));
        Assert.IsFalse(characterData.Validate());
    }

    [Test]
    public void CharacterData_WithNoEmotions_ValidateFails()
    {
        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("CharacterData.*At least one emotion is required"));
        Assert.IsFalse(characterData.Validate());
    }

    [Test]
    public void CharacterData_WithInvalidDefaultEmotion_ValidateFails()
    {
        var emotion = new CharacterEmotion { emotionName = "happy" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("neutral")
            .WithEmotion(emotion)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("CharacterData.*Default emotion 'neutral' not found in emotions list"));
        Assert.IsFalse(characterData.Validate());
    }

    [Test]
    public void CharacterData_GetEmotion_ReturnsCorrectEmotion()
    {
        var happyEmotion = new CharacterEmotion { emotionName = "happy" };
        var sadEmotion = new CharacterEmotion { emotionName = "sad" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("happy")
            .WithEmotion(happyEmotion)
            .WithEmotion(sadEmotion)
            .Build();

        var emotion = characterData.GetEmotion("happy");

        Assert.IsNotNull(emotion);
        Assert.AreEqual("happy", emotion.Value.emotionName);
    }

    [Test]
    public void CharacterData_GetEmotion_WithInvalidKey_ReturnsNull()
    {
        var emotion = new CharacterEmotion { emotionName = "happy" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("happy")
            .WithEmotion(emotion)
            .Build();

        var result = characterData.GetEmotion("nonexistent");

        Assert.IsNull(result);
    }

    [Test]
    public void CharacterData_WithSpineAnimation_ValidatesSpineData()
    {
        var emotion = new CharacterEmotion { emotionName = "neutral" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("neutral")
            .WithEmotion(emotion)
            .WithAnimationType(AnimationType.Spine)
            .Build();

        LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("CharacterData.*Spine animation type requires spineDataAsset"));
        Assert.IsFalse(characterData.Validate());
    }

    [Test]
    public void CharacterData_DefaultScale_IsOne()
    {
        var emotion = new CharacterEmotion { emotionName = "neutral" };

        var characterData = new CharacterDataBuilder()
            .WithCharacterId("char_001")
            .WithCharacterName("Test Character")
            .WithDefaultEmotion("neutral")
            .WithEmotion(emotion)
            .Build();

        Assert.AreEqual(Vector2.one, characterData.DefaultScale);
    }
}

}
