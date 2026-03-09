using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using DialogueSystemImpl = NovelCore.Runtime.Core.DialogueSystem.DialogueSystem;
using NovelCore.Runtime.Data.Scenes;
using NovelCore.Runtime.Data.Dialogue;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Tests.Runtime.Builders;
using NovelCore.Tests.Runtime.Core;

namespace NovelCore.Tests.Runtime.Integration
{
    /// <summary>
    /// Integration tests for scene navigation history flow (User Story 3).
    /// Validates end-to-end back/forward navigation through scenes.
    /// </summary>
    public class SceneNavigationFlowTests : BaseTestFixture
    {
        private DialogueSystemImpl _dialogueSystem;
        private SceneNavigationHistory _history;
        private MockAudioService _mockAudioService;
        private MockInputService _mockInputService;
        private MockSceneManager _mockSceneManager;
        private MockAssetManager _mockAssetManager;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            _mockAudioService = new MockAudioService();
            _mockInputService = new MockInputService();
            _mockSceneManager = new MockSceneManager();
            _mockAssetManager = new MockAssetManager();
            _history = new SceneNavigationHistory(SceneNavigationHistory.DEFAULT_MAX_HISTORY_SIZE);
            
            _dialogueSystem = new DialogueSystemImpl(
                _mockAudioService,
                _mockInputService,
                _mockSceneManager,
                _mockAssetManager
            );
        }

        /// <summary>
        /// Test T043: Full navigation flow - forward progression, then back, then forward again.
        /// </summary>
        [UnityTest]
        public IEnumerator NavigationFlow_ThreeScenes_BackAndForward()
        {
            // Arrange: Create three scenes
            var scene1 = new SceneDataBuilder()
                .WithSceneId("scene_001")
                .WithSceneName("Scene 1")
                .WithDialogueLine(new DialogueLineDataBuilder()
                    .WithLineId("line_1_1")
                    .WithFallbackText("First scene")
                    .Build())
                .Build();

            var scene2 = new SceneDataBuilder()
                .WithSceneId("scene_002")
                .WithSceneName("Scene 2")
                .WithDialogueLine(new DialogueLineDataBuilder()
                    .WithLineId("line_2_1")
                    .WithFallbackText("Second scene")
                    .Build())
                .Build();

            var scene3 = new SceneDataBuilder()
                .WithSceneId("scene_003")
                .WithSceneName("Scene 3")
                .WithDialogueLine(new DialogueLineDataBuilder()
                    .WithLineId("line_3_1")
                    .WithFallbackText("Third scene")
                    .Build())
                .Build();

            // Act & Assert: Navigate through scenes and build history
            // Scene 1
            _dialogueSystem.StartScene(scene1);
            _history.Push(new SceneHistoryEntry("scene_001", 0, sceneData: scene1));
            yield return null;
            Assert.AreEqual(1, _history.Count, "History should have 1 entry after scene 1");
            Assert.IsFalse(_history.CanNavigateBack(), "Cannot navigate back from first scene");

            // Scene 2
            _dialogueSystem.StartScene(scene2);
            _history.Push(new SceneHistoryEntry("scene_002", 0, sceneData: scene2));
            yield return null;
            Assert.AreEqual(2, _history.Count, "History should have 2 entries after scene 2");
            Assert.IsTrue(_history.CanNavigateBack(), "Can navigate back from second scene");
            Assert.IsFalse(_history.CanNavigateForward(), "Cannot navigate forward (at latest)");

            // Scene 3
            _dialogueSystem.StartScene(scene3);
            _history.Push(new SceneHistoryEntry("scene_003", 0, sceneData: scene3));
            yield return null;
            Assert.AreEqual(3, _history.Count, "History should have 3 entries after scene 3");
            Assert.IsTrue(_history.CanNavigateBack(), "Can navigate back from third scene");

            // Navigate back to scene 2
            var entry = _history.NavigateBack();
            Assert.IsNotNull(entry, "NavigateBack should return entry");
            Assert.AreEqual("scene_002", entry.sceneId, "Should navigate back to scene 2");
            Assert.IsTrue(_history.CanNavigateBack(), "Can still navigate back");
            Assert.IsTrue(_history.CanNavigateForward(), "Can navigate forward after going back");

            // Navigate back to scene 1
            entry = _history.NavigateBack();
            Assert.AreEqual("scene_001", entry.sceneId, "Should navigate back to scene 1");
            Assert.IsFalse(_history.CanNavigateBack(), "Cannot navigate back from first scene");
            Assert.IsTrue(_history.CanNavigateForward(), "Can navigate forward from first scene");

            // Navigate forward to scene 2
            entry = _history.NavigateForward();
            Assert.AreEqual("scene_002", entry.sceneId, "Should navigate forward to scene 2");
            Assert.IsTrue(_history.CanNavigateBack(), "Can navigate back again");
            Assert.IsTrue(_history.CanNavigateForward(), "Can still navigate forward");

            // Navigate forward to scene 3
            entry = _history.NavigateForward();
            Assert.AreEqual("scene_003", entry.sceneId, "Should navigate forward to scene 3");
            Assert.IsTrue(_history.CanNavigateBack(), "Can navigate back from scene 3");
            Assert.IsFalse(_history.CanNavigateForward(), "Cannot navigate forward (at end)");
        }

