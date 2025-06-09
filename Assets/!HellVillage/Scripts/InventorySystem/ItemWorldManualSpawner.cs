using UnityEngine;

namespace HellVillage.InventorySystem {
    public class ItemWorldManualSpawner : MonoBehaviour {
        public Item item;

        private void Start() {
            ItemWorld.SpawnItemWorld(item, transform.position);
            Destroy(gameObject);
        }
    }
}
