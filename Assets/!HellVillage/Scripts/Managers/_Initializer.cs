using UnityEngine;

namespace HellVillage.Init {
    public static class Initializer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        public static void Execute() {
            Debug.Log("Loaded by the Persist Objects from the Initializer script");
            Object.Instantiate(Resources.Load("PersistentObject"));
        }
    }
}
