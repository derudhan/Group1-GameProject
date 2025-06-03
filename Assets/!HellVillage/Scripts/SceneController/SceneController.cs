using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellVillage {
    public class SceneController : MonoBehaviour {
        [SerializeField]
        private float _sceneFadeDuration;

        private SceneFade _sceneFade;

        private void Awake() {
            _sceneFade = GetComponentInChildren<SceneFade>();
        }

        private IEnumerator Start() {
            yield return _sceneFade.FadeInCoroutine(_sceneFadeDuration);
        }

        public void LoadScene(string nextScene) {
            StartCoroutine(LoadSceneCoroutine(nextScene));
        }

        private IEnumerator LoadSceneCoroutine(string nextScene) {
            yield return _sceneFade.FadeOutCoroutine(_sceneFadeDuration);
            yield return SceneManager.LoadSceneAsync(nextScene);
        }
    }
}
