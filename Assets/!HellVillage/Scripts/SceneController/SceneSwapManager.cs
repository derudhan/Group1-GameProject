using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellVillage {
    public class SceneSwapManager : MonoBehaviour {
        public static SceneSwapManager Instance { get; private set; }

        private static bool _sceneLoadedFromAreaUse;

        private GameObject _player;
        private Collider2D _playerCollider => _player.GetComponent<Collider2D>();
        private Collider2D _areaCollider;
        private Vector2 _playerSpawnPosition;

        private SceneController _sceneController;
        private SceneChangerInteraction.AreaToSpawnAt _areaToSpawnAt;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }

            _sceneController = GetComponentInChildren<SceneController>();
            _player = GameObject.FindGameObjectWithTag("Player");

        }

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void SwapSceneFromAreaUse(SceneField nextScene, SceneChangerInteraction.AreaToSpawnAt areaToSpawnAt) {
            _sceneLoadedFromAreaUse = true;
            _areaToSpawnAt = areaToSpawnAt;
            _sceneController.LoadScene(nextScene);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (_sceneLoadedFromAreaUse) {
                FindAreaToSpawnAt(_areaToSpawnAt);
                _player.transform.position = _playerSpawnPosition;
                _sceneLoadedFromAreaUse = false;
            }
        }

        private void FindAreaToSpawnAt(SceneChangerInteraction.AreaToSpawnAt areaToSpawnAt) {
            SceneChangerInteraction[] areas = FindObjectsByType<SceneChangerInteraction>(FindObjectsSortMode.None);

            for (int i = 0; i < areas.Length; i++) {
                if (areas[i].CurrentAreaPosition == areaToSpawnAt) {
                    _areaCollider = areas[i].GetComponent<Collider2D>();
                    CalculateSpawnPosition();
                    return;
                }
            }
        }

        private void CalculateSpawnPosition() {
            float colliderHeight = _playerCollider.bounds.extents.y;
            _playerSpawnPosition = _areaCollider.transform.position - new Vector3(0f, colliderHeight, 0f);
        }
    }
}
