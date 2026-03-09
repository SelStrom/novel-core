using NUnit.Framework;
using UnityEngine;
using NovelCore.Tests.Editor.Builders;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Runtime.Data.Dialogue;

namespace NovelCore.Tests.Editor
{

/// <summary>
/// Sample test to verify test infrastructure is working correctly.
/// </summary>
public class SampleTest : BaseTestFixture
{
    [Test]
    public void SampleTest_Infrastructure_Works()
    {
        Assert.IsTrue(true, "Test infrastructure is working");
    }

    [Test]
    public void SceneDataBuilder_CreatesValidSceneData()
    {
        var sceneData = new SceneDataBuilder()
            .WithSceneId("test_scene_001")
            .WithSceneName("Test Scene")
            .Build();

        Assert.IsNotNull(sceneData, "SceneData should not be null");
        Assert.AreEqual("test_scene_001", sceneData.SceneId, "SceneId should match");
        Assert.AreEqual("Test Scene", sceneData.SceneName, "SceneName should match");
    }

    [Test]
    public void CharacterDataBuilder_CreatesValidCharacterData()
    {
        var characterData = new CharacterDataBuilder()
            .WithCharacterId("test_char_001")
            .WithCharacterName("Test Character")
            .Build();

        Assert.IsNotNull(characterData, "CharacterData should not be null");
        Assert.AreEqual("test_char_001", characterData.CharacterId, "CharacterId should match");
        Assert.AreEqual("Test Character", characterData.CharacterName, "CharacterName should match");
    }

    [Test]
    public void DialogueLineDataBuilder_CreatesValidDialogueLineData()
    {
        var dialogueLineData = new DialogueLineDataBuilder()
            .WithLineId("test_line_001")
            .WithFallbackText("Hello, World!")
            .Build();

        Assert.IsNotNull(dialogueLineData, "DialogueLineData should not be null");
        Assert.AreEqual("test_line_001", dialogueLineData.LineId, "LineId should match");
        Assert.AreEqual("Hello, World!", dialogueLineData.FallbackText, "FallbackText should match");
    }
}

}
