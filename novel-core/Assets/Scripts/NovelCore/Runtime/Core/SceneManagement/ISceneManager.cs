namespace NovelCore.Runtime.Core.SceneManagement;

/// <summary>
/// Interface for scene management system handling scene loading, rendering, and transitions.
/// </summary>
public interface ISceneManager
{
    /// <summary>
    /// Currently loaded scene.
    /// </summary>
    SceneData CurrentScene { get; }

    /// <summary>
    /// Whether a scene is currently loading.
    /// </summary>
    bool IsLoading { get; }

    /// <summary>
    /// Event triggered when a scene starts loading.
    /// </summary>
    event System.Action<SceneData> OnSceneLoadStart;

    /// <summary>
    /// Event triggered when a scene finishes loading.
    /// </summary>
    event System.Action<SceneData> OnSceneLoadComplete;

    /// <summary>
    /// Event triggered when a scene transition begins.
    /// </summary>
    event System.Action<SceneData> OnSceneTransitionStart;

    /// <summary>
    /// Event triggered when a scene transition completes.
    /// </summary>
    event System.Action<SceneData> OnSceneTransitionComplete;

    /// <summary>
    /// Load and display a scene.
    /// </summary>
    /// <param name="sceneData">Scene to load.</param>
    void LoadScene(SceneData sceneData);

    /// <summary>
    /// Load and display a scene asynchronously.
    /// </summary>
    /// <param name="sceneData">Scene to load.</param>
    System.Threading.Tasks.Task LoadSceneAsync(SceneData sceneData);

    /// <summary>
    /// Unload the current scene and clean up resources.
    /// </summary>
    void UnloadCurrentScene();

    /// <summary>
    /// Preload scene assets in the background.
    /// </summary>
    /// <param name="sceneData">Scene to preload.</param>
    System.Threading.Tasks.Task PreloadSceneAsync(SceneData sceneData);

    /// <summary>
    /// Update character emotion/sprite in current scene.
    /// </summary>
    /// <param name="characterId">Character to update.</param>
    /// <param name="emotion">New emotion to display.</param>
    void UpdateCharacterEmotion(string characterId, string emotion);

    /// <summary>
    /// Show or hide a character in the current scene.
    /// </summary>
    /// <param name="characterId">Character to show/hide.</param>
    /// <param name="visible">True to show, false to hide.</param>
    void SetCharacterVisible(string characterId, bool visible);

    /// <summary>
    /// Move a character to a new position with optional animation.
    /// </summary>
    /// <param name="characterId">Character to move.</param>
    /// <param name="targetPosition">Target position (normalized 0-1).</param>
    /// <param name="duration">Animation duration in seconds.</param>
    void MoveCharacter(string characterId, Vector2 targetPosition, float duration = 0.5f);
}
