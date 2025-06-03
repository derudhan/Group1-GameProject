using UnityEngine;

namespace HellVillage {
    public class ItemWorldManualSpawner : MonoBehaviour {
        public Item item;

        private void Start() {
            ItemWorld.SpawnItemWorld(item, transform.position);
            Destroy(gameObject);
        }
    }
}
