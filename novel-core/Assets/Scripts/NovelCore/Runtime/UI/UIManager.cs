using UnityEngine;
using VContainer;
using VContainer.Unity;
using NovelCore.Runtime.Core.DialogueSystem;
using NovelCore.Runtime.Core.InputHandling;
using NovelCore.Runtime.Core.Localization;
using NovelCore.Runtime.UI.DialogueBox;

namespace NovelCore.Runtime.UI
{

/// <summary>
/// Manages UI components and bridges them with core systems via dependency injection.
/// Handles initialization of DialogueBox and other UI elements.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    [Tooltip("Reference to DialogueBoxController (will be auto-found if null)")]
    private DialogueBoxController _dialogueBoxController;

    [SerializeField]
    [Tooltip("Canvas for UI elements (will be auto-found if null)")]
    private Canvas _canvas;

    [Header("DialogueBox Prefab")]
    [SerializeField]
    [Tooltip("Prefab to instantiate if DialogueBox not found in scene")]
    private GameObject _dialogueBoxPrefab;

    [Inject]
    private IDialogueSystem _dialogueSystem;

    [Inject]
    private IInputService _inputService;

    [Inject]
    private ILocalizationService _localizationService;

    private bool _isInitialized = false;

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("UIManager: Already initialized, skipping");
            return;
        }

        Debug.Log("UIManager: Initializing UI...");

        // Find or create Canvas
        if (_canvas == null)
        {
            _canvas = FindFirstObjectByType<Canvas>();
            
            if (_canvas == null)
            {
                Debug.LogWarning("UIManager: No Canvas found in scene, creating one");
                _canvas = CreateCanvas();
            }
        }

        // Find or create DialogueBox
        if (_dialogueBoxController == null)
        {
            _dialogueBoxController = FindFirstObjectByType<DialogueBoxController>();
            
            if (_dialogueBoxController == null)
            {
                Debug.Log("UIManager: DialogueBoxController not found, attempting to create from prefab or Resources");
                _dialogueBoxController = CreateDialogueBox();
            }
        }

        // Initialize DialogueBox with dependencies
        if (_dialogueBoxController != null)
        {
            if (_dialogueSystem == null)
            {
                Debug.LogError("UIManager: DialogueSystem not injected! Make sure GameLifetimeScope is present.");
                return;
            }

            _dialogueBoxController.Initialize(_dialogueSystem, _localizationService);
            Debug.Log("UIManager: DialogueBoxController initialized successfully");
        }
        else
        {
            Debug.LogError("UIManager: Failed to find or create DialogueBoxController!");
            return;
        }

        // Subscribe to input events
        if (_inputService != null)
        {
            _inputService.OnPrimaryAction += OnPrimaryInput;
            Debug.Log("UIManager: Subscribed to input events");
        }
        else
        {
            Debug.LogWarning("UIManager: InputService not available, manual input handling disabled");
        }

        _isInitialized = true;
        Debug.Log("UIManager: UI initialization complete");
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        // Add CanvasScaler for responsive UI
        var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // Add GraphicRaycaster for UI input
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        Debug.Log("UIManager: Created Canvas");
        return canvas;
    }

    private DialogueBoxController CreateDialogueBox()
    {
        GameObject dialogueBoxObj = null;

        // Try to instantiate from assigned prefab
        if (_dialogueBoxPrefab != null)
        {
            dialogueBoxObj = Instantiate(_dialogueBoxPrefab, _canvas.transform);
            Debug.Log("UIManager: Instantiated DialogueBox from assigned prefab");
        }
        else
        {
            // Try to load from Resources
            GameObject prefab = Resources.Load<GameObject>("NovelCore/UI/DialogueBox");
            if (prefab != null)
            {
                dialogueBoxObj = Instantiate(prefab, _canvas.transform);
                Debug.Log("UIManager: Instantiated DialogueBox from Resources");
            }
            else
            {
                Debug.LogError("UIManager: DialogueBox prefab not found! Generate it via NovelCore → Generate UI Prefabs → Dialogue Box");
                return null;
            }
        }

        if (dialogueBoxObj != null)
        {
            var controller = dialogueBoxObj.GetComponent<DialogueBoxController>();
            if (controller == null)
            {
                Debug.LogError("UIManager: DialogueBox prefab missing DialogueBoxController component!");
                return null;
            }
            return controller;
        }

        return null;
    }

    private void OnPrimaryInput()
    {
        if (!_isInitialized || _dialogueSystem == null || _dialogueBoxController == null)
        {
            return;
        }

        // If dialogue is playing and not waiting for choice, advance
        if (_dialogueSystem.IsPlaying && !_dialogueSystem.IsWaitingForChoice)
        {
            _dialogueSystem.AdvanceDialogue();
        }
    }

    private void Update()
    {
        // Update DialogueSystem (for auto-advance)
        if (_dialogueSystem != null)
        {
            _dialogueSystem.Update(Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from input events
        if (_inputService != null)
        {
            _inputService.OnPrimaryAction -= OnPrimaryInput;
        }
    }

    private void OnValidate()
    {
        // Find Canvas in scene if not assigned
        if (_canvas == null)
        {
            _canvas = FindFirstObjectByType<Canvas>();
        }

        // Find DialogueBoxController in scene if not assigned
        if (_dialogueBoxController == null)
        {
            _dialogueBoxController = FindFirstObjectByType<DialogueBoxController>();
        }
    }
}

}
