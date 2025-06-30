using HellVillage.InventorySystem;
using UnityEngine;

namespace HellVillage.InteractionSystem {
    public class PickupInteraction : TriggerInteractionBase {
        private InventoryManage inventorySystem;

        private void Awake() {
            inventorySystem = FindFirstObjectByType<InventoryManage>();
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
