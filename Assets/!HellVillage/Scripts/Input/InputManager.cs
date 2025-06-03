using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace HellVillage {
    public class InputManager : MonoBehaviour {
        public static PlayerInput PlayerInput;

        public static Vector2 Movement;
        public static bool RunIsHeld;
        public static bool InteractWasPressed;

        public static bool InputDisabled = false;

        private InputAction _moveAction;
        private InputAction _runAction;
        private InputAction _interactAction;

        private void Awake() {
            PlayerInput = GetComponent<PlayerInput>();

            _moveAction = PlayerInput.actions["Move"];
            _runAction = PlayerInput.actions["Sprint"];
            _interactAction = PlayerInput.actions["Interact"];
        }

        private void Update() {
            if (InputDisabled) return;

            Movement = _moveAction.ReadValue<Vector2>();

            RunIsHeld = _runAction.IsPressed();

            InteractWasPressed = _interactAction.WasPressedThisFrame();
        }

        [YarnCommand("FreezePlayer")]
        public static void EnableInput() {
            InputDisabled = false;
        }
        [YarnCommand("UnFreezePlayer")]
        public static void DisableInput() {
            InputDisabled = true;
        }
    }
}
