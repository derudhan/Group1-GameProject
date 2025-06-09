using TMPro;
using UnityEngine;
using HellVillage.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.UIComponents {
    /// <summary>
    /// This class represents a toggle button in the menu system for audio settings.
    /// It allows the user to toggle audio mute on or off.
    /// The button updates its label based on the current audio state.
    /// </summary>
    public class MenuAudioToggle : MonoBehaviour {
        [SerializeField] private string _toggleLabel = "Audio Mute";

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private MenuButton _toggleButton;

        private bool currentState = false;

        private void Awake() {
            if (_toggleButton == null) _toggleButton = GetComponentInChildren<MenuButton>();
            if (_label == null) _label = GetComponentInChildren<TextMeshProUGUI>();

            _label.text = _toggleLabel;
            currentState = DataManager.Instance.AudioOptions.IsMuted();
            UpdateUI();
        }

        private void OnEnable() {
            _toggleButton.OnButtonClicked.AddListener(ToggleButton_OnButtonClicked);
        }
        private void OnDisable() {
            _toggleButton.OnButtonClicked.RemoveListener(ToggleButton_OnButtonClicked);
        }

        #region Private Methods

        private void UpdateUI() {
            _toggleButton.SetButtonText(currentState ? "On" : "Off");
        }

        #endregion

        #region Listener Methods

        private void ToggleButton_OnButtonClicked() {
            currentState = !currentState;
            DataManager.Instance.AudioOptions.SetAudioMixerMute(currentState);
            UpdateUI();
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
            if (_label == null) return;

            if (_label.text != _toggleLabel) {
                if (!PrefabUtility.IsPartOfPrefabAsset(this)) {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }

                Undo.RecordObject(_label, "Update Toggle Label");
                _label.text = _toggleLabel;
                EditorUtility.SetDirty(_label);
            }
        }

        [ContextMenu("Get All References")]
        private void GetAllReferences() {
            Undo.RecordObject(this, "Get All References");
            _label = GetComponentInChildren<TextMeshProUGUI>();
            _toggleButton = GetComponentInChildren<MenuButton>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
