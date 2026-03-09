using System;
using System.Collections.Generic;

namespace NovelCore.Runtime.Data.Scenes
{

/// <summary>
/// Serializable state for scene navigation history.
/// Used for save/load persistence of navigation state.
/// </summary>
[Serializable]
public class SceneNavigationState
{
    /// <summary>
    /// List of visited scenes in order.
    /// </summary>
    public List<SceneHistoryEntry> history = new();

    /// <summary>
    /// Current position in the history stack.
    /// Used for forward navigation after going back.
    /// </summary>
    public int currentIndex = -1;

    /// <summary>
    /// Maximum number of history entries to keep in memory.
    /// Default: 50 entries (prevents unlimited memory growth).
    /// </summary>
    public int maxHistorySize = 50;

    /// <summary>
    /// Creates a new empty navigation state.
    /// </summary>
    public SceneNavigationState()
    {
        history = new List<SceneHistoryEntry>();
        currentIndex = -1;
        maxHistorySize = 50;
    }

    /// <summary>
    /// Validates that the navigation state is consistent.
    /// </summary>
    public bool IsValid()
    {
        if (history == null)
            return false;

        if (currentIndex < -1 || currentIndex >= history.Count)
            return false;

        if (maxHistorySize <= 0)
            return false;

        return true;
    }

    /// <summary>
    /// Clears all navigation history.
    /// </summary>
    public void Clear()
    {
        history.Clear();
        currentIndex = -1;
    }

    /// <summary>
    /// Gets the current history entry if available.
    /// </summary>
    public SceneHistoryEntry GetCurrentEntry()
    {
        if (currentIndex >= 0 && currentIndex < history.Count)
        {
            return history[currentIndex];
        }
        return null;
    }

    /// <summary>
    /// Checks if we can navigate back in history.
    /// </summary>
    public bool CanNavigateBack()
    {
        return currentIndex > 0;
    }

    /// <summary>
    /// Checks if we can navigate forward in history.
    /// </summary>
    public bool CanNavigateForward()
    {
        return currentIndex >= 0 && currentIndex < history.Count - 1;
    }
}

}
