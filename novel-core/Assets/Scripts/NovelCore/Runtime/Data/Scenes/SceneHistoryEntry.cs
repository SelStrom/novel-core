using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovelCore.Runtime.Data.Scenes
{

/// <summary>
/// Represents a single entry in the scene navigation history.
/// Stores scene state for back/forward navigation.
/// </summary>
[Serializable]
public class SceneHistoryEntry
{
    /// <summary>
    /// Unique identifier of the scene.
    /// </summary>
    public string sceneId;

    /// <summary>
    /// Reference to the actual SceneData object for loading.
    /// Note: This is not serialized to save files (use sceneId for that).
    /// </summary>
    [NonSerialized]
    public SceneData sceneData;

    /// <summary>
    /// Current dialogue line index when the scene was visited.
    /// </summary>
    public int dialogueLineIndex;

    /// <summary>
    /// Snapshot of game state (flags, variables) at this point.
    /// Stored as serializable dictionary for save/load compatibility.
    /// </summary>
    public Dictionary<string, object> gameStateSnapshot;

    /// <summary>
    /// Timestamp when this entry was created (for debugging).
    /// </summary>
    public long timestamp;

    /// <summary>
    /// Creates a new scene history entry.
    /// </summary>
    public SceneHistoryEntry(string sceneId, int dialogueLineIndex, Dictionary<string, object> gameStateSnapshot = null, SceneData sceneData = null)
    {
        this.sceneId = sceneId;
        this.sceneData = sceneData;
        this.dialogueLineIndex = dialogueLineIndex;
        this.gameStateSnapshot = gameStateSnapshot ?? new Dictionary<string, object>();
        this.timestamp = DateTime.UtcNow.Ticks;
    }

    /// <summary>
    /// Validates that the history entry has valid data.
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(sceneId) && dialogueLineIndex >= 0;
    }
}

}
