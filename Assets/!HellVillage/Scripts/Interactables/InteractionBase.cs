using HellVillage.Input;
using HellVillage.Interface;
using UnityEngine;

namespace HellVillage.InteractionSystem {
    [RequireComponent(typeof(Collider2D))]
    public class TriggerInteractionBase : MonoBehaviour, IInteractable {
        public Collider2D Collider { get; set; }
        public bool CanInteract { get; set; }

        private void Awake() {
            Collider = GetComponent<Collider2D>();
            Collider.isTrigger = true;
            CanInteract = false;
        }

        private void OnEnable() {
            InputManager.PlayerAction.Interact.performed += Interact;
        }
        private void OnDisable() {
            InputManager.PlayerAction.Interact.performed -= Interact;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Player")) {
                CanInteract = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Player")) {
                CanInteract = false;
            }
        }

        #region Listener Methods

        private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext context) {
            if (CanInteract) {
                Interact();
            }
        }

        #endregion

        public virtual void Interact() { }
    }
}