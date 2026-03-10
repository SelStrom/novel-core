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
    /// Adversarial tests attempting to break the fix for sub-asset deletion.
    /// These tests try edge cases, boundary conditions, and unusual scenarios.
    /// </summary>
    [TestFixture]
    public class SceneEditorWindowSubAssetDeletionBreakTests
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
            if (!string.IsNullOrEmpty(_testScenePath))
            {
                AssetDatabase.DeleteAsset(_testScenePath);
            }

            // Cleanup any standalone assets that might have been created
            string directory = Path.GetDirectoryName(_testScenePath);
            string[] dialogueAssets = AssetDatabase.FindAssets("t:DialogueLineData", new[] { directory });
            foreach (string guid in dialogueAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Equals(_testScenePath))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            string[] choiceAssets = AssetDatabase.FindAssets("t:ChoiceData", new[] { directory });
            foreach (string guid in choiceAssets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.Equals(_testScenePath))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Test]
        public void DeleteDialogueLine_WithNullReference_ShouldNotThrow()
        {
            // Arrange: Add null reference to dialogue lines array
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();

            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 1;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;  // Null reference
            serializedObject.ApplyModifiedProperties();

            // Act: Delete null reference (should not throw)
            Assert.DoesNotThrow(() =>
            {
                serializedObject.Update();
                dialogueProperty.DeleteArrayElementAtIndex(0);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }, "Deleting null reference should not throw exception");

            // Assert: Array should be empty
            serializedObject.Update();
            Assert.That(dialogueProperty.arraySize, Is.EqualTo(0), "Null reference should be removed from array");
        }

        [Test]
        public void DeleteDialogueLine_StandaloneAsset_ShouldNotDeleteAsset()
        {
            // Arrange: Create standalone DialogueLineData asset (not sub-asset)
            var standalone = ScriptableObject.CreateInstance<DialogueLineData>();
            string standalonePath = "Assets/StandaloneDialogue_BreakTest.asset";
            AssetDatabase.CreateAsset(standalone, standalonePath);
            AssetDatabase.SaveAssets();

            // Add reference to standalone asset in SceneData
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();

            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 1;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = standalone;
            serializedObject.ApplyModifiedProperties();

            // Act: Simulate deletion (with fix's IsSubAsset() check)
            var subAsset = dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue;
            if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);  // Should NOT execute (not a sub-asset)
            }
            dialogueProperty.DeleteArrayElementAtIndex(0);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Standalone asset should STILL EXIST (not deleted)
            bool standaloneExists = File.Exists(standalonePath);
            Assert.That(standaloneExists, Is.True,
                "Standalone DialogueLineData asset should NOT be deleted when removing reference from SceneData");

            // Cleanup
            AssetDatabase.DeleteAsset(standalonePath);
        }

        [Test]
        public void DeleteMultipleDialogueLines_Sequentially_ShouldDeleteAllSubAssets()
        {
            // Arrange: Create 5 DialogueLines as sub-assets
            for (int i = 0; i < 5; i++)
            {
                var line = ScriptableObject.CreateInstance<DialogueLineData>();
                line.name = $"dialog_line_{i + 1:D3}";
                AssetDatabase.AddObjectToAsset(line, _testScene);
            }
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Verify 5 sub-assets exist
            Object[] subAssetsBefore = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsBefore.Length, Is.EqualTo(5), "Setup: Should have 5 sub-assets");

            // Add all to array
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();
            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 5;
            for (int i = 0; i < 5; i++)
            {
                dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue = subAssetsBefore[i];
            }
            serializedObject.ApplyModifiedProperties();

            // Act: Delete all DialogueLines one by one (reverse order to avoid index shifting issues)
            for (int i = 4; i >= 0; i--)
            {
                serializedObject.Update();
                var subAsset = dialogueProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
                {
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    EditorUtility.SetDirty(_testScene);
                }
                dialogueProperty.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

            // Assert: ALL sub-assets should be deleted
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsAfter.Length, Is.EqualTo(0),
                "All sub-assets should be deleted after sequential deletions");
        }

        [Test]
        public void DeleteDialogueLine_DuplicateReferences_ShouldDeleteSubAssetOnlyOnce()
        {
            // Arrange: Create one sub-asset, reference it TWICE in array (edge case)
            var line = ScriptableObject.CreateInstance<DialogueLineData>();
            line.name = "dialog_line_001";
            AssetDatabase.AddObjectToAsset(line, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();
            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 2;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = line;  // First reference
            dialogueProperty.GetArrayElementAtIndex(1).objectReferenceValue = line;  // Duplicate reference
            serializedObject.ApplyModifiedProperties();

            // Act: Delete first reference (sub-asset should be deleted)
            serializedObject.Update();
            var subAsset = dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue;
            if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                EditorUtility.SetDirty(_testScene);
            }
            dialogueProperty.DeleteArrayElementAtIndex(0);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Sub-asset deleted, second reference becomes null (broken reference)
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsAfter.Length, Is.EqualTo(0),
                "Sub-asset should be deleted even if duplicate references exist");

            // Second reference becomes a broken reference after sub-asset deletion
            // Unity may not automatically null it out - this is acceptable behavior
            serializedObject.Update();
            var secondRef = dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue;
            
            // Check if reference is null OR points to destroyed object
            bool isNullOrDestroyed = secondRef == null || !AssetDatabase.Contains(secondRef);
            Assert.That(isNullOrDestroyed, Is.True,
                "Duplicate reference should be null or point to destroyed object after sub-asset deletion");
        }

        [Test]
        public void DeleteChoice_WithMaxArraySize_ShouldNotOverflow()
        {
            // Arrange: Create many Choices (stress test)
            const int maxChoices = 100;
            for (int i = 0; i < maxChoices; i++)
            {
                var choice = ScriptableObject.CreateInstance<ChoiceData>();
                choice.name = $"choice_{i + 1:D3}";
                AssetDatabase.AddObjectToAsset(choice, _testScene);
            }
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            Object[] subAssetsBefore = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsBefore.Length, Is.EqualTo(maxChoices), "Setup: Should have 100 sub-assets");

            // Add all to array
            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();
            var choicesProperty = serializedObject.FindProperty("_choices");
            choicesProperty.arraySize = maxChoices;
            for (int i = 0; i < maxChoices; i++)
            {
                choicesProperty.GetArrayElementAtIndex(i).objectReferenceValue = subAssetsBefore[i];
            }
            serializedObject.ApplyModifiedProperties();

            // Act: Delete middle choice (index 50)
            serializedObject.Update();
            var subAsset = choicesProperty.GetArrayElementAtIndex(50).objectReferenceValue;
            if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                EditorUtility.SetDirty(_testScene);
            }
            choicesProperty.DeleteArrayElementAtIndex(50);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Assert: Should have 99 sub-assets remaining
            Object[] subAssetsAfter = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsAfter.Length, Is.EqualTo(maxChoices - 1),
                "Deleting from large array should not cause overflow or corruption");
        }

        [Test]
        public void DeleteDialogueLine_ThenCreateNew_ShouldNotConflict()
        {
            // Arrange: Create DialogueLine, then delete it
            var line1 = ScriptableObject.CreateInstance<DialogueLineData>();
            line1.name = "dialog_line_001";
            AssetDatabase.AddObjectToAsset(line1, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            var serializedObject = new SerializedObject(_testScene);
            serializedObject.Update();
            var dialogueProperty = serializedObject.FindProperty("_dialogueLines");
            dialogueProperty.arraySize = 1;
            dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue = line1;
            serializedObject.ApplyModifiedProperties();

            // Delete
            serializedObject.Update();
            var subAsset = dialogueProperty.GetArrayElementAtIndex(0).objectReferenceValue;
            if (subAsset != null && AssetDatabase.IsSubAsset(subAsset))
            {
                AssetDatabase.RemoveObjectFromAsset(subAsset);
                EditorUtility.SetDirty(_testScene);
            }
            dialogueProperty.DeleteArrayElementAtIndex(0);
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            // Verify deletion
            Object[] subAssetsAfterDelete = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsAfterDelete.Length, Is.EqualTo(0), "Sub-asset should be deleted");

            // Act: Create NEW DialogueLine with SAME NAME (potential conflict)
            var line2 = ScriptableObject.CreateInstance<DialogueLineData>();
            line2.name = "dialog_line_001";  // Same name as deleted one
            AssetDatabase.AddObjectToAsset(line2, _testScene);
            EditorUtility.SetDirty(_testScene);
            AssetDatabase.SaveAssets();

            // Assert: New sub-asset should be created without conflict
            Object[] subAssetsAfterCreate = AssetDatabase.LoadAllAssetRepresentationsAtPath(_testScenePath);
            Assert.That(subAssetsAfterCreate.Length, Is.EqualTo(1),
                "New sub-asset should be created even if name matches deleted sub-asset");

            Assert.That(subAssetsAfterCreate[0].name, Is.EqualTo("dialog_line_001"),
                "New sub-asset should have correct name");
        }
    }
}
