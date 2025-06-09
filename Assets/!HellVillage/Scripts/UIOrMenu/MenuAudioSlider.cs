using HellVillage.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.UIComponents {
    /// <summary>
    /// This class represents a slider in the menu system for adjusting audio volume.
    /// It allows the user to set volume levels for different audio types (e.g., music, sound effects).
    /// The slider can be adjusted in steps, and it updates the audio settings accordingly.
    /// </summary>
    public class MenuAudioSlider : MonoBehaviour {
        [Header("Settings")]
        [SerializeField, Range(0, 100)] private int _stepSize = 10;

        [Header("References")]
        [SerializeField] private AudioVolumeType _audioVolumeType;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _volumeLabel;
        [SerializeField] private Slider _slider;

        [SerializeField] private MenuButton _stepDecrementButton;
        [SerializeField] private MenuButton _decrementButton;
        [SerializeField] private MenuButton _incrementButton;
        [SerializeField] private MenuButton _stepIncrementButton;

        private float _currentValue;

        void Awake() {
            if (_audioVolumeType == AudioVolumeType.None) {
                Debug.LogError("AudioVolumeType is not set for MenuSlider on " + gameObject.name);
            }

            if (_slider == null) _slider = GetComponentInChildren<Slider>();
        }

        private void Start() {
            if (_slider != null) {
                _label.text = _audioVolumeType.ToString() + " Volume";
                _currentValue = DataManager.Instance.AudioOptions.GetVolume(_audioVolumeType);
                UpdateUI();
            } else {
                Debug.LogError("Slider component not found on " + gameObject.name);
            }
        }

        private void OnEnable() {
            ConnectListeners();
        }
        void OnDisable() {
            DisconnectListeners();
        }

        #region Private Methods

        private void UpdateValue(float value) {
            _currentValue = value;
            DataManager.Instance.AudioOptions.SetVolume(_audioVolumeType, value);
            UpdateUI();
        }

        private void UpdateUI() {
            _volumeLabel.text = (_currentValue * 100f).ToString("F0") + " %";
            _slider.value = _currentValue;
        }

        #endregion

        #region Listener Methods

        private void ConnectListeners() {
            DataManager.Instance.AudioOptions.OnResetVolumes += DataManager_OnResetVolumes;
            _slider.onValueChanged.AddListener(UpdateValue);

            _stepDecrementButton.OnButtonClicked.AddListener(() => UpdateValue(_currentValue - (_stepSize / 100f)));
            _decrementButton.OnButtonClicked.AddListener(() => UpdateValue(_currentValue - 0.01f));
            _incrementButton.OnButtonClicked.AddListener(() => UpdateValue(_currentValue + 0.01f));
            _stepIncrementButton.OnButtonClicked.AddListener(() => UpdateValue(_currentValue + (_stepSize / 100f)));
        }

        private void DisconnectListeners() {
            DataManager.Instance.AudioOptions.OnResetVolumes -= DataManager_OnResetVolumes;
            _slider.onValueChanged.RemoveListener(UpdateValue);

            _stepDecrementButton.OnButtonClicked.RemoveAllListeners();
            _decrementButton.OnButtonClicked.RemoveAllListeners();
            _incrementButton.OnButtonClicked.RemoveAllListeners();
            _stepIncrementButton.OnButtonClicked.RemoveAllListeners();
        }

        private void DataManager_OnResetVolumes() {
            float value = DataManager.Instance.AudioOptions.GetVolume(_audioVolumeType);
            UpdateValue(value);
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Get All References")]
        private void GetAllReferences() {
            Undo.RecordObject(this, "Get All References");
            _slider = GetComponentInChildren<Slider>();
            _label = GetComponentsInChildren<TextMeshProUGUI>()[0];
            _volumeLabel = GetComponentsInChildren<TextMeshProUGUI>()[1];

            _stepDecrementButton = GetComponentsInChildren<MenuButton>()[0];
            _decrementButton = GetComponentsInChildren<MenuButton>()[1];
            _incrementButton = GetComponentsInChildren<MenuButton>()[2];
            _stepIncrementButton = GetComponentsInChildren<MenuButton>()[3];
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
