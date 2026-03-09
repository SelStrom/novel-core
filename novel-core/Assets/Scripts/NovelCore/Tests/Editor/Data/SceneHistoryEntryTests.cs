using NUnit.Framework;
using NovelCore.Runtime.Data.Scenes;
using System.Collections.Generic;
using UnityEngine;

namespace NovelCore.Tests.Editor.Data
{
    /// <summary>
    /// EditMode tests for SceneHistoryEntry serialization (User Story 3).
    /// Validates entry creation, validation, and JSON serialization.
    /// </summary>
    [TestFixture]
    public class SceneHistoryEntryTests
    {
        /// <summary>
        /// Test T039: Constructor creates entry with valid data.
        /// </summary>
        [Test]
        public void Constructor_ValidData_CreatesEntry()
        {
            // Arrange & Act
            var entry = new SceneHistoryEntry("scene_001", 5);
            
            // Assert
            Assert.AreEqual("scene_001", entry.sceneId, "Scene ID should be set");
            Assert.AreEqual(5, entry.dialogueLineIndex, "Dialogue line index should be set");
            Assert.IsNotNull(entry.gameStateSnapshot, "Game state snapshot should be initialized");
            Assert.AreEqual(0, entry.gameStateSnapshot.Count, "Game state snapshot should be empty by default");
            Assert.Greater(entry.timestamp, 0, "Timestamp should be set");
        }

        /// <summary>
        /// Test T039: Constructor with game state snapshot stores snapshot.
        /// </summary>
        [Test]
        public void Constructor_WithGameState_StoresSnapshot()
        {
            // Arrange
            var gameState = new Dictionary<string, object>
            {
                { "flag_met_character", true },
                { "chapter", 2 }
            };
            
            // Act
            var entry = new SceneHistoryEntry("scene_001", 5, gameState);
            
            // Assert
            Assert.AreEqual(2, entry.gameStateSnapshot.Count, "Should store game state snapshot");
            Assert.IsTrue((bool)entry.gameStateSnapshot["flag_met_character"], "Should preserve flag value");
            Assert.AreEqual(2, (int)entry.gameStateSnapshot["chapter"], "Should preserve variable value");
        }

        /// <summary>
        /// Test T039: IsValid() returns true for valid entry.
        /// </summary>
        [Test]
        public void IsValid_ValidEntry_ReturnsTrue()
        {
            // Arrange
            var entry = new SceneHistoryEntry("scene_001", 0);
            
            // Act & Assert
            Assert.IsTrue(entry.IsValid(), "Valid entry should pass validation");
        }

        /// <summary>
        /// Test T039: IsValid() returns false for empty sceneId.
        /// </summary>
        [Test]
        public void IsValid_EmptySceneId_ReturnsFalse()
        {
            // Arrange
            var entry = new SceneHistoryEntry("", 0);
            
            // Act & Assert
            Assert.IsFalse(entry.IsValid(), "Entry with empty sceneId should be invalid");
        }

        /// <summary>
        /// Test T039: IsValid() returns false for null sceneId.
        /// </summary>
        [Test]
        public void IsValid_NullSceneId_ReturnsFalse()
        {
            // Arrange
            var entry = new SceneHistoryEntry(null, 0);
            
            // Act & Assert
            Assert.IsFalse(entry.IsValid(), "Entry with null sceneId should be invalid");
        }

        /// <summary>
        /// Test T039: IsValid() returns false for negative dialogueLineIndex.
        /// </summary>
        [Test]
        public void IsValid_NegativeDialogueIndex_ReturnsFalse()
        {
            // Arrange
            var entry = new SceneHistoryEntry("scene_001", -1);
            
            // Act & Assert
            Assert.IsFalse(entry.IsValid(), "Entry with negative dialogue index should be invalid");
        }

