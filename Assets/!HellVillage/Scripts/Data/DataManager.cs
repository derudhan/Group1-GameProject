using System;
using System.Collections.Generic;
using System.Linq;
using HellVillage.Player2DRPG;
using HellVillage.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HellVillage.Data {
    [Serializable]
    public class GameData {
        public string Name;
        public ScenesEnum CurrentActiveScene;
        public PlayerData PlayerData;
    }

    public class DataManager : MonoBehaviour {
        public static DataManager Instance { get; private set; }

        [SerializeField] private bool _encryptSave = false;
        public GameData gameData;

        private IDataService _dataService;
        public AudioOptions AudioOptions { get; private set; }


        private void Awake() {
            if (Instance == null) Instance = this;
            AudioOptions = GetComponentInChildren<AudioOptions>();

            if (_encryptSave) {
                _dataService = new FileDataService(new JsonBase64Serializer());
            } else {
                _dataService = new FileDataService(new JsonSerializer());
            }
        }

        private void Start() => OnActiveSceneChanged(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());

        private void OnEnable() => SceneManager.activeSceneChanged += OnActiveSceneChanged;
        private void OnDisable() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        private void OnActiveSceneChanged(Scene current, Scene next) {
            // Debug.Log($"Active scene changed from {current.name} to {next.name}");
            if (next.buildIndex == (int)ScenesEnum._PersistentScene || next.buildIndex == (int)ScenesEnum.MainMenu) {
                return;
            }

            gameData.CurrentActiveScene = (ScenesEnum)next.buildIndex;
            LoadGameOrCreate(gameData.Name);
        }

        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null) {
                if (data == null) data = new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        private void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities) {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null) {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void SaveGame() => _dataService.Save(gameData);

        public void LoadGameOrCreate(string gameName) {
            if (!_dataService.SaveExists(gameName)) SaveGame();
            gameData = _dataService.Load(gameName);
            // Debug.Log($"Loading game data: {gameData == null}");

            if (String.IsNullOrWhiteSpace(gameData.Name)) gameData.Name = "Game";

            Bind<PlayerControl, PlayerData>(gameData.PlayerData);
        }

        public void SaveToPlayerPrefs(string key, float value) {
            PlayerPrefs.SetFloat(key, value);
        }

        public float TryLoadFromPlayerPrefs(string key, float defaultValue) {
            if (PlayerPrefs.HasKey(key)) {
                return PlayerPrefs.GetFloat(key);
            } else {
                SaveToPlayerPrefs(key, defaultValue);
                return defaultValue;
            }
        }
    }

    public interface ISaveable {
        string Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable {
        string Id { get; }
        void Bind(TData data);
    }
}
