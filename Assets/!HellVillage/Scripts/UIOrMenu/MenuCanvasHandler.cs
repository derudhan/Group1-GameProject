using UnityEngine;

namespace HellVillage.UIComponents {
    public class MenuCanvasHandler : MonoBehaviour {
        [SerializeField] private MainMenuHandler _mainMenu;

        private GameObject _currentMenu;

        private void Awake() {
            if (_mainMenu == null) _mainMenu = GetComponentInChildren<MainMenuHandler>(true);
        }

        private void Start() {
            InitMenu();
        }

        #region Private Methods

        private void InitMenu() {
            foreach (Transform child in transform) {
                if (child != _mainMenu.transform) {
                    child.gameObject.SetActive(false);
                }
            }
            ToggleMenu(_mainMenu.gameObject);
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

        public void ExitGame() {
            Debug.Log("Exiting game...");
            Application.Quit();
        }

        #endregion
    }
}
