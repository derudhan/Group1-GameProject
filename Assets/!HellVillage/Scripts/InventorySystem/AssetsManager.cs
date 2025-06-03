using UnityEngine;

namespace HellVillage {
    public class AssetsManager : MonoBehaviour {
        public static AssetsManager Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }

        public ItemWorld itemWorldPrefab;

        public Sprite item1;
        public Sprite item2;
    }
}
