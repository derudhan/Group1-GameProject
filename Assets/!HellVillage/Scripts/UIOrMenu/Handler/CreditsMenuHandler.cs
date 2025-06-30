using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using HellVillage.Input;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.UIComponents {
    /// <summary>
    /// This class handles the display of credits in the game.
    /// It reads the credits from a text asset, formats it, and displays it in a TextMeshProUGUI component.
    /// The credits can include titles, comments, and URLs, which are formatted accordingly.
    /// </summary>
    public class CreditsMenuHandler : MonoBehaviour, IPointerClickHandler {
        [Header("Text Settings - Formatting")]
        [SerializeField] private bool trimFirstLine = false;
        [SerializeField] private int _h1FontSize = 72;
        [SerializeField] private int _h2FontSize = 64;
        [SerializeField] private int _h3FontSize = 48;
        [SerializeField] private int _h4FontSize = 32;

        [Header("Scroll Settings")]
        [SerializeField] private bool _autoScrollEnabled = true;
        [SerializeField, Range(0.0001f, 1f)] private float _scrollSpeed = 0.01f;
        [SerializeField, Range(1f, 100f)] private float _boostSpeedFactor = 20f;
        private float _currentScrollPosition = 1f;

        [Header("References")]
        [SerializeField] private TextAsset _creditsTextAsset;
        [SerializeField] private TextMeshProUGUI _creditsText;
        [SerializeField] private Scrollbar _scrollbar;

        private void Awake() {
            if (_creditsTextAsset == null) _creditsTextAsset = Resources.Load<TextAsset>("CREDITS");
            if (_creditsText == null) _creditsText = GetComponentInChildren<TextMeshProUGUI>();
            if (_scrollbar == null) _scrollbar = GetComponentInChildren<Scrollbar>();
        }

        private void Start() {
            UpdateTextFromFile();
        }

        private void Update() {
            float input_axis = -InputManager.UIAction.Navigate.ReadValue<Vector2>().y;
            if (input_axis != 0f) {
                ScrollContainer(_boostSpeedFactor * _scrollSpeed * input_axis * Time.deltaTime);
            } else {
                ScrollContainer(_scrollSpeed * Time.deltaTime);
            }
        }

        #region Private Methods

        private void ScrollContainer(float amount) {
            if (!gameObject.activeInHierarchy || !_autoScrollEnabled || _scrollbar.value < 0f) return;

            _currentScrollPosition = Mathf.Clamp01(_currentScrollPosition - amount);
            _scrollbar.value = _currentScrollPosition;
        }

        private void UpdateTextFromFile() {
            string text = _creditsTextAsset != null ? _creditsTextAsset.text : "Credits not found.";
            if (trimFirstLine) {
                int endOfFirstLine = text.IndexOf('\n');
                text = text[(endOfFirstLine + 1)..]; // Remove the first line
            }

            text = text.Replace("\r\n", "\n");
            text = text.Replace("\n\r", "\n");
            text = text.Replace("\\" + "\n", "\n");
            text = RegexReplaceComments(text);
            text = RegexReplaceUrls(text);
            text = RegexReplaceTitles(text);

            _creditsText.text = text;
        }

        private string RegexReplaceComments(string credits) {
            string pattern = @"^\[.*?\]:.*(?:\r?\n)?";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            string result = regex.Replace(credits, string.Empty);
            return result;
        }

        private string RegexReplaceUrls(string credits) {
            string pattern = @"\[(.*?)\]\((.*?)\)";
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            string result = regex.Replace(credits, "<u><link=$2>$1</link></u>");
            return result;
        }

        private string RegexReplaceTitles(string credits) {
            int iteration = 0;
            int[] headingFontSizes = new int[] { _h1FontSize, _h2FontSize, _h3FontSize, _h4FontSize };

            foreach (int fontSize in headingFontSizes) {
                iteration++;
                string pattern = $@"([^#]|^)#{{{iteration}}}\s([^\n]*)";
                Regex regex = new Regex(pattern, RegexOptions.Multiline);
                string result = regex.Replace(credits, $"$1<size={fontSize}>$2</size>");
                credits = result;
            }
            return credits;
        }

        #endregion

        #region Interface Implementations

        public void OnPointerClick(PointerEventData eventData) {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_creditsText, eventData.position, eventData.pressEventCamera);

            if (linkIndex != -1) {
                TMP_LinkInfo linkInfo = _creditsText.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();

                Debug.Log($"Link clicked: {linkId}");
                Application.OpenURL(linkId);
            }
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Get All References")]
        private void GetAllReferences() {
            Undo.RecordObject(this, "Get All References");
            _creditsTextAsset = Resources.Load<TextAsset>("CREDITS");
            _creditsText = GetComponentInChildren<TextMeshProUGUI>();
            _scrollbar = GetComponentInChildren<Scrollbar>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
