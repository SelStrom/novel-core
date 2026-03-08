using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Runtime.Core.SceneManagement
{

/// <summary>
/// Interface for scene navigation history management.
/// Provides stack-based navigation with back/forward support.
/// </summary>
public interface ISceneNavigationHistory
{
    /// <summary>
    /// Gets the number of entries in the history.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Pushes a new scene entry onto the history stack.
    /// Automatically removes entries exceeding max size.
    /// </summary>
    void Push(SceneHistoryEntry entry);

    /// <summary>
    /// Removes and returns the most recent entry from history.
    /// </summary>
    SceneHistoryEntry Pop();

    /// <summary>
    /// Returns the most recent entry without removing it.
    /// </summary>
    SceneHistoryEntry Peek();

    /// <summary>
    /// Checks if navigation back is possible.
    /// </summary>
    bool CanNavigateBack();

    /// <summary>
    /// Checks if navigation forward is possible.
    /// </summary>
    bool CanNavigateForward();

    /// <summary>
    /// Navigates back in history and returns the previous entry.
    /// </summary>
    SceneHistoryEntry NavigateBack();

    /// <summary>
    /// Navigates forward in history and returns the next entry.
    /// </summary>
    SceneHistoryEntry NavigateForward();

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets the current navigation state for serialization.
    /// </summary>
    SceneNavigationState GetState();

    /// <summary>
    /// Restores navigation state from serialized data.
    /// </summary>
    void RestoreState(SceneNavigationState state);
}

}
