using UnityEngine;
using UnityEngine.UI;
using NovelCore.Runtime.Core.SceneManagement;

namespace NovelCore.Runtime.UI.NavigationControls
{

/// <summary>
/// Optional UI component for scene navigation (back/forward buttons).
/// Creators can use this or create their own navigation UI.
/// </summary>
public class SceneNavigationUI : MonoBehaviour
{
    [Header("Navigation Buttons")]
    [SerializeField]
    [Tooltip("Button to navigate to previous scene")]
    private Button _backButton;

    [SerializeField]
    [Tooltip("Button to navigate to next scene (forward in history)")]
    private Button _forwardButton;

    [Header("Visual Feedback")]
    [SerializeField]
    [Tooltip("Color for enabled buttons")]
    private Color _enabledColor = Color.white;

    [SerializeField]
    [Tooltip("Color for disabled buttons")]
    private Color _disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private ISceneManager _sceneManager;

    /// <summary>
    /// Initializes the navigation UI with a scene manager reference.
    /// Should be called by dependency injection or manual setup.
    /// </summary>
    public void Initialize(ISceneManager sceneManager)
    {
        _sceneManager = sceneManager ?? throw new System.ArgumentNullException(nameof(sceneManager));

        // Setup button listeners
        if (_backButton != null)
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }

        if (_forwardButton != null)
        {
            _forwardButton.onClick.AddListener(OnForwardButtonClicked);
        }

        Debug.Log("SceneNavigationUI: Initialized");
    }

    private void Update()
    {
        if (_sceneManager == null)
        {
            return;
        }

        // Update button states based on navigation availability
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        if (_backButton != null)
        {
            bool canGoBack = _sceneManager.CanNavigateBack();
            _backButton.interactable = canGoBack;
            
            // Update visual feedback
            var backImage = _backButton.GetComponent<Image>();
            if (backImage != null)
            {
                backImage.color = canGoBack ? _enabledColor : _disabledColor;
            }
        }

        if (_forwardButton != null)
        {
            bool canGoForward = _sceneManager.CanNavigateForward();
            _forwardButton.interactable = canGoForward;
            
            // Update visual feedback
            var forwardImage = _forwardButton.GetComponent<Image>();
            if (forwardImage != null)
            {
                forwardImage.color = canGoForward ? _enabledColor : _disabledColor;
            }
        }
    }

    private void OnBackButtonClicked()
    {
        if (_sceneManager == null)
        {
            Debug.LogWarning("SceneNavigationUI: Scene manager not initialized");
            return;
        }

        bool success = _sceneManager.NavigateBack();
        if (success)
        {
            Debug.Log("SceneNavigationUI: Navigated back");
        }
        else
        {
            Debug.LogWarning("SceneNavigationUI: Failed to navigate back");
        }
    }

    private void OnForwardButtonClicked()
    {
        if (_sceneManager == null)
        {
            Debug.LogWarning("SceneNavigationUI: Scene manager not initialized");
            return;
        }

        bool success = _sceneManager.NavigateForward();
        if (success)
        {
            Debug.Log("SceneNavigationUI: Navigated forward");
        }
        else
        {
            Debug.LogWarning("SceneNavigationUI: Failed to navigate forward");
        }
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }

        if (_forwardButton != null)
        {
            _forwardButton.onClick.RemoveListener(OnForwardButtonClicked);
        }
    }
}

}
