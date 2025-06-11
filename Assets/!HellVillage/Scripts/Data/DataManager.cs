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
        public PlayerData PlayerData;
    }

    public class DataManager : MonoBehaviour {
        public static DataManager Instance { get; private set; }

        public GameData gameData;

        private IDataService _dataService;
        public AudioOptions AudioOptions { get; private set; }


        private void Awake() {
            if (Instance == null) Instance = this;
            AudioOptions = GetComponentInChildren<AudioOptions>();
            _dataService = new FileDataService(new JsonSerializer());
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (scene.name == ScenesEnum.MainMenu.ToString()) {
                return;
            }

            Bind<PlayerControl, PlayerData>(gameData.PlayerData);
        }

        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null) {
                if (data == null) data = new TData { Id = entity.Id };
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
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

        public void SaveGame() => _dataService.Save(gameData, true);

        public void LoadGame(string gameName) {
            gameData = _dataService.Load(gameName);

            if (String.IsNullOrWhiteSpace(gameData.Name)) {
                gameData.Name = "Game";
            }
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
