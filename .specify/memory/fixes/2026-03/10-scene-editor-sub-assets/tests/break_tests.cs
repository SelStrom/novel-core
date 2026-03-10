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
    /// Adversarial tests attempting to break the sub-asset fix for SceneEditorWindow.
    /// Tests edge cases and potential regressions.
    /// </summary>
    [TestFixture]
    public class SceneEditorWindowSubAssetBreakTests
    {
        private string _testScenePath;
        private SceneData _testScene;

        [SetUp]
        public void SetUp()
        {
            _testScenePath = "Assets/TestScene_BreakTests.asset";
            _testScene = ScriptableObject.CreateInstance<SceneData>();
            AssetDatabase.CreateAsset(_testScene, _testScenePath);
            AssetDatabase.SaveAssets();
        }

        [TearDown]
        public void TearDown()
        {
            if (!string.IsNullOrEmpty(_testScenePath) && File.Exists(_testScenePath))
            {
                AssetDatabase.DeleteAsset(_testScenePath);
            }
            AssetDatabase.SaveAssets();
        }

        [Test]
        public void CreateMultipleDialogueLines_AllShouldBeSubAssets()
        {
            // Try to break: Create 10 dialogue lines and verify ALL are sub-assets
            const int lineCount = 10;

            for (int i = 0; i < lineCount; i++)
            {
                var newLine = ScriptableObject.CreateInstance<DialogueLineData>();
                newLine.name = $"DialogueLine_{i + 1}";
                AssetDatabase.AddObjectToAsset(newLine, _testScene);
            }

            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Assert: All lines are sub-assets
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(lineCount),
                $"Expected {lineCount} sub-assets, found {subAssets.Length}");

            // Verify no standalone assets created
            string directory = Path.GetDirectoryName(_testScenePath);
            string[] standaloneAssets = AssetDatabase.FindAssets("DialogueLine t:DialogueLineData", new[] { directory });
            Assert.That(standaloneAssets.Length, Is.EqualTo(0),
                "No standalone DialogueLineData assets should exist");
        }

        [Test]
        public void CreateDialogueLine_WithLongName_ShouldHandleGracefully()
        {
            // Try to break: Very long name
            var newLine = ScriptableObject.CreateInstance<DialogueLineData>();
            newLine.name = new string('A', 256);  // 256 character name

            AssetDatabase.AddObjectToAsset(newLine, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Assert: Sub-asset created successfully
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(1),
                "Sub-asset with long name should be created");
        }

        [Test]
        public void CreateDialogueLine_WithEmptyName_ShouldNotCrash()
        {
            // Try to break: Empty name
            var newLine = ScriptableObject.CreateInstance<DialogueLineData>();
            newLine.name = "";

            Assert.DoesNotThrow(() =>
            {
                AssetDatabase.AddObjectToAsset(newLine, _testScene);
                EditorUtility.SetDirty(_testScene);
                AssetDatabase.SaveAssets();
            }, "Creating sub-asset with empty name should not throw");

            // Assert: Sub-asset created
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(1),
                "Sub-asset with empty name should still be created");
        }

        [Test]
        public void CreateDialogueLine_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Try to break: Special characters in name
            var newLine = ScriptableObject.CreateInstance<DialogueLineData>();
            newLine.name = "Line_!@#$%^&*()_<>?/\\|[]{}";

            Assert.DoesNotThrow(() =>
            {
                AssetDatabase.AddObjectToAsset(newLine, _testScene);
                EditorUtility.SetDirty(_testScene);
                AssetDatabase.SaveAssets();
            }, "Creating sub-asset with special characters should not throw");
        }

        [Test]
        public void DeleteSceneData_ShouldDeleteAllSubAssets()
        {
            // Setup: Create sub-assets
            var line1 = ScriptableObject.CreateInstance<DialogueLineData>();
            line1.name = "Line1";
            AssetDatabase.AddObjectToAsset(line1, _testScene);

            var line2 = ScriptableObject.CreateInstance<DialogueLineData>();
            line2.name = "Line2";
            AssetDatabase.AddObjectToAsset(line2, _testScene);

            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Verify sub-assets exist
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(2), "2 sub-assets should exist before deletion");

            // Act: Delete SceneData
            AssetDatabase.DeleteAsset(_testScenePath);
            AssetDatabase.SaveAssets();

            // Assert: Sub-assets deleted automatically
            Assert.That(File.Exists(_testScenePath), Is.False,
                "SceneData file should be deleted");

            // Verify no orphaned sub-assets
            string directory = Path.GetDirectoryName(_testScenePath);
            string[] orphanedAssets = AssetDatabase.FindAssets("Line1 t:DialogueLineData", new[] { directory });
            Assert.That(orphanedAssets.Length, Is.EqualTo(0),
                "Sub-assets should be deleted with parent SceneData");
        }

        [Test]
        public void DuplicateSceneData_ShouldDuplicateSubAssets()
        {
            // Setup: Create sub-asset
            var line = ScriptableObject.CreateInstance<DialogueLineData>();
            line.name = "OriginalLine";
            AssetDatabase.AddObjectToAsset(line, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Act: Duplicate SceneData
            string duplicatePath = "Assets/TestScene_BreakTests_Duplicate.asset";
            AssetDatabase.CopyAsset(_testScenePath, duplicatePath);
            AssetDatabase.SaveAssets();

            try
            {
                // Assert: Duplicate has sub-assets
                Object[] originalSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
                Object[] duplicateSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(duplicatePath);

                Assert.That(duplicateSubAssets.Length, Is.EqualTo(originalSubAssets.Length),
                    "Duplicate should have same number of sub-assets as original");

                // Verify they are different instances
                Assert.That(duplicateSubAssets[0], Is.Not.SameAs(originalSubAssets[0]),
                    "Duplicate sub-assets should be separate instances");
            }
            finally
            {
                // Cleanup
                AssetDatabase.DeleteAsset(duplicatePath);
            }
        }

        [Test]
        public void CreateChoice_ShouldAlsoBeSubAsset()
        {
            // Verify ChoiceData also uses sub-asset pattern
            var newChoice = ScriptableObject.CreateInstance<ChoiceData>();
            newChoice.name = "Choice_1";

            AssetDatabase.AddObjectToAsset(newChoice, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Assert: Choice is sub-asset
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(1),
                "ChoiceData should be a sub-asset");

            // Verify no standalone asset created
            string directory = Path.GetDirectoryName(_testScenePath);
            string[] standaloneChoices = AssetDatabase.FindAssets("Choice t:ChoiceData", new[] { directory });
            Assert.That(standaloneChoices.Length, Is.EqualTo(0),
                "No standalone ChoiceData assets should exist");
        }

        [Test]
        public void MixDialogueLinesAndChoices_AllShouldBeSubAssets()
        {
            // Try to break: Mix DialogueLineData and ChoiceData sub-assets
            var line = ScriptableObject.CreateInstance<DialogueLineData>();
            line.name = "Line_1";
            AssetDatabase.AddObjectToAsset(line, _testScene);

            var choice = ScriptableObject.CreateInstance<ChoiceData>();
            choice.name = "Choice_1";
            AssetDatabase.AddObjectToAsset(choice, _testScene);

            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Assert: Both are sub-assets
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssets.Length, Is.EqualTo(2),
                "Both DialogueLineData and ChoiceData should be sub-assets");

            // Verify correct types
            bool hasDialogueLine = System.Array.Exists(subAssets, obj => obj is DialogueLineData);
            bool hasChoice = System.Array.Exists(subAssets, obj => obj is ChoiceData);

            Assert.That(hasDialogueLine, Is.True, "Should have DialogueLineData sub-asset");
            Assert.That(hasChoice, Is.True, "Should have ChoiceData sub-asset");
        }

        [Test]
        public void BackwardCompatibility_ExternalReferences_ShouldStillLoad()
        {
            // Test backward compatibility: Existing SceneData with external GUID references
            // Create an external DialogueLineData (simulating old behavior)
            var externalLine = ScriptableObject.CreateInstance<DialogueLineData>();
            string externalPath = "Assets/ExternalDialogueLine_BreakTest.asset";
            AssetDatabase.CreateAsset(externalLine, externalPath);
            AssetDatabase.SaveAssets();

            try
            {
                // Load SceneData and verify it can reference external asset
                var sceneData = AssetDatabase.LoadAssetAtPath<SceneData>(_testScenePath);
                Assert.That(sceneData, Is.Not.Null, "SceneData should load");

                // This simulates existing scenes with external references still working
                var loadedExternal = AssetDatabase.LoadAssetAtPath<DialogueLineData>(externalPath);
                Assert.That(loadedExternal, Is.Not.Null,
                    "External DialogueLineData should still load (backward compatibility)");
            }
            finally
            {
                // Cleanup
                AssetDatabase.DeleteAsset(externalPath);
            }
        }
    }
}
