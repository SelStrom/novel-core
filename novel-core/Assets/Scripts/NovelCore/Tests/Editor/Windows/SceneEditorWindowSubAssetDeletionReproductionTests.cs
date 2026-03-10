using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Data.Choices;

namespace NovelCore.Tests.Editor.Windows
{
    /// <summary>
    /// Reproduction test for SceneEditorWindow not deleting sub-assets when removing DialogueLine/Choice.
    /// This test MUST FAIL before the fix is applied.
    /// IGNORED: These tests demonstrate the old bug and are kept for documentation only.
    /// </summary>
    [TestFixture]
    [Ignore("Reproduction tests - demonstrate old bug, kept for documentation")]
    public class SceneEditorWindowSubAssetDeletionReproductionTests
    {
        private string _testScenePath;
        private SceneData _testScene;

        [SetUp]
        public void SetUp()
        {
            // Create a temporary test scene
            _testScenePath = "Assets/TestScene_SubAssetDeletionBug.asset";
            _testScene = ScriptableObject.CreateInstance<SceneData>();
            AssetDatabase.CreateAsset(_testScene, _testScenePath);
            AssetDatabase.SaveAssets();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test assets
            if (!string.IsNullOrEmpty(_testScenePath))
            {
                AssetDatabase.DeleteAsset(_testScenePath);
            }

            CleanupAllTestAssets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CleanupAllTestAssets()
        {
            string[] dialogueAssets = AssetDatabase.FindAssets("t:DialogueLineData", new[] { "Assets" });
            foreach (string guid in dialogueAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Test") || path.Contains("dialog"))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            string[] choiceAssets = AssetDatabase.FindAssets("t:ChoiceData", new[] { "Assets" });
            foreach (string guid in choiceAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Test") || path.Contains("Choice"))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }

        [Test]
        public void DeleteDialogueLine_ShouldDeleteSubAsset_NotJustArrayReference()
        {
            // Arrange: Create DialogueLine as sub-asset (correct approach)
            var dialogueLine = ScriptableObject.CreateInstance<DialogueLineData>();
            dialogueLine.name = "dialog_line_001";
            AssetDatabase.AddObjectToAsset(dialogueLine, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Verify sub-asset was created
            Object[] subAssetsBefore = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsBefore.Length, Is.EqualTo(1), "Setup failed: Sub-asset should exist");
            Assert.That(subAssetsBefore[0], Is.EqualTo(dialogueLine), "Setup failed: Sub-asset mismatch");

            // Act: Simulate SceneEditorWindow deletion (buggy approach)
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();

            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 1;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = dialogueLine;
            serializedObject.ApplyModifiedProperties();

            // Now delete (this is what SceneEditorWindow does - BUG!)
            dialogueProperty.DeleteArrayElementAtIndex(0);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Sub-asset should be DELETED from asset database
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);

            Assert.That(subAssetsAfter.Length, Is.EqualTo(0),
                "Bug reproduced: DialogueLineData sub-asset was NOT deleted. " +
                $"Expected 0 sub-assets, but found {subAssetsAfter.Length}. " +
                "DeleteArrayElementAtIndex() only removes the reference, not the sub-asset itself.");
        }

        [Test]
        public void DeleteChoice_ShouldDeleteSubAsset_NotJustArrayReference()
        {
            // Arrange: Create Choice as sub-asset (correct approach)
            var choice = ScriptableObject.CreateInstance<ChoiceData>();
            choice.name = "choice_001";
            AssetDatabase.AddObjectToAsset(choice, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Verify sub-asset was created
            Object[] subAssetsBefore = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsBefore.Length, Is.EqualTo(1), "Setup failed: Sub-asset should exist");
            Assert.That(subAssetsBefore[0], Is.EqualTo(choice), "Setup failed: Sub-asset mismatch");

            // Act: Simulate SceneEditorWindow deletion (buggy approach)
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();

            var choicesProperty = serializedObject.FindProperty("_choices");
            choicesProperty.arraySize = 1;
            choicesProperty.GetArrayElementAtIndex(0).objectReferenceValue = choice;
            serializedObject.ApplyModifiedProperties();

            // Now delete (this is what SceneEditorWindow does - BUG!)
            choicesProperty.DeleteArrayElementAtIndex(0);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Sub-asset should be DELETED from asset database
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);

            Assert.That(subAssetsAfter.Length, Is.EqualTo(0),
                "Bug reproduced: ChoiceData sub-asset was NOT deleted. " +
                $"Expected 0 sub-assets, but found {subAssetsAfter.Length}. " +
                "DeleteArrayElementAtIndex() only removes the reference, not the sub-asset itself.");
        }

        [Test]
        public void DeleteMultipleDialogueLines_ShouldDeleteAllSubAssets()
        {
            // Arrange: Create 3 DialogueLines as sub-assets
            var line1 = ScriptableObject.CreateInstance<DialogueLineData>();
            line1.name = "dialog_line_001";
            AssetDatabase.AddObjectToAsset(line1, _testScene);

            var line2 = ScriptableObject.CreateInstance<DialogueLineData>();
            line2.name = "dialog_line_002";
            AssetDatabase.AddObjectToAsset(line2, _testScene);

            var line3 = ScriptableObject.CreateInstance<DialogueLineData>();
            line3.name = "dialog_line_003";
            AssetDatabase.AddObjectToAsset(line3, _testScene);

            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Verify 3 sub-assets exist
            Object[] subAssetsBefore = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsBefore.Length, Is.EqualTo(3), "Setup failed: Should have 3 sub-assets");

            // Act: Add all to array, then delete middle one (line2)
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();

            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 3;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = line1;
            dialogueProperty.GetArrayElementAtIndex(1).objectReferenceValue = line2;
            dialogueProperty.GetArrayElementAtIndex(2).objectReferenceValue = line3;
            serializedObject.ApplyModifiedProperties();

            // Delete middle DialogueLine (index 1)
            dialogueProperty.DeleteArrayElementAtIndex(1);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Should have 2 sub-assets (line1 and line3), line2 should be deleted
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);

            Assert.That(subAssetsAfter.Length, Is.EqualTo(2),
                "Bug reproduced: Deleted DialogueLine sub-asset still exists. " +
                $"Expected 2 sub-assets (after deleting 1 of 3), but found {subAssetsAfter.Length}.");

            // Verify deleted line is NOT in sub-assets
            bool line2Exists = System.Array.Exists(subAssetsAfter, obj => obj.name == "dialog_line_002");
            Assert.That(line2Exists, Is.False,
                "Bug reproduced: Deleted DialogueLine 'dialog_line_002' still exists as sub-asset.");
        }
    }
}
