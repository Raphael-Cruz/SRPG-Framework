using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Anything that can hand the current party to the menu when it opens
    /// (your real party/save manager should implement this).
    /// </summary>
    public interface IPartyProvider
    {
        IEnumerable<PartyMemberRuntimeState> GetParty();
    }

    /// <summary>
    /// Opens/closes the Class Evolution screen from keyboard Esc or the
    /// gamepad Menu button (the "≡" / three-lines button on Xbox controllers,
    /// mapped by the Input System to Gamepad.startButton — this is usually
    /// what people mean by "three dots"; the actual Share/Capture button is
    /// OS-level and isn't exposed the same way, so it's not bound here).
    /// Pauses the game (Time.timeScale = 0) whenever the menu is open, and
    /// restores the previous time scale when it closes. Uses the new Input
    /// System, whose actions keep firing while Time.timeScale is 0, so Esc /
    /// Menu still closes the paused menu.
    /// </summary>
    public class ClassEvolutionMenuInput : MonoBehaviour
    {
        [Header("Menu")]
        [Tooltip("Root GameObject of the Class Evolution screen (tabs + tree + footer). Shown/hidden by this controller.")]
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private ClassEvolutionScreen menuScreen;

        [Header("Party Source")]
        [Tooltip("Drag in your party/save manager here — it must implement IPartyProvider.")]
        [SerializeField] private MonoBehaviour partyProviderBehaviour;

        public bool IsOpen { get; private set; }
        public event Action OnMenuOpened;
        public event Action OnMenuClosed;

        private IPartyProvider _partyProvider;
        private InputAction _toggleMenuAction;
        private float _timeScaleBeforePause = 1f;

        private void Awake()
        {
            _partyProvider = partyProviderBehaviour as IPartyProvider;

            _toggleMenuAction = new InputAction(name: "ToggleClassMenu", type: InputActionType.Button);
            _toggleMenuAction.AddBinding("<Keyboard>/escape");
            _toggleMenuAction.AddBinding("<Gamepad>/start"); // Xbox Menu (≡) button
            _toggleMenuAction.performed += _ => ToggleMenu();

            if (menuRoot != null) menuRoot.SetActive(false);
        }

        private void OnEnable() => _toggleMenuAction.Enable();
        private void OnDisable() => _toggleMenuAction.Disable();
        private void OnDestroy() => _toggleMenuAction.Dispose();

        public void ToggleMenu()
        {
            if (IsOpen) CloseMenu();
            else OpenMenu();
        }

        public void OpenMenu()
        {
            if (IsOpen) return;
            IsOpen = true;

            if (menuRoot != null) menuRoot.SetActive(true);
            if (_partyProvider != null) menuScreen.Open(_partyProvider.GetParty());

            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;

            OnMenuOpened?.Invoke();
        }

        public void CloseMenu()
        {
            if (!IsOpen) return;
            IsOpen = false;

            if (menuRoot != null) menuRoot.SetActive(false);
            Time.timeScale = _timeScaleBeforePause > 0f ? _timeScaleBeforePause : 1f;

            OnMenuClosed?.Invoke();
        }

        /* ---------------------------------------------------------------
         * Using the legacy Input Manager instead of the new Input System?
         * Delete this component's Awake/OnEnable/OnDisable/OnDestroy above
         * and replace with:
         *
         *   private void Update()
         *   {
         *       if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
         *           ToggleMenu();
         *   }
         *
         * (JoystickButton7 is the Xbox controller's Menu/Start button under
         * Unity's default joystick mapping — verify against your Input
         * Manager axes/buttons setup.)
         * ------------------------------------------------------------- */
    }
}
