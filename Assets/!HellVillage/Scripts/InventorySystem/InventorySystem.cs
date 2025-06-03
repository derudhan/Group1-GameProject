using UnityEngine;

namespace HellVillage {
    public class InventorySystem : MonoBehaviour {
        [SerializeField] private UIInventory uIInventory;
        public Inventory inventory { get; private set; }

        private void Awake() {
            inventory = new Inventory();

            if (uIInventory != null) {
                uIInventory.SetInventory(inventory);
            } else {
                Debug.LogWarning("UIInventory is not assigned in the InventorySystem.");
            }

            inventory.AddItem(new Item { itemType = Item.ItemType.Item1, amount = 1 });
            inventory.AddItem(new Item { itemType = Item.ItemType.Item2, amount = 1 });
        }
    }
}
