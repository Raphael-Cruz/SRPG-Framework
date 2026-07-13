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

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        LeftClick?.Invoke();
    }
}