        /// <summary>
        /// Test T043: Navigation with branching - new path clears forward history.
        /// </summary>
        [UnityTest]
        public IEnumerator NavigationFlow_BranchingPath_ClearsForwardHistory()
        {
            // Arrange
            var scene1 = new SceneDataBuilder()
                .WithSceneId("scene_001")
                .WithSceneName("Scene 1")
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_1").WithFallbackText("Line 1").Build())
                .Build();

            var scene2 = new SceneDataBuilder()
                .WithSceneId("scene_002")
                .WithSceneName("Scene 2")
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_2").WithFallbackText("Line 2").Build())
                .Build();

            var scene3 = new SceneDataBuilder()
                .WithSceneId("scene_003")
                .WithSceneName("Scene 3")
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_3").WithFallbackText("Line 3").Build())
                .Build();

            var sceneAlt = new SceneDataBuilder()
                .WithSceneId("scene_alt")
                .WithSceneName("Alternate Scene")
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_alt").WithFallbackText("Alt").Build())
                .Build();

            // Act: Build initial path
            _history.Push(new SceneHistoryEntry("scene_001", 0, sceneData: scene1));
            _history.Push(new SceneHistoryEntry("scene_002", 0, sceneData: scene2));
            _history.Push(new SceneHistoryEntry("scene_003", 0, sceneData: scene3));
            yield return null;

            Assert.AreEqual(3, _history.Count, "Should have 3 entries");

            // Navigate back to scene 1
            _history.NavigateBack(); // Now at scene 2
            _history.NavigateBack(); // Now at scene 1
            Assert.IsTrue(_history.CanNavigateForward(), "Should be able to navigate forward");

            // Take alternate path (branch)
            _history.Push(new SceneHistoryEntry("scene_alt", 0, sceneData: sceneAlt));
            yield return null;

            // Assert: Forward history cleared
            Assert.AreEqual(2, _history.Count, "Should have 2 entries (scene_001 + scene_alt)");
            Assert.IsFalse(_history.CanNavigateForward(), "Forward history should be cleared");
            Assert.AreEqual("scene_alt", _history.Peek().sceneId, "Current scene should be alternate");
        }

        /// <summary>
        /// Test T043: Long navigation chain (stress test for history management).
        /// </summary>
        [UnityTest]
        public IEnumerator NavigationFlow_LongChain_MaintainsCorrectState()
        {
            // Arrange: Create 10 scenes
            for (int i = 1; i <= 10; i++)
            {
                var scene = new SceneDataBuilder()
                    .WithSceneId($"scene_{i:D3}")
                    .WithSceneName($"Scene {i}")
                    .WithDialogueLine(new DialogueLineDataBuilder()
                        .WithLineId($"line_{i}")
                        .WithFallbackText($"Scene {i} dialogue")
                        .Build())
                    .Build();

                _history.Push(new SceneHistoryEntry($"scene_{i:D3}", 0, sceneData: scene));
            }

            yield return null;

            // Assert initial state
            Assert.AreEqual(10, _history.Count, "Should have 10 entries");
            Assert.AreEqual("scene_010", _history.Peek().sceneId, "Should be at scene 10");
            Assert.IsTrue(_history.CanNavigateBack(), "Can navigate back");
            Assert.IsFalse(_history.CanNavigateForward(), "Cannot navigate forward");

            // Navigate back 5 times (from index 9 to index 4)
            SceneHistoryEntry lastBackEntry = null;
            for (int i = 0; i < 5; i++)
            {
                lastBackEntry = _history.NavigateBack();
            }

            // After 5x NavigateBack from index 9, we're at index 4 (scene_005)
            Assert.AreEqual("scene_005", lastBackEntry.sceneId, "After 5 back steps, should be at scene_005");
            Assert.IsTrue(_history.CanNavigateBack(), "Can still navigate back (not at index 0)");
            Assert.IsTrue(_history.CanNavigateForward(), "Can navigate forward");

            // Navigate forward 3 times (from index 4 to index 7)
            SceneHistoryEntry lastForwardEntry = null;
            for (int i = 0; i < 3; i++)
            {
                lastForwardEntry = _history.NavigateForward();
            }

            // After 3x NavigateForward from index 4, we're at index 7 (scene_008)
            Assert.AreEqual("scene_008", lastForwardEntry.sceneId, "After 3 forward steps from scene_005, should be at scene_008");
        }

        /// <summary>
        /// Test T043: Navigation preserves dialogue line index.
        /// </summary>
        [UnityTest]
        public IEnumerator NavigationFlow_PreservesDialogueLineIndex()
        {
            // Arrange
            var scene1 = new SceneDataBuilder()
                .WithSceneId("scene_001")
                .WithSceneName("Scene 1")
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_1_1").WithFallbackText("Line 1").Build())
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_1_2").WithFallbackText("Line 2").Build())
                .WithDialogueLine(new DialogueLineDataBuilder().WithLineId("line_1_3").WithFallbackText("Line 3").Build())
                .Build();

            // Act: Save history at dialogue line 2
            _history.Push(new SceneHistoryEntry("scene_001", 2, sceneData: scene1));
            yield return null;

            // Navigate away and back
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            
            // Navigate back to scene_001 (which has dialogueLineIndex=2)
            var entry = _history.NavigateBack();

            // Assert
            Assert.AreEqual("scene_001", entry.sceneId, "Should navigate back to scene_001");
            Assert.AreEqual(2, entry.dialogueLineIndex, "Should preserve dialogue line index from scene_001");
        }
    }
}