        /// <summary>
        /// Test T039: JSON serialization preserves all fields.
        /// </summary>
        [Test]
        public void JsonSerialization_PreservesFields()
        {
            // Arrange
            var gameState = new Dictionary<string, object>
            {
                { "flag_completed_tutorial", true },
                { "player_level", 5 }
            };
            var entry = new SceneHistoryEntry("scene_002", 10, gameState);
            
            // Act: Serialize to JSON
            var json = JsonUtility.ToJson(entry);
            
            // Assert: JSON should contain key fields
            Assert.IsTrue(json.Contains("scene_002"), "JSON should contain sceneId");
            Assert.IsTrue(json.Contains("10"), "JSON should contain dialogueLineIndex");
            
            Debug.Log($"Serialized entry: {json}");
        }

        /// <summary>
        /// Test T039: JSON deserialization restores basic fields.
        /// </summary>
        [Test]
        public void JsonDeserialization_RestoresBasicFields()
        {
            // Arrange
            var original = new SceneHistoryEntry("scene_003", 15);
            var json = JsonUtility.ToJson(original);
            
            // Act: Deserialize from JSON
            var restored = JsonUtility.FromJson<SceneHistoryEntry>(json);
            
            // Assert
            Assert.AreEqual("scene_003", restored.sceneId, "Should restore sceneId");
            Assert.AreEqual(15, restored.dialogueLineIndex, "Should restore dialogueLineIndex");
            Assert.Greater(restored.timestamp, 0, "Should restore timestamp");
        }

        /// <summary>
        /// Test T039: sceneData field is not serialized (marked as NonSerialized).
        /// </summary>
        [Test]
        public void JsonSerialization_SceneDataNotSerialized()
        {
            // Arrange
            var sceneData = ScriptableObject.CreateInstance<SceneData>();
            sceneData.name = "TestScene";
            var entry = new SceneHistoryEntry("scene_004", 0, sceneData: sceneData);
            
            // Act
            var json = JsonUtility.ToJson(entry);
            
            // Assert
            Assert.IsFalse(json.Contains("sceneData"), "sceneData should NOT be serialized");
            Assert.IsFalse(json.Contains("TestScene"), "sceneData name should NOT be in JSON");
            
            // Cleanup
            Object.DestroyImmediate(sceneData);
        }

        /// <summary>
        /// Test T039: Multiple entries can be serialized as array.
        /// </summary>
        [Test]
        public void JsonSerialization_MultipleEntries_SerializesAsArray()
        {
            // Arrange
            var entries = new List<SceneHistoryEntry>
            {
                new SceneHistoryEntry("scene_001", 0),
                new SceneHistoryEntry("scene_002", 5),
                new SceneHistoryEntry("scene_003", 10)
            };
            
            // Act: Create wrapper class for array serialization
            var wrapper = new SceneNavigationState();
            wrapper.history = entries;
            var json = JsonUtility.ToJson(wrapper);
            
            // Assert
            Assert.IsTrue(json.Contains("scene_001"), "Should contain first scene");
            Assert.IsTrue(json.Contains("scene_002"), "Should contain second scene");
            Assert.IsTrue(json.Contains("scene_003"), "Should contain third scene");
            
            Debug.Log($"Serialized history: {json}");
        }

        /// <summary>
        /// Test T039: Entry with max values serializes correctly.
        /// </summary>
        [Test]
        public void JsonSerialization_MaxValues_SerializesCorrectly()
        {
            // Arrange
            var entry = new SceneHistoryEntry("scene_with_very_long_id_that_exceeds_normal_length", int.MaxValue);
            
            // Act
            var json = JsonUtility.ToJson(entry);
            var restored = JsonUtility.FromJson<SceneHistoryEntry>(json);
            
            // Assert
            Assert.AreEqual(entry.sceneId, restored.sceneId, "Should handle long scene IDs");
            Assert.AreEqual(int.MaxValue, restored.dialogueLineIndex, "Should handle max int value");
        }
    }
}
