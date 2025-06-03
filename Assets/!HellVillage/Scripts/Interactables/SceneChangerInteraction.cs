using UnityEngine;

namespace HellVillage {
    public class SceneChangerInteraction : TriggerInteractionBase {
        public enum AreaToSpawnAt {
            None,
            Area1,
            Area2,
            Area3,
            Area4,
        }

        [Header("Spawn TO")]
        [SerializeField] private AreaToSpawnAt areaToSpawnAt = AreaToSpawnAt.None;
        [SerializeField] private SceneField _sceneToLoad;

        [Space(10f)]
        [Header("This Area is")]
        public AreaToSpawnAt CurrentAreaPosition = AreaToSpawnAt.None;

        public override void Interact() {
            SceneSwapManager.Instance.SwapSceneFromAreaUse(_sceneToLoad, areaToSpawnAt);
        }
    }
}
