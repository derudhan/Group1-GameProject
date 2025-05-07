using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HellVillage {
    public class MenuManager : MonoBehaviour {
        [Header("Menus")]
        [SerializeField] GameObject _menuObj;
        [SerializeField] GameObject _controlsObj;
        [SerializeField] GameObject _audioObj;
        [SerializeField] GameObject _graphicsObj;

        bool _isOpen;

        private void Awake() {
            MainMenuOpen();
        }

        private void Update() {
            if (Keyboard.current.tabKey.wasPressedThisFrame) {
                ToggleMenu();
            }
        }

        private void ToggleMenu() {
            if (_isOpen) {
                MainMenuClose();
                _isOpen = false;
            } else {
                MainMenuOpen();
                _isOpen = true;
            }
        }

        #region Button Events
        public void MainMenuOpen() {
            _isOpen = true;
            _menuObj.SetActive(true);
            _controlsObj.SetActive(false);
            _audioObj.SetActive(false);
            _graphicsObj.SetActive(false);
        }

        public void MainMenuClose() {
            _isOpen = false;
            _menuObj.SetActive(false);
            _controlsObj.SetActive(false);
            _audioObj.SetActive(false);
            _graphicsObj.SetActive(false);
        }

        public void ControlsOpen() {
            _controlsObj.SetActive(true);
            _menuObj.SetActive(false);
            _audioObj.SetActive(false);
            _graphicsObj.SetActive(false);
        }

        public void AudioOpen() {
            _audioObj.SetActive(true);
            _controlsObj.SetActive(false);
            _menuObj.SetActive(false);
            _graphicsObj.SetActive(false);
        }

        public void GraphicsOpen() {
            _graphicsObj.SetActive(true);
            _audioObj.SetActive(false);
            _controlsObj.SetActive(false);
            _menuObj.SetActive(false);
        }
        #endregion
    }
}
