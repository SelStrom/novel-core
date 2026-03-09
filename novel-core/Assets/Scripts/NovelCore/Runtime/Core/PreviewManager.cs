#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Runtime.Core
{

/// <summary>
/// Manages preview mode state transfer from Unity Editor to Play Mode.
/// Enables Scene Editor "Preview Scene" functionality by bridging editor tools and runtime systems.
/// </summary>
/// <remarks>
/// Constitution Principle VIII (Editor-Runtime Bridge):
/// - Editor tools MUST transfer preview state to runtime Play Mode
/// - GameStarter MUST check preview state on initialization
/// - Preview state MUST be cleared after consumption to prevent stale data
/// 
/// Usage:
/// - Editor: PreviewManager.SetPreviewScene(sceneData) before entering Play Mode
/// - Runtime: PreviewManager.GetPreviewScene() returns preview scene or null
/// - Automatic cleanup after GetPreviewScene() to prevent re-use
/// </remarks>
public static class PreviewManager
{
    private const string PREVIEW_SCENE_KEY = "NovelCore_PreviewScene";
    private const string PREVIEW_DIALOGUE_INDEX_KEY = "NovelCore_PreviewDialogueIndex";
    
    /// <summary>
    /// Returns true if preview mode is active (preview scene is set in EditorPrefs).
    /// </summary>
    public static bool IsPreviewMode
    {
        get
        {
            #if UNITY_EDITOR
            return EditorPrefs.HasKey(PREVIEW_SCENE_KEY);
            #else
            return false;
            #endif
        }
    }
    
    /// <summary>
    /// Gets the preview scene from EditorPrefs and clears it after retrieval.
    /// Returns null if no preview scene is set or if the asset path is invalid.
    /// </summary>
    /// <returns>SceneData asset to preview, or null if not in preview mode</returns>
    public static SceneData GetPreviewScene()
    {
        #if UNITY_EDITOR
        if (!IsPreviewMode)
        {
            return null;
        }
            
        string path = EditorPrefs.GetString(PREVIEW_SCENE_KEY);
        
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("PreviewManager: Preview scene path is empty");
            ClearPreviewData();
            return null;
        }
        
        SceneData scene = AssetDatabase.LoadAssetAtPath<SceneData>(path);
        
        if (scene == null)
        {
            Debug.LogWarning($"PreviewManager: Failed to load preview scene at path: {path}");
            ClearPreviewData();
            return null;
        }
        
        Debug.Log($"[Preview Mode] Loading scene: {scene.SceneName} (path: {path})");
        
        // Clear after successful retrieval to prevent re-use
        ClearPreviewData();
        
        return scene;
        #else
        return null;
        #endif
    }
    
    /// <summary>
    /// Sets the preview scene to be loaded when Play Mode starts.
    /// Should be called from Editor scripts before entering Play Mode.
    /// </summary>
    /// <param name="scene">SceneData asset to preview</param>
    public static void SetPreviewScene(SceneData scene)
    {
        #if UNITY_EDITOR
        if (scene == null)
        {
            Debug.LogError("PreviewManager: Cannot set null scene for preview");
            return;
        }
        
        string path = AssetDatabase.GetAssetPath(scene);
        
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError($"PreviewManager: Scene {scene.SceneName} has invalid asset path");
            return;
        }
        
        EditorPrefs.SetString(PREVIEW_SCENE_KEY, path);
        Debug.Log($"[Preview Mode] Set preview scene: {scene.SceneName} (path: {path})");
        #endif
    }
    
    /// <summary>
    /// Clears all preview data from EditorPrefs.
    /// Called automatically after GetPreviewScene() or manually to reset state.
    /// </summary>
    public static void ClearPreviewData()
    {
        #if UNITY_EDITOR
        if (EditorPrefs.HasKey(PREVIEW_SCENE_KEY))
        {
            EditorPrefs.DeleteKey(PREVIEW_SCENE_KEY);
            Debug.Log("[Preview Mode] Cleared preview data");
        }
        
        if (EditorPrefs.HasKey(PREVIEW_DIALOGUE_INDEX_KEY))
        {
            EditorPrefs.DeleteKey(PREVIEW_DIALOGUE_INDEX_KEY);
        }
        #endif
    }
    
    /// <summary>
    /// Gets the preview dialogue index (for future feature: preview from specific dialogue line).
    /// Returns null if not set.
    /// </summary>
    /// <returns>Dialogue index to start from, or null</returns>
    public static int? GetPreviewDialogueIndex()
    {
        #if UNITY_EDITOR
        if (EditorPrefs.HasKey(PREVIEW_DIALOGUE_INDEX_KEY))
        {
            return EditorPrefs.GetInt(PREVIEW_DIALOGUE_INDEX_KEY);
        }
        #endif
        return null;
    }
    
    /// <summary>
    /// Sets the preview dialogue index (for future feature: preview from specific dialogue line).
    /// </summary>
    /// <param name="dialogueIndex">Dialogue line index to start preview from</param>
    public static void SetPreviewDialogueIndex(int dialogueIndex)
    {
        #if UNITY_EDITOR
        EditorPrefs.SetInt(PREVIEW_DIALOGUE_INDEX_KEY, dialogueIndex);
        Debug.Log($"[Preview Mode] Set preview dialogue index: {dialogueIndex}");
        #endif
    }
}

}
