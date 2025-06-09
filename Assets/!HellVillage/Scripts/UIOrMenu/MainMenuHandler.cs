using HellVillage.Scenes;
using UnityEngine;

namespace HellVillage.UIComponents {
    public class MainMenuHandler : MonoBehaviour {
        [SerializeField] private SceneField _nextScene;

        private void Awake() {
            if (_nextScene == null) {
                Debug.LogError("_nextScene is not assigned in MainMenuHandler. Please assign a valid SceneField.");
            }
        }

        public void StartGame() {
            if (_nextScene != null) {
                SceneHandler.Instance.SwitchSceneWithFade(_nextScene, 1f);
            } else {
                Debug.LogError("Next scene is not assigned. Please assign a valid SceneField in MainMenuHandler.");
            }
        }
    }
}
