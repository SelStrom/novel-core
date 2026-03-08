using System;
using System.Collections.Generic;
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Runtime.Core.SceneManagement
{

/// <summary>
/// Implementation of scene navigation history with stack-based back/forward support.
/// </summary>
public class SceneNavigationHistory : ISceneNavigationHistory
{
    private SceneNavigationState _state;
    private const int DEFAULT_MAX_HISTORY_SIZE = 50;

    public int Count => _state?.history?.Count ?? 0;

    /// <summary>
    /// Creates a new navigation history with default max size.
    /// This parameterless constructor is required for VContainer DI.
    /// </summary>
    public SceneNavigationHistory() : this(DEFAULT_MAX_HISTORY_SIZE)
    {
    }

    /// <summary>
    /// Creates a new navigation history with custom max size.
    /// </summary>
    public SceneNavigationHistory(int maxHistorySize)
    {
        _state = new SceneNavigationState
        {
            maxHistorySize = maxHistorySize
        };
    }

    public void Push(SceneHistoryEntry entry)
    {
        if (entry == null || !entry.IsValid())
        {
            Debug.LogWarning("SceneNavigationHistory: Cannot push invalid entry");
            return;
        }

        // If we're in the middle of history (navigated back), remove forward entries
        if (_state.currentIndex < _state.history.Count - 1)
        {
            int removeCount = _state.history.Count - _state.currentIndex - 1;
            _state.history.RemoveRange(_state.currentIndex + 1, removeCount);
            Debug.Log($"SceneNavigationHistory: Removed {removeCount} forward entries");
        }

        // Add new entry
        _state.history.Add(entry);
        _state.currentIndex = _state.history.Count - 1;

        // Enforce max size by removing oldest entries
        if (_state.history.Count > _state.maxHistorySize)
        {
            int removeCount = _state.history.Count - _state.maxHistorySize;
            _state.history.RemoveRange(0, removeCount);
            _state.currentIndex -= removeCount;
            Debug.Log($"SceneNavigationHistory: Removed {removeCount} oldest entries (max size: {_state.maxHistorySize})");
        }

        Debug.Log($"SceneNavigationHistory: Pushed entry for scene '{entry.sceneId}' at line {entry.dialogueLineIndex} (total: {_state.history.Count})");
    }

    public SceneHistoryEntry Pop()
    {
        if (_state.history.Count == 0)
        {
            Debug.LogWarning("SceneNavigationHistory: Cannot pop from empty history");
            return null;
        }

        var entry = _state.history[_state.history.Count - 1];
        _state.history.RemoveAt(_state.history.Count - 1);
        
        if (_state.currentIndex >= _state.history.Count)
        {
            _state.currentIndex = _state.history.Count - 1;
        }

        Debug.Log($"SceneNavigationHistory: Popped entry for scene '{entry.sceneId}'");
        return entry;
    }

    public SceneHistoryEntry Peek()
    {
        if (_state.history.Count == 0)
        {
            return null;
        }

        return _state.history[_state.history.Count - 1];
    }

    public bool CanNavigateBack()
    {
        return _state.currentIndex > 0;
    }

    public bool CanNavigateForward()
    {
        return _state.currentIndex >= 0 && _state.currentIndex < _state.history.Count - 1;
    }

    public SceneHistoryEntry NavigateBack()
    {
        if (!CanNavigateBack())
        {
            Debug.LogWarning("SceneNavigationHistory: Cannot navigate back (at beginning of history)");
            return null;
        }

        _state.currentIndex--;
        var entry = _state.history[_state.currentIndex];
        Debug.Log($"SceneNavigationHistory: Navigated back to scene '{entry.sceneId}' (index: {_state.currentIndex})");
        return entry;
    }

    public SceneHistoryEntry NavigateForward()
    {
        if (!CanNavigateForward())
        {
            Debug.LogWarning("SceneNavigationHistory: Cannot navigate forward (at end of history)");
            return null;
        }

        _state.currentIndex++;
        var entry = _state.history[_state.currentIndex];
        Debug.Log($"SceneNavigationHistory: Navigated forward to scene '{entry.sceneId}' (index: {_state.currentIndex})");
        return entry;
    }

    public void Clear()
    {
        _state.Clear();
        Debug.Log("SceneNavigationHistory: History cleared");
    }

    public SceneNavigationState GetState()
    {
        return _state;
    }

    public void RestoreState(SceneNavigationState state)
    {
        if (state == null || !state.IsValid())
        {
            Debug.LogWarning("SceneNavigationHistory: Cannot restore invalid state");
            return;
        }

        _state = state;
        Debug.Log($"SceneNavigationHistory: State restored ({state.history.Count} entries, currentIndex: {state.currentIndex})");
    }
}

}
