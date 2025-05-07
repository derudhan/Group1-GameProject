using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HellVillage
{
    public class DebugUI : MonoBehaviour
    {
        private UIDocument _uiDocs;
        private Label _titleLabel;

        private void Start()
        {
            _uiDocs = GetComponent<UIDocument>();

            VisualElement root = _uiDocs.rootVisualElement;
            _titleLabel = root.Q<Label>("TitleGame");
            _titleLabel.text = $"{PlayerSettings.productName} - {PlayerSettings.bundleVersion}";
        }
    }
}
