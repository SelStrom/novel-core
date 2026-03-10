using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;
using System.IO;

namespace NovelCore.Tests.Editor.Windows
{
    /// <summary>
    /// Reproduction test for SceneEditorWindow creating standalone assets instead of sub-assets.
    /// This test MUST FAIL before the fix is applied.
    /// IGNORED: These tests demonstrate the old bug and are kept for documentation only.
    /// </summary>
    [TestFixture]
    [Ignore("Reproduction tests - demonstrate old bug, kept for documentation")]
    public class SceneEditorWindowSubAssetReproductionTests
    {
        private string _testScenePath;
        private SceneData _testScene;

        [SetUp]
        public void SetUp()
        {
            // Create a temporary test scene
            _testScenePath = "Assets/TestScene_SubAssetBug.asset";
            _testScene = ScriptableObject.CreateInstance<SceneData>();
            AssetDatabase.CreateAsset(_testScene, _testScenePath);
            AssetDatabase.SaveAssets();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test assets
            if (!string.IsNullOrEmpty(_testScenePath) && File.Exists(_testScenePath))
            {
                AssetDatabase.DeleteAsset(_testScenePath);
            }

            // Clean up any created DialogueLine or Choice assets
            string directory = Path.GetDirectoryName(_testScenePath);
            string[] dialogueAssets = AssetDatabase.FindAssets("t:DialogueLineData", new[] { directory });
            foreach (string guid in dialogueAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("DialogueLine") && !path.Equals(_testScenePath))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            string[] choiceAssets = AssetDatabase.FindAssets("t:ChoiceData", new[] { directory });
            foreach (string guid in choiceAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Choice") && !path.Equals(_testScenePath))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Test]
        public void CreateDialogueLine_ShouldCreateSubAsset_NotStandaloneAsset()
        {
            // Arrange
            string directory = Path.GetDirectoryName(_testScenePath);
            int initialAssetCount = AssetDatabase.FindAssets("t:DialogueLineData", new[] { directory }).Length;

            // Act - Simulate SceneEditorWindow.CreateNewDialogueLine()
            var newLine = ScriptableObject.CreateInstance<DialogueLineData>();
            string linePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/DialogueLine.asset");

            // BUG: This creates a standalone asset instead of sub-asset
            AssetDatabase.CreateAsset(newLine, linePath);
            AssetDatabase.SaveAssets();

            // Assert
            int finalAssetCount = AssetDatabase.FindAssets("t:DialogueLineData", new[] { directory }).Length;
            
            // EXPECTED: DialogueLine should be a sub-asset, so asset count should remain the same
            // ACTUAL: A new standalone asset is created, incrementing the count
            Assert.That(finalAssetCount, Is.EqualTo(initialAssetCount),
                "Bug reproduced: DialogueLineData created as standalone asset instead of sub-asset. " +
                $"Expected {initialAssetCount} DialogueLineData assets, but found {finalAssetCount}.");

            // Additional check: Verify it's NOT a sub-asset of SceneData
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            bool isSubAsset = System.Array.Exists(subAssets, obj => obj == newLine);

            Assert.That(isSubAsset, Is.True,
                "Bug reproduced: DialogueLineData is NOT a sub-asset of SceneData. " +
                "It should be embedded in the SceneData file.");
        }

        [Test]
        public void CreateChoice_ShouldCreateSubAsset_NotStandaloneAsset()
        {
            // Arrange
            string directory = Path.GetDirectoryName(_testScenePath);
            int initialAssetCount = AssetDatabase.FindAssets("t:ChoiceData", new[] { directory }).Length;

            // Act - Simulate SceneEditorWindow.CreateNewChoice()
            var newChoice = ScriptableObject.CreateInstance<ChoiceData>();
            string choicePath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/Choice.asset");

            // BUG: This creates a standalone asset instead of sub-asset
            AssetDatabase.CreateAsset(newChoice, choicePath);
            AssetDatabase.SaveAssets();

            // Assert
            int finalAssetCount = AssetDatabase.FindAssets("t:ChoiceData", new[] { directory }).Length;

            // EXPECTED: Choice should be a sub-asset, so asset count should remain the same
            // ACTUAL: A new standalone asset is created, incrementing the count
            Assert.That(finalAssetCount, Is.EqualTo(initialAssetCount),
                "Bug reproduced: ChoiceData created as standalone asset instead of sub-asset. " +
                $"Expected {initialAssetCount} ChoiceData assets, but found {finalAssetCount}.");

            // Additional check: Verify it's NOT a sub-asset of SceneData
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            bool isSubAsset = System.Array.Exists(subAssets, obj => obj == newChoice);

            Assert.That(isSubAsset, Is.True,
                "Bug reproduced: ChoiceData is NOT a sub-asset of SceneData. " +
                "It should be embedded in the SceneData file.");
        }

        [Test]
        public void SampleProjectGenerator_CreatesSubAssets_AsExpected()
        {
            // This test verifies that SampleProjectGenerator does it correctly (for comparison)
            // Arrange
            var scene = ScriptableObject.CreateInstance<SceneData>();
            string path = "Assets/TestScene_SubAssetCorrect.asset";
            AssetDatabase.CreateAsset(scene, path);

            // Act - Simulate SampleProjectGenerator approach
            var line = ScriptableObject.CreateInstance<DialogueLineData>();
            line.name = "TestLine_SubAsset";

            AssetDatabase.AddObjectToAsset(line, scene);  // ✅ Correct approach
            AssetDatabase.SaveAssets();

            // Assert
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            bool isSubAsset = System.Array.Exists(subAssets, obj => obj.name == "TestLine_SubAsset");

            Assert.That(isSubAsset, Is.True,
                "SampleProjectGenerator approach: DialogueLineData IS a sub-asset (correct behavior)");

            // Verify no standalone asset was created
            string directory = Path.GetDirectoryName(path);
            string[] standaloneAssets = AssetDatabase.FindAssets("TestLine_SubAsset t:DialogueLineData", new[] { directory });
            Assert.That(standaloneAssets.Length, Is.EqualTo(0),
                "SampleProjectGenerator approach: No standalone DialogueLineData asset created (correct)");

            // Cleanup
            AssetDatabase.DeleteAsset(path);
        }
    }
}
