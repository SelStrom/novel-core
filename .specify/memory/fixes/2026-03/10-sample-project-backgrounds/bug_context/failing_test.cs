using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Characters;
using NovelCore.Editor.Tools.Generators;

namespace NovelCore.Tests.Editor.Tools.Generators
{
    /// <summary>
    /// Failing test для воспроизведения бага: бэкграунды и персонажи не прокидываются в Sample Project
    /// </summary>
    [TestFixture]
    public class SampleProjectGeneratorBackgroundAndCharacterTests
    {
        private const string SAMPLE_PROJECT_DIR = "Assets/Content/Projects/Sample";
        private const string SCENES_DIR = "Assets/Content/Projects/Sample/Scenes";
        private const string CHARACTERS_DIR = "Assets/Content/Projects/Sample/Characters";

        [SetUp]
        public void Setup()
        {
            // Очистить существующий sample project если есть
            if (Directory.Exists(SAMPLE_PROJECT_DIR))
            {
                AssetDatabase.DeleteAsset(SAMPLE_PROJECT_DIR);
                AssetDatabase.Refresh();
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Очистить после теста
            if (Directory.Exists(SAMPLE_PROJECT_DIR))
            {
                AssetDatabase.DeleteAsset(SAMPLE_PROJECT_DIR);
                AssetDatabase.Refresh();
            }
        }

        [Test]
        public void GenerateSampleProject_Scene01_ShouldHaveBackgroundImageReference()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            var scene01 = AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene01_Introduction.asset");
            Assert.That(scene01, Is.Not.Null, "Scene01_Introduction должна быть создана");

            // BUG: BackgroundImage должен содержать AssetReference на "Backgrounds/bg_room"
            Assert.That(scene01.BackgroundImage, Is.Not.Null, "Scene01 должна иметь BackgroundImage");
            Assert.That(scene01.BackgroundImage.RuntimeKeyIsValid(), Is.True, "BackgroundImage должен иметь валидный RuntimeKey");
        }

        [Test]
        public void GenerateSampleProject_Scene03a_ShouldHaveBackgroundImageReference()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            var scene03a = AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene03a_PathA.asset");
            Assert.That(scene03a, Is.Not.Null, "Scene03a_PathA должна быть создана");

            // BUG: BackgroundImage должен содержать AssetReference на "Backgrounds/bg_street"
            Assert.That(scene03a.BackgroundImage, Is.Not.Null, "Scene03a должна иметь BackgroundImage");
            Assert.That(scene03a.BackgroundImage.RuntimeKeyIsValid(), Is.True, "BackgroundImage должен иметь валидный RuntimeKey");
        }

        [Test]
        public void GenerateSampleProject_Scene03b_ShouldHaveBackgroundImageReference()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            var scene03b = AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene03b_PathB.asset");
            Assert.That(scene03b, Is.Not.Null, "Scene03b_PathB должна быть создана");

            // BUG: BackgroundImage должен содержать AssetReference на "Backgrounds/bg_home"
            Assert.That(scene03b.BackgroundImage, Is.Not.Null, "Scene03b должна иметь BackgroundImage");
            Assert.That(scene03b.BackgroundImage.RuntimeKeyIsValid(), Is.True, "BackgroundImage должен иметь валидный RuntimeKey");
        }

        [Test]
        public void GenerateSampleProject_ShouldCreateCharacterData()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            // BUG: CharacterData ScriptableObject должен быть создан
            var characterData = AssetDatabase.LoadAssetAtPath<CharacterData>($"{CHARACTERS_DIR}/CharacterData_Protagonist.asset");
            Assert.That(characterData, Is.Not.Null, "CharacterData для протагониста должен быть создан");
            
            Assert.That(characterData.CharacterId, Is.Not.Null.And.Not.Empty, "CharacterData должен иметь CharacterId");
            Assert.That(characterData.CharacterName, Is.Not.Null.And.Not.Empty, "CharacterData должен иметь CharacterName");
            Assert.That(characterData.Emotions.Count, Is.GreaterThan(0), "CharacterData должен иметь хотя бы одну эмоцию");
        }

        [Test]
        public void GenerateSampleProject_ScenesWithCharacters_ShouldHaveCharacterPlacements()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            // Загрузить все сцены
            var scenes = new[]
            {
                AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene01_Introduction.asset"),
                AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene02_ChoicePoint.asset"),
                AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene03a_PathA.asset"),
                AssetDatabase.LoadAssetAtPath<SceneData>($"{SCENES_DIR}/Scene03b_PathB.asset")
            };

            // BUG: Хотя бы одна сцена должна иметь персонажа
            var scenesWithCharacters = scenes.Where(s => s != null && s.Characters.Count > 0).ToList();
            Assert.That(scenesWithCharacters.Count, Is.GreaterThan(0), "Хотя бы одна сцена должна иметь персонажей");

            // Проверить, что CharacterPlacement имеет валидную ссылку на CharacterData
            foreach (var scene in scenesWithCharacters)
            {
                foreach (var placement in scene.Characters)
                {
                    Assert.That(placement.character, Is.Not.Null, $"CharacterPlacement в {scene.SceneName} должен иметь ссылку на character");
                    Assert.That(placement.character.RuntimeKeyIsValid(), Is.True, $"CharacterPlacement в {scene.SceneName} должен иметь валидный RuntimeKey");
                }
            }
        }

        [Test]
        public void GenerateSampleProject_CharacterData_ShouldBeAddressable()
        {
            // Arrange & Act
            SampleProjectGenerator.GenerateSampleProject();

            // Assert
            var characterData = AssetDatabase.LoadAssetAtPath<CharacterData>($"{CHARACTERS_DIR}/CharacterData_Protagonist.asset");
            Assert.That(characterData, Is.Not.Null, "CharacterData должен существовать");

            // BUG: CharacterData должен быть настроен как Addressable
            var assetPath = AssetDatabase.GetAssetPath(characterData);
            var guid = AssetDatabase.AssetPathToGUID(assetPath);

            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            Assert.That(settings, Is.Not.Null, "Addressables settings должны быть доступны");

            var entry = settings.FindAssetEntry(guid);
            Assert.That(entry, Is.Not.Null, "CharacterData должен быть зарегистрирован в Addressables");
            Assert.That(entry.address, Does.StartWith("Characters/"), "CharacterData должен иметь адрес начинающийся с 'Characters/'");
        }
    }
}
