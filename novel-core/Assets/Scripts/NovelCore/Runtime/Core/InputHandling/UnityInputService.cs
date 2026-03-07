using UnityEngine.InputSystem;

namespace NovelCore.Runtime.Core.InputHandling
{

/// <summary>
/// Unity Input System implementation of IInputService.
/// Handles mouse, touch, and keyboard inputs using the new Input System.
/// </summary>
public class UnityInputService : MonoBehaviour, IInputService
{
    // Events
    public event System.Action OnPrimaryAction;
    public event System.Action OnSecondaryAction;
    public event System.Action OnCancelAction;

    // Properties
    public Vector2 PointerPosition { get; private set; }
    public bool WasPrimaryActionPerformed { get; private set; }
    public bool IsPointerPressed { get; private set; }
    public bool InputEnabled { get; set; } = true;

    private InputAction _primaryAction;
    private InputAction _secondaryAction;
    private InputAction _cancelAction;
    private InputAction _pointerPosition;
    private InputAction _pointerPress;

    private void Awake()
    {
        InitializeInputActions();
        Debug.Log("UnityInputService: Initialized");
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void Update()
    {
        if (!InputEnabled)
        {
            WasPrimaryActionPerformed = false;
            return;
        }

        // Update pointer position
        PointerPosition = _pointerPosition.ReadValue<Vector2>();

        // Update pointer press state
        IsPointerPressed = _pointerPress.ReadValue<float>() > 0.5f;

        // Check for primary action
        WasPrimaryActionPerformed = _primaryAction.WasPressedThisFrame();
        if (WasPrimaryActionPerformed)
        {
            OnPrimaryAction?.Invoke();
            Debug.Log("UnityInputService: Primary action performed");
        }

        // Check for secondary action
        if (_secondaryAction.WasPressedThisFrame())
        {
            OnSecondaryAction?.Invoke();
            Debug.Log("UnityInputService: Secondary action performed");
        }

        // Check for cancel action
        if (_cancelAction.WasPressedThisFrame())
        {
            OnCancelAction?.Invoke();
            Debug.Log("UnityInputService: Cancel action performed");
        }
    }

    private void InitializeInputActions()
    {
        // Primary action: Left mouse button / Touch / Space / Enter
        _primaryAction = new InputAction("PrimaryAction", binding: "<Mouse>/leftButton");
        _primaryAction.AddBinding("<Touchscreen>/primaryTouch/tap");
        _primaryAction.AddBinding("<Keyboard>/space");
        _primaryAction.AddBinding("<Keyboard>/enter");

        // Secondary action: Right mouse button / Two-finger tap
        _secondaryAction = new InputAction("SecondaryAction", binding: "<Mouse>/rightButton");
        _secondaryAction.AddCompositeBinding("TwoModifiers")
            .With("modifier1", "<Touchscreen>/touch0/press")
            .With("modifier2", "<Touchscreen>/touch1/press")
            .With("binding", "<Touchscreen>/primaryTouch/tap");

        // Cancel action: Escape / Back button
        _cancelAction = new InputAction("CancelAction", binding: "<Keyboard>/escape");
        _cancelAction.AddBinding("<Gamepad>/buttonEast");

        // Pointer position: Mouse / Touch
        _pointerPosition = new InputAction("PointerPosition", binding: "<Mouse>/position");
        _pointerPosition.AddBinding("<Touchscreen>/primaryTouch/position");

        // Pointer press state
        _pointerPress = new InputAction("PointerPress", binding: "<Mouse>/leftButton");
        _pointerPress.AddBinding("<Touchscreen>/primaryTouch/press");
    }

    private void EnableInputActions()
    {
        _primaryAction?.Enable();
        _secondaryAction?.Enable();
        _cancelAction?.Enable();
        _pointerPosition?.Enable();
        _pointerPress?.Enable();
    }

    private void DisableInputActions()
    {
        _primaryAction?.Disable();
        _secondaryAction?.Disable();
        _cancelAction?.Disable();
        _pointerPosition?.Disable();
        _pointerPress?.Disable();
    }

    private void OnDestroy()
    {
        DisableInputActions();
        _primaryAction?.Dispose();
        _secondaryAction?.Dispose();
        _cancelAction?.Dispose();
        _pointerPosition?.Dispose();
        _pointerPress?.Dispose();
    }
}

}