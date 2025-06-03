using UnityEngine;

namespace HellVillage {
    public interface IInteractable {
        Collider2D Collider { get; set; }

        bool CanInteract { get; set; }

        void Interact();
    }
}