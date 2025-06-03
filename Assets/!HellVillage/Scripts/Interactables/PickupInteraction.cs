using UnityEngine;

namespace HellVillage {
    public class PickupInteraction : TriggerInteractionBase {
        private InventorySystem inventorySystem;

        private void Awake() {
            inventorySystem = FindFirstObjectByType<InventorySystem>();
            if (inventorySystem == null) {
                Debug.LogError("InventorySystem not found in the scene.");
            }
        }

        public override void Interact() {
            ItemWorld itemWorld = GetComponent<ItemWorld>();
            if (itemWorld != null && inventorySystem != null) {
                inventorySystem.inventory.AddItem(itemWorld.GetItem());
                itemWorld.DestroySelf();
            }
        }
    }
}
