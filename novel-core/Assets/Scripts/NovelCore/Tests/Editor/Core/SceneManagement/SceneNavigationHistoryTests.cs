using NUnit.Framework;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Data.Scenes;
using System.Collections.Generic;

namespace NovelCore.Tests.Editor.Core.SceneManagement
{
    /// <summary>
    /// EditMode tests for SceneNavigationHistory stack operations (User Story 3).
    /// Validates Push, Pop, NavigateBack, NavigateForward, and memory management.
    /// </summary>
    [TestFixture]
    public class SceneNavigationHistoryTests
    {
        private SceneNavigationHistory _history;
        
        [SetUp]
        public void SetUp()
        {
            _history = new SceneNavigationHistory(SceneNavigationHistory.DEFAULT_MAX_HISTORY_SIZE);
        }

        [TearDown]
        public void TearDown()
        {
            _history = null;
        }

        /// <summary>
        /// Test T038: Push() adds entry to history and increments count.
        /// </summary>
        [Test]
        public void Push_ValidEntry_AddsToHistory()
        {
            // Arrange
            var entry = new SceneHistoryEntry("scene_001", 0);
            
            // Act
            _history.Push(entry);
            
            // Assert
            Assert.AreEqual(1, _history.Count, "History should contain 1 entry after Push");
            Assert.AreEqual(entry, _history.Peek(), "Peek should return the pushed entry");
        }

        /// <summary>
        /// Test T038: Push() with null entry does not modify history.
        /// </summary>
        [Test]
        public void Push_NullEntry_DoesNotModifyHistory()
        {
            // Act
            _history.Push(null);
            
            // Assert
            Assert.AreEqual(0, _history.Count, "History should remain empty after pushing null");
        }

        /// <summary>
        /// Test T038: Push() with invalid entry (empty sceneId) does not modify history.
        /// </summary>
        [Test]
        public void Push_InvalidEntry_DoesNotModifyHistory()
        {
            // Arrange
            var invalidEntry = new SceneHistoryEntry("", 0); // Empty sceneId
            
            // Act
            _history.Push(invalidEntry);
            
            // Assert
            Assert.AreEqual(0, _history.Count, "History should reject invalid entry");
        }

        /// <summary>
        /// Test T038: Pop() removes and returns last entry.
        /// </summary>
        [Test]
        public void Pop_WithEntries_ReturnsAndRemovesLastEntry()
        {
            // Arrange
            var entry1 = new SceneHistoryEntry("scene_001", 0);
            var entry2 = new SceneHistoryEntry("scene_002", 5);
            _history.Push(entry1);
            _history.Push(entry2);
            
            // Act
            var popped = _history.Pop();
            
            // Assert
            Assert.AreEqual(entry2, popped, "Pop should return last pushed entry");
            Assert.AreEqual(1, _history.Count, "History count should decrease after Pop");
            Assert.AreEqual(entry1, _history.Peek(), "Remaining entry should be the first one");
        }

        /// <summary>
        /// Test T038: Pop() on empty history returns null.
        /// </summary>
        [Test]
        public void Pop_EmptyHistory_ReturnsNull()
        {
            // Act
            var popped = _history.Pop();
            
            // Assert
            Assert.IsNull(popped, "Pop on empty history should return null");
            Assert.AreEqual(0, _history.Count, "Count should remain 0");
        }

        /// <summary>
        /// Test T038: Peek() returns last entry without removing it.
        /// </summary>
        [Test]
        public void Peek_WithEntries_ReturnsLastEntryWithoutRemoving()
        {
            // Arrange
            var entry = new SceneHistoryEntry("scene_001", 0);
            _history.Push(entry);
            
            // Act
            var peeked = _history.Peek();
            
            // Assert
            Assert.AreEqual(entry, peeked, "Peek should return last entry");
            Assert.AreEqual(1, _history.Count, "Peek should not modify history count");
        }

        /// <summary>
        /// Test T038: Peek() on empty history returns null.
        /// </summary>
        [Test]
        public void Peek_EmptyHistory_ReturnsNull()
        {
            // Act
            var peeked = _history.Peek();
            
            // Assert
            Assert.IsNull(peeked, "Peek on empty history should return null");
        }

