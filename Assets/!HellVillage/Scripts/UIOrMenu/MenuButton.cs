using TMPro;
using UnityEngine;
using HellVillage.Utils;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.UIComponents {
    /// <summary>
    /// This class represents a button in the menu system.
    /// It allows for setting a custom label text and invokes an event when the button is clicked.
    /// The button text can be customized with padding spaces for better readability.
    /// </summary>
    public class MenuButton : MonoBehaviour {
        public UnityEvent OnButtonClicked = new UnityEvent();

        [Header("Settings")]
        [SerializeField] private string _buttonTextValue = "Button";
        [SerializeField] private int _paddingSpaces = 7;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Button _button;

        private void Awake() {
            if (_buttonText == null) _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            if (_button == null) _button = GetComponent<Button>();
            RefreshLabel();
        }

        private void OnEnable() {
            _button.onClick.AddListener(OnButtonClickedHandler);
        }
        private void OnDisable() {
            _button.onClick.RemoveListener(OnButtonClickedHandler);
        }

        #region Private Methods

        private void RefreshLabel() {
            _buttonText.text = GetButtonText();
        }

        private string GetButtonText() {
            string labelText = _buttonTextValue;
            return Helpers.SetTextWithPadding(labelText, _paddingSpaces);
        }

        #endregion

        #region Listener Methods

        private void OnButtonClickedHandler() {
            OnButtonClicked.Invoke();
        }

        #endregion

        #region Public Methods

        public void SetButtonText(string text) {
            if (string.IsNullOrEmpty(text)) return;

            _buttonTextValue = text;
            RefreshLabel();
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
            if (_buttonText == null) return;

            string newText = GetButtonText();

            if (_buttonText.text != newText) {
                if (!PrefabUtility.IsPartOfPrefabAsset(this)) {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }

                Undo.RecordObject(_buttonText, "Update Button Text");
                _buttonText.text = newText;
                EditorUtility.SetDirty(_buttonText);
            }
        }

        [ContextMenu("Find Text Component in Children")]
        private void FindTextComponent() {
            Undo.RecordObject(this, "Find Text Component");
            _buttonText = GetComponentInChildren<TextMeshProUGUI>();
            _button = GetComponent<Button>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}