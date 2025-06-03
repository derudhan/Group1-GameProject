using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HellVillage {
    public class MenuManager : MonoBehaviour {
        public UnityEvent OnMenuOpen;
        public UnityEvent OnMenuClose;

        [SerializeField] bool InMainMenu = true;

        [Header("Menus")]
        [SerializeField] GameObject _menuObj;
        [SerializeField] GameObject _controlsObj;
        [SerializeField] GameObject _audioObj;
        [SerializeField] GameObject _graphicsObj;

        bool _isOpen;

        private void Awake() {
            if (InMainMenu) {
                MenuOpen();
            } else {
                MenuClose();
            }
        }

        private void Update() {
            if (Keyboard.current.tabKey.wasPressedThisFrame && !InMainMenu) {
                ToggleMenu();
            }
        }

        private void ToggleMenu() {
            if (_isOpen) {
                MenuClose();
                _isOpen = false;
            } else {
                MenuOpen();
                _isOpen = true;
            }
        }

        #region Button Events
        public void MenuOpen() {
            _isOpen = true;
            _menuObj.SetActive(true);
            _controlsObj.SetActive(false);
            _audioObj.SetActive(false);
            _graphicsObj.SetActive(false);
            OnMenuOpen?.Invoke();
        }

        public void MenuClose() {
            _isOpen = false;
            _menuObj.SetActive(false);
            _controlsObj.SetActive(false);
            _audioObj.SetActive(false);
            _graphicsObj.SetActive(false);
            OnMenuClose?.Invoke();
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

        public void QuitGame() {
            Application.Quit();
            Debug.Log("Quit Game");
        }
        #endregion
    }
}
