using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;

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
        [SerializeField] private int h1FontSize = 72;
        [SerializeField] private int h2FontSize = 64;
        [SerializeField] private int h3FontSize = 48;
        [SerializeField] private int h4FontSize = 32;

        [Header("References")]
        [SerializeField] private TextAsset _creditsTextAsset;
        [SerializeField] private TextMeshProUGUI _creditsText;

        private void Awake() {
            if (_creditsTextAsset == null) _creditsTextAsset = Resources.Load<TextAsset>("CREDITS");
            if (_creditsText == null) _creditsText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start() {
            UpdateTextFromFile();
        }

        #region Private Methods

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
            int[] headingFontSizes = new int[] { h1FontSize, h2FontSize, h3FontSize, h4FontSize };

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
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
