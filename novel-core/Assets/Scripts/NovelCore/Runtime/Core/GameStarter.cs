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

    private void Start()
    {
        if (_autoStart)
        {
            Invoke(nameof(StartGame), _startDelay);
        }
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

        if (_startingScene == null)
        {
            Debug.LogError("GameStarter: Starting scene is not assigned! Please assign a SceneData asset in the Inspector.");
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

        _hasStarted = true;

        Debug.Log($"GameStarter: Starting game with scene: {_startingScene.SceneName}");

        // Load the scene visually (background, characters, music)
        _sceneManager.LoadScene(_startingScene);

        // Start the dialogue system
        _dialogueSystem.StartScene(_startingScene);
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
