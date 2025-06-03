using UnityEngine;

namespace HellVillage {
    [RequireComponent(typeof(Collider2D))]
    public class TriggerInteractionBase : MonoBehaviour, IInteractable {
        public Collider2D Collider { get; set; }
        public bool CanInteract { get; set; }

        private void Awake() {
            Collider = GetComponent<Collider2D>();
            Collider.isTrigger = true;
            CanInteract = false;
        }

        private void Update() {
            if (CanInteract) {
                if (InputManager.InteractWasPressed) {
                    Interact();
                }
            }
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

        public virtual void Interact() { }
    }
}