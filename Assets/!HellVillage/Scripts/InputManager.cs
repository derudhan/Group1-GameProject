using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace HellVillage.Input {
    public class InputManager : MonoBehaviour {
        private static InputSystem inputSystem;
        private static EventSystem eventSystem;

        public static InputSystem.PlayerActions PlayerAction => inputSystem.Player;
        public static InputSystem.UIActions UIAction => inputSystem.UI;

        public static Vector2 Move;
        public static bool RunIsHeld;

        private void Awake() {
            inputSystem ??= new InputSystem();
            eventSystem = eventSystem != null ? eventSystem : GetComponentInChildren<EventSystem>();
            inputSystem.Enable();
        }

        private void OnEnable() {
            PlayerAction.Move.performed += OnMoveAction;
            PlayerAction.Move.canceled += OnMoveAction;

            PlayerAction.Sprint.performed += OnSprintAction;
            PlayerAction.Sprint.canceled += OnSprintAction;
        }
        private void OnDisable() {
            PlayerAction.Move.performed -= OnMoveAction;
            PlayerAction.Move.canceled -= OnMoveAction;

            PlayerAction.Sprint.performed -= OnSprintAction;
            PlayerAction.Sprint.canceled -= OnSprintAction;
        }

        private void OnMoveAction(InputAction.CallbackContext context) {
            if (context.performed) {
                Move = context.ReadValue<Vector2>();
            } else if (context.canceled) {
                Move = Vector2.zero;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext context) {
            if (context.performed) {
                RunIsHeld = true;
            } else if (context.canceled) {
                RunIsHeld = false;
            }
        }

        [YarnCommand("UnFreezePlayer")]
        public static void EnablePlayerInput() => PlayerAction.Enable();
        [YarnCommand("FreezePlayer")]
        public static void DisablePlayerInput() => PlayerAction.Disable();

        public static void EnableAllInput() {
            PlayerAction.Enable();
            UIAction.Enable();
            eventSystem.sendNavigationEvents = true;
        }
        public static void DisableAllInput() {
            PlayerAction.Disable();
            UIAction.Disable();
            eventSystem.sendNavigationEvents = false;
        }
    }
}
