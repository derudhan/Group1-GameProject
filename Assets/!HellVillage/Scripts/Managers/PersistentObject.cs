using UnityEngine;

namespace HellVillage.Init {
    public class PersistentObject : MonoBehaviour {
        public static PersistentObject Instance { get; private set; }

        private void Awake() {
            if (Instance != null) {
                Debug.Log("Duplikasi ditemukan! OTW menghancurkan " + gameObject.name);
                Destroy(gameObject);
            } else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
