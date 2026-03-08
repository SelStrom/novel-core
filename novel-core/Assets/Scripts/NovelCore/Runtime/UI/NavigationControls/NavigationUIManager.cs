using UnityEngine;
using VContainer;
using NovelCore.Runtime.Core.SceneManagement;
using NovelCore.Runtime.Core.DialogueSystem;

namespace NovelCore.Runtime.UI.NavigationControls
{

/// <summary>
/// Manager component that initializes SceneNavigationUI with dependency injection.
/// Bridges the gap between VContainer DI and Unity MonoBehaviour lifecycle.
/// Also handles integration with DialogueSystem for scene transitions.
/// </summary>
public class NavigationUIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to SceneNavigationUI component to initialize")]
    private SceneNavigationUI _navigationUI;

    [Inject]
    private ISceneManager _sceneManager;

    [Inject]
    private IDialogueSystem _dialogueSystem;

    private bool _isInitialized = false;

    private void Start()
    {
        InitializeNavigationUI();
        SubscribeToNavigationEvents();
    }

    private void InitializeNavigationUI()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("NavigationUIManager: Already initialized, skipping");
            return;
        }

        if (_navigationUI == null)
        {
            Debug.LogError("NavigationUIManager: SceneNavigationUI reference is not assigned! Please assign it in the Inspector.");
            return;
        }

        if (_sceneManager == null)
        {
            Debug.LogError("NavigationUIManager: ISceneManager not injected! Make sure GameLifetimeScope is present in the scene.");
            return;
        }

        if (_dialogueSystem == null)
        {
            Debug.LogError("NavigationUIManager: IDialogueSystem not injected! Make sure GameLifetimeScope is present in the scene.");
            return;
        }

        // Initialize the navigation UI with the scene manager
        _navigationUI.Initialize(_sceneManager);
        _isInitialized = true;

        Debug.Log("NavigationUIManager: Successfully initialized SceneNavigationUI");
    }

    private void SubscribeToNavigationEvents()
    {
        if (_sceneManager == null) return;

        // Subscribe to scene load complete event to restart dialogue
        _sceneManager.OnSceneLoadComplete += OnSceneNavigated;
        Debug.Log("NavigationUIManager: Subscribed to scene navigation events");
    }

    private void OnSceneNavigated(Data.Scenes.SceneData sceneData)
    {
        if (_dialogueSystem == null || sceneData == null) return;

        // Restart dialogue for the navigated scene
        // This ensures dialogue system is in sync with the loaded scene
        if (!_dialogueSystem.IsPlaying || _dialogueSystem.CurrentScene != sceneData)
        {
            Debug.Log($"NavigationUIManager: Restarting dialogue for scene '{sceneData.SceneName}'");
            _dialogueSystem.StartScene(sceneData);
        }
    }

    private void OnDestroy()
    {
        if (_sceneManager != null)
        {
            _sceneManager.OnSceneLoadComplete -= OnSceneNavigated;
        }
    }

    private void OnValidate()
    {
        // Auto-find SceneNavigationUI if not assigned
        if (_navigationUI == null)
        {
            _navigationUI = FindFirstObjectByType<SceneNavigationUI>();
            if (_navigationUI != null)
            {
                Debug.Log("NavigationUIManager: Auto-found SceneNavigationUI component");
            }
        }
    }
}

}
