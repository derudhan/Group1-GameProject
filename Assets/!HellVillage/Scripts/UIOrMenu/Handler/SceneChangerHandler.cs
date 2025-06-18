using HellVillage.Scenes;
using UnityEngine;

namespace HellVillage.UIComponents {
    public class SceneChangerHandler : MonoBehaviour {
        [SerializeField] private ScenesEnum _refScene;

        public void ChangeScene() {
            SceneHandler.Instance.SwitchSceneWithFade((int)_refScene, 1f);
        }
    }
}
