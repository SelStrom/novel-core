using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using NovelCore.Runtime.Core.DialogueSystem;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Data.Scenes;

namespace NovelCore.Runtime.Core
{

/// <summary>
/// Initializes the game and loads the first scene.
/// Attach this to a GameObject in the main Unity scene.
/// </summary>
public class GameStarter : MonoBehaviour
{
    [Header("Starting Scene")]
    [SerializeField]
    [Tooltip("The first scene to load when the game starts (SceneData asset)")]
    private SceneData _startingScene;

    [SerializeField]
    [Tooltip("Alternative: Addressable reference to the starting scene")]
    private AssetReference _startingSceneReference;

    [Header("Auto Start")]
    [SerializeField]
    [Tooltip("Automatically start the game on Awake")]
    private bool _autoStart = true;

    [SerializeField]
    [Tooltip("Delay before starting (in seconds)")]
    private float _startDelay = 0.5f;

    [Inject]
    private IDialogueSystem _dialogueSystem;

    [Inject]
    private ISceneManager _sceneManager;

    private bool _hasStarted = false;
    private bool _isSubscribed = false;

    private void Start()
    {
        if (_autoStart)
        {
            Invoke(nameof(StartGame), _startDelay);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (_dialogueSystem != null && _isSubscribed)
        {
            _dialogueSystem.OnSceneNavigationRequested -= HandleSceneNavigation;
            _isSubscribed = false;
        }
    }

    /// <summary>
    /// Handles scene navigation requests from DialogueSystem (choice selection or linear progression).
    /// </summary>
    private void HandleSceneNavigation(SceneData targetScene)
    {
        if (targetScene == null)
        {
            Debug.LogError("GameStarter: Cannot navigate to null scene");
            return;
        }

        Debug.Log($"GameStarter: Navigating to scene: {targetScene.SceneName}");

        // Load the new scene visually (background, characters, music)
        _sceneManager.LoadScene(targetScene);

        // Start dialogue for the new scene
        _dialogueSystem.StartScene(targetScene);
    }

    /// <summary>
    /// Starts the game by loading the starting scene.
    /// Can be called manually if autoStart is false.
    /// </summary>
    public void StartGame()
    {
        if (_hasStarted)
        {
            Debug.LogWarning("GameStarter: Game already started, ignoring duplicate call");
            return;
        }

        SceneData sceneToLoad = GetSceneToLoad();
        
        if (sceneToLoad == null)
        {
            Debug.LogError("GameStarter: No scene to load! Please assign a SceneData asset in the Inspector.");
            return;
        }

        if (_dialogueSystem == null)
        {
            Debug.LogError("GameStarter: DialogueSystem not injected! Make sure GameLifetimeScope is present in the scene.");
            return;
        }

        if (_sceneManager == null)
        {
            Debug.LogError("GameStarter: SceneManager not injected! Make sure GameLifetimeScope is present in the scene.");
            return;
        }

        // Subscribe to scene navigation events BEFORE starting the game
        if (!_isSubscribed)
        {
            _dialogueSystem.OnSceneNavigationRequested += HandleSceneNavigation;
            _isSubscribed = true;
            Debug.Log("GameStarter: Subscribed to OnSceneNavigationRequested event");
        }

        _hasStarted = true;

        Debug.Log($"GameStarter: Starting game with scene: {sceneToLoad.SceneName}");

        // Load the scene visually (background, characters, music)
        _sceneManager.LoadScene(sceneToLoad);

        // Start the dialogue system
        _dialogueSystem.StartScene(sceneToLoad);
    }
    
    /// <summary>
    /// Determines which scene to load on game start.
    /// Priority order:
    /// 1. Preview scene (from Scene Editor "Preview" button)
    /// 2. Selected scene (from Project Window selection)
    /// 3. Default starting scene (from Inspector field)
    /// </summary>
    /// <returns>SceneData to load</returns>
    private SceneData GetSceneToLoad()
    {
        // 1. Check for preview mode (Constitution Principle VIII: Editor-Runtime Bridge)
        SceneData previewScene = PreviewManager.GetPreviewScene();
        if (previewScene != null)
        {
            Debug.Log($"[Preview Mode] Loading preview scene: {previewScene.SceneName}");
            return previewScene;
        }
        
        // 2. Check for selected SceneData in Project Window (Editor only)
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeObject is SceneData selectedScene)
        {
            Debug.Log($"GameStarter: Loading selected scene: {selectedScene.SceneName}");
            return selectedScene;
        }
        #endif
        
        // 3. Fallback to default starting scene from Inspector
        if (_startingScene != null)
        {
            Debug.Log($"GameStarter: Loading default starting scene: {_startingScene.SceneName}");
        }
        
        return _startingScene;
    }

    /// <summary>
    /// Restart the game from the beginning.
    /// </summary>
    public void RestartGame()
    {
        _hasStarted = false;
        _dialogueSystem.StopDialogue();
        _sceneManager.UnloadCurrentScene();
        StartGame();
    }

    private void OnValidate()
    {
        // Validate in editor
        if (_startingScene == null && (_startingSceneReference == null || !_startingSceneReference.RuntimeKeyIsValid()))
        {
            Debug.LogWarning("GameStarter: No starting scene assigned. Assign a SceneData asset to start the game.");
        }
    }
}

}