        /// <summary>
        /// Test T038: CanNavigateBack() returns false when at beginning.
        /// </summary>
        [Test]
        public void CanNavigateBack_AtBeginning_ReturnsFalse()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            
            // Assert
            Assert.IsFalse(_history.CanNavigateBack(), "Cannot navigate back when at first entry");
        }

        /// <summary>
        /// Test T038: CanNavigateBack() returns true when not at beginning.
        /// </summary>
        [Test]
        public void CanNavigateBack_NotAtBeginning_ReturnsTrue()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            
            // Assert
            Assert.IsTrue(_history.CanNavigateBack(), "Should be able to navigate back with multiple entries");
        }

        /// <summary>
        /// Test T038: CanNavigateForward() returns false at end of history.
        /// </summary>
        [Test]
        public void CanNavigateForward_AtEnd_ReturnsFalse()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            
            // Assert
            Assert.IsFalse(_history.CanNavigateForward(), "Cannot navigate forward when at latest entry");
        }

        /// <summary>
        /// Test T038: CanNavigateForward() returns true after NavigateBack().
        /// </summary>
        [Test]
        public void CanNavigateForward_AfterNavigateBack_ReturnsTrue()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            _history.NavigateBack();
            
            // Assert
            Assert.IsTrue(_history.CanNavigateForward(), "Should be able to navigate forward after going back");
        }

        /// <summary>
        /// Test T038: NavigateBack() moves to previous entry.
        /// </summary>
        [Test]
        public void NavigateBack_WithHistory_ReturnsPreviousEntry()
        {
            // Arrange
            var entry1 = new SceneHistoryEntry("scene_001", 0);
            var entry2 = new SceneHistoryEntry("scene_002", 5);
            _history.Push(entry1);
            _history.Push(entry2);
            
            // Act
            var navigated = _history.NavigateBack();
            
            // Assert
            Assert.AreEqual(entry1, navigated, "NavigateBack should return previous entry");
            Assert.IsTrue(_history.CanNavigateForward(), "Should be able to navigate forward after going back");
        }

        /// <summary>
        /// Test T038: NavigateBack() at beginning returns null.
        /// </summary>
        [Test]
        public void NavigateBack_AtBeginning_ReturnsNull()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            
            // Act
            var navigated = _history.NavigateBack();
            
            // Assert
            Assert.IsNull(navigated, "NavigateBack at beginning should return null");
        }

        /// <summary>
        /// Test T038: NavigateForward() moves to next entry.
        /// </summary>
        [Test]
        public void NavigateForward_AfterNavigateBack_ReturnsNextEntry()
        {
            // Arrange
            var entry1 = new SceneHistoryEntry("scene_001", 0);
            var entry2 = new SceneHistoryEntry("scene_002", 5);
            _history.Push(entry1);
            _history.Push(entry2);
            _history.NavigateBack();
            
            // Act
            var navigated = _history.NavigateForward();
            
            // Assert
            Assert.AreEqual(entry2, navigated, "NavigateForward should return next entry");
            Assert.IsFalse(_history.CanNavigateForward(), "Should be at end after navigating forward");
        }

        /// <summary>
        /// Test T038: NavigateForward() at end returns null.
        /// </summary>
        [Test]
        public void NavigateForward_AtEnd_ReturnsNull()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            
            // Act
            var navigated = _history.NavigateForward();
            
            // Assert
            Assert.IsNull(navigated, "NavigateForward at end should return null");
        }

        /// <summary>
        /// Test T038: Push() after NavigateBack() clears forward history.
        /// </summary>
        [Test]
        public void Push_AfterNavigateBack_ClearsForwardHistory()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            _history.Push(new SceneHistoryEntry("scene_003", 0));
            _history.NavigateBack(); // Now at scene_002
            _history.NavigateBack(); // Now at scene_001
            
            // Act
            _history.Push(new SceneHistoryEntry("scene_new", 0));
            
            // Assert
            Assert.AreEqual(2, _history.Count, "Should have original + new entry (forward cleared)");
            Assert.IsFalse(_history.CanNavigateForward(), "Should not be able to navigate forward");
            Assert.AreEqual("scene_new", _history.Peek().sceneId, "New entry should be at top");
        }

        /// <summary>
        /// Test T038: Memory limit enforcement - oldest entries removed when exceeding max.
        /// </summary>
        [Test]
        public void Push_ExceedsMaxSize_RemovesOldestEntries()
        {
            // Arrange
            var smallHistory = new SceneNavigationHistory(3); // Max 3 entries
            
            // Act: Add 5 entries
            smallHistory.Push(new SceneHistoryEntry("scene_001", 0));
            smallHistory.Push(new SceneHistoryEntry("scene_002", 0));
            smallHistory.Push(new SceneHistoryEntry("scene_003", 0));
            smallHistory.Push(new SceneHistoryEntry("scene_004", 0));
            smallHistory.Push(new SceneHistoryEntry("scene_005", 0));
            
            // Assert
            Assert.AreEqual(3, smallHistory.Count, "Should enforce max size limit");
            Assert.AreEqual("scene_005", smallHistory.Peek().sceneId, "Latest entry should be at top");
            
            // Verify oldest entries were removed (scene_001 and scene_002 should be gone)
            // After Push(5 scenes) with max=3, history should be: [scene_003, scene_004, scene_005]
            // currentIndex = 2 (scene_005)
            var entry = smallHistory.NavigateBack(); // currentIndex = 1 (scene_004)
            Assert.AreEqual("scene_004", entry.sceneId, "NavigateBack should return scene_004");
            
            entry = smallHistory.NavigateBack(); // currentIndex = 0 (scene_003)
            Assert.AreEqual("scene_003", entry.sceneId, "NavigateBack again should return scene_003");
            Assert.IsFalse(smallHistory.CanNavigateBack(), "Should be at oldest entry (scene_003)");
        }

        /// <summary>
        /// Test T038: Clear() removes all entries.
        /// </summary>
        [Test]
        public void Clear_WithEntries_RemovesAllEntries()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 0));
            _history.Push(new SceneHistoryEntry("scene_003", 0));
            
            // Act
            _history.Clear();
            
            // Assert
            Assert.AreEqual(0, _history.Count, "Count should be 0 after Clear");
            Assert.IsNull(_history.Peek(), "Peek should return null after Clear");
            Assert.IsFalse(_history.CanNavigateBack(), "Cannot navigate back after Clear");
            Assert.IsFalse(_history.CanNavigateForward(), "Cannot navigate forward after Clear");
        }

        /// <summary>
        /// Test T038: GetState() returns current state.
        /// </summary>
        [Test]
        public void GetState_ReturnsCurrentState()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            _history.Push(new SceneHistoryEntry("scene_002", 5));
            
            // Act
            var state = _history.GetState();
            
            // Assert
            Assert.IsNotNull(state, "GetState should return non-null state");
            Assert.AreEqual(2, state.history.Count, "State should contain all entries");
            Assert.AreEqual(1, state.currentIndex, "State should reflect current index");
        }

        /// <summary>
        /// Test T038: RestoreState() restores history from state.
        /// </summary>
        [Test]
        public void RestoreState_ValidState_RestoresHistory()
        {
            // Arrange
            var state = new SceneNavigationState();
            state.history.Add(new SceneHistoryEntry("scene_001", 0));
            state.history.Add(new SceneHistoryEntry("scene_002", 5));
            state.currentIndex = 0;
            
            // Act
            _history.RestoreState(state);
            
            // Assert
            Assert.AreEqual(2, _history.Count, "History should have restored entries");
            Assert.IsTrue(_history.CanNavigateForward(), "Should be able to navigate forward (at index 0)");
            Assert.IsFalse(_history.CanNavigateBack(), "Should not be able to navigate back (at index 0)");
        }

        /// <summary>
        /// Test T038: RestoreState() with null state does not modify history.
        /// </summary>
        [Test]
        public void RestoreState_NullState_DoesNotModifyHistory()
        {
            // Arrange
            _history.Push(new SceneHistoryEntry("scene_001", 0));
            var originalCount = _history.Count;
            
            // Act
            _history.RestoreState(null);
            
            // Assert
            Assert.AreEqual(originalCount, _history.Count, "History should not be modified by null state");
        }
    }
}
