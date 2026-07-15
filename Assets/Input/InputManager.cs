using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions inputActions;

    public Vector2 MousePosition =>
        inputActions.Player.Point.ReadValue<Vector2>();

    public Vector2 CameraMovement =>
        inputActions.Player.CameraMove.ReadValue<Vector2>();

    public Vector2 MouseScroll =>
    inputActions.Player.CameraZoom.ReadValue<Vector2>();
    public event Action LeftClick;

    // Confirm/Cancel for gameplay flows that sit outside the UI button
    // menu (e.g. confirming a movement preview, cancelling mid-action).
    // Polled directly against Keyboard/Gamepad rather than added to the
    // generated PlayerInputActions asset, so this works without editing
    // that asset - swap for a proper rebindable "Confirm"/"Cancel" action
    // later if needed.
    public event Action ConfirmPressed;
    public event Action CancelPressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Click.performed += OnLeftClick;
    }

    private void OnDisable()
    {
        inputActions.Player.Click.performed -= OnLeftClick;

        inputActions.Disable();
    }

    private void Update()
    {
        if (WasConfirmPressedThisFrame())
        {
            ConfirmPressed?.Invoke();
        }

        if (WasCancelPressedThisFrame())
        {
            CancelPressed?.Invoke();
        }
    }

    private bool WasConfirmPressedThisFrame()
    {
        bool keyboard = Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame ||
             Keyboard.current.numpadEnterKey.wasPressedThisFrame);

        bool gamepad = Gamepad.current != null &&
            Gamepad.current.buttonSouth.wasPressedThisFrame;

        return keyboard || gamepad;
    }

    private bool WasCancelPressedThisFrame()
    {
        bool keyboard = Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame;

        bool gamepad = Gamepad.current != null &&
            Gamepad.current.buttonEast.wasPressedThisFrame;

        return keyboard || gamepad;
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        LeftClick?.Invoke();
    }
}
