using System.Collections;
using HellVillage.Scenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HellVillage.UIComponents {
    /// <summary>
    /// Script untuk inisialisasi tampilan splash screen di Unity.
    /// Semua settingan diusahakan memakai settingan yang ada di PlayerSettings.
    /// </summary>
    public class BootSplashHandler : MonoBehaviour {
        [Header("Settings")]
        [Tooltip("Delay sebelum splash screen menghilang dan masuk ke main menu.")]
        [SerializeField] private float delayBeforeMainMenu = 3f;

        [Header("References")]
        [SerializeField] private CanvasGroup bootSplashContainer;
        [SerializeField] private RawImage iconImage;

        private void Awake() {
            /// Kalau dikosongin akan mencari komponen yang ada di dalam children.
            /// Ini untuk menghindari error kalau ada yang lupa drag and drop di inspector.
            if (bootSplashContainer == null) bootSplashContainer = GetComponentInChildren<CanvasGroup>();
            if (iconImage == null) iconImage = bootSplashContainer.GetComponentInChildren<RawImage>();
        }

        private void Start() {
            iconImage.texture = PlayerSettings.virtualRealitySplashScreen;
            _ = StartCoroutine(_goToMainMenu(delayBeforeMainMenu));
        }

        #region Private Methods

        private IEnumerator _goToMainMenu(float delay) {
            float elapsedTime = 0f;

            while (elapsedTime < delay) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            SceneHandler.Instance.SwitchSceneWithFade((int)ScenesEnum.MainMenu, 1f);
            yield return null;
        }

        #endregion
    }
}
