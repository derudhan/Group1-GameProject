using UnityEngine;

namespace HellVillage.Init {
    public static class Initializer {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

        public static void Execute() {
            Debug.Log("Load Persistent Scene from the Initializer script");
            Object.Instantiate(Resources.Load<PersistentObject>("PersistentObject"));
        }
    }
}
