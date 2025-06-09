using UnityEngine;

namespace HellVillage.Interface {
    public interface IInteractable {
        Collider2D Collider { get; set; }

        bool CanInteract { get; set; }

        void Interact();
    }
}