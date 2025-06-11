using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellVillage.Init {
    public class PersistentObject : MonoBehaviour {
        public static PersistentObject Instance { get; private set; }

        private const string PersistentSceneName = "_PersistentScene";

        private IEnumerator Start() {
            yield return SceneManager.LoadSceneAsync(PersistentSceneName, LoadSceneMode.Additive);
        }

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
