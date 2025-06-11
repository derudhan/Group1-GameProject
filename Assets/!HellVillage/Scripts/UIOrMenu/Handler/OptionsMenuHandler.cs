using HellVillage.Data;
using UnityEngine;

namespace HellVillage.UIComponents {
    public class OptionsMenuHandler : MonoBehaviour {
        [SerializeField] private MenuButton _resetButton;

        private void Awake() {
            if (_resetButton == null) Debug.LogError($"Referensi Reset Button di {gameObject.name} belum ditambahkan!");
        }

        private void OnEnable() {
            _resetButton.OnButtonClicked.AddListener(ResetButton_OnButtonClicked);
        }
        private void OnDisable() {
            _resetButton.OnButtonClicked.AddListener(ResetButton_OnButtonClicked);
        }

        #region Private Methods

        private void ResetButton_OnButtonClicked() {
            DataManager.Instance.AudioOptions.ResetAllVolumes();
        }

        #endregion
    }
}
