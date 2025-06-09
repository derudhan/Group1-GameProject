using UnityEngine;

namespace HellVillage.Data {
    public class DataManager : MonoBehaviour {
        public static DataManager Instance { get; private set; }

        public AudioOptions AudioOptions { get; private set; }

        private void Awake() {
            if (Instance == null) Instance = this;
            AudioOptions = GetComponentInChildren<AudioOptions>();
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
}
