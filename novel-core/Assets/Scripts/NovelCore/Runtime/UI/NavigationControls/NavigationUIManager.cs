using UnityEngine;
using VContainer;
using NovelCore.Runtime.Core.SceneManagement;

namespace NovelCore.Runtime.UI.NavigationControls
{

/// <summary>
/// Manager component that initializes SceneNavigationUI with dependency injection.
/// Bridges the gap between VContainer DI and Unity MonoBehaviour lifecycle.
/// </summary>
public class NavigationUIManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to SceneNavigationUI component to initialize")]
    private SceneNavigationUI _navigationUI;

    [Inject]
    private ISceneManager _sceneManager;

    private bool _isInitialized = false;

    private void Start()
    {
        InitializeNavigationUI();
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

        // Initialize the navigation UI with the scene manager
        _navigationUI.Initialize(_sceneManager);
        _isInitialized = true;

        Debug.Log("NavigationUIManager: Successfully initialized SceneNavigationUI");
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
