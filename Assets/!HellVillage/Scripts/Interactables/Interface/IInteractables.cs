using UnityEngine;

namespace HellVillage.InteractionSystem {
    public interface IInteractable {
        Collider2D Collider { get; set; }

        bool CanInteract { get; set; }

        void Interact();
    }
}