using UnityEngine;
using Yarn.Unity;

namespace HellVillage.Input {
    public class InputManager : MonoBehaviour {
        private static InputSystem inputSystem;

        public static InputSystem.PlayerActions PlayerAction => inputSystem.Player;
        public static InputSystem.UIActions UIAction => inputSystem.UI;

        private void Awake() {
            inputSystem ??= new InputSystem();
            inputSystem.Enable();
        }

        [YarnCommand("UnFreezePlayer")]
        public static void EnablePlayerInput() => PlayerAction.Enable();
        [YarnCommand("FreezePlayer")]
        public static void DisablePlayerInput() => PlayerAction.Disable();

        public static void EnableAllInput() {
            PlayerAction.Enable();
            UIAction.Enable();
        }
        public static void DisableAllInput() {
            PlayerAction.Disable();
            UIAction.Disable();
        }
    }
}
