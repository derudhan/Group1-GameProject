using UnityEngine;

namespace HellVillage.UIComponents {
    public class MenuCanvasHandler : MonoBehaviour {
        [SerializeField] private GameObject _firstMenu;

        private GameObject _currentMenu;

        private void Start() {
            InitMenu();
        }

        #region Private Methods

        private void InitMenu() {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
            }

            if (_firstMenu != null) ToggleMenu(_firstMenu.gameObject);
        }

        #endregion

        #region Public Methods

        public void ToggleMenu(GameObject menu) {
            menu.SetActive(true);
            if (_currentMenu != null && _currentMenu != menu) {
                _currentMenu.SetActive(false);
            }
            _currentMenu = menu;
        }

        public void HideAllMenu() {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
            }
            _currentMenu = null;
        }

        public void ExitGame() {
            Debug.Log("Exiting game...");
            Application.Quit();
        }

        #endregion
    }
}
