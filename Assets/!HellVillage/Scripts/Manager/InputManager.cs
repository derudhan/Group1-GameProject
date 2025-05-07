using UnityEngine;
using UnityEngine.InputSystem;

namespace HellVillage
{
    public class InputManager : MonoBehaviour
    {
        public static PlayerInput PlayerInput;

        public static Vector2 Movement;
        public static bool RunIsHeld;

        private InputAction _moveAction;
        private InputAction _runAction;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();

            _moveAction = PlayerInput.actions["Move"];
            _runAction = PlayerInput.actions["Sprint"];
        }

        private void Update()
        {
            Movement = _moveAction.ReadValue<Vector2>();

            RunIsHeld = _runAction.IsPressed();
        }
    }
}
