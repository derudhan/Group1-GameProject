using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HellVillage {
    /// <summary>
    /// Mengelola efek transisi antar scene dengan menggunakan UI Image.
    /// Memungkinkan fade in dan fade out dengan durasi yang ditentukan.
    /// </summary>
    public class SceneFader : MonoBehaviour {
        private Image _sceneFadeImage;

        private void Awake() {
            _sceneFadeImage = GetComponent<Image>();
        }

        // ===============================================================================================================================

        #region Private Methods

        private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration) {
            float elapsedTime = 0;
            float elapsedPercentage = 0;

            while (elapsedPercentage < 1) {
                elapsedPercentage = elapsedTime / duration;
                _sceneFadeImage.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

                yield return null;
                elapsedTime += Time.deltaTime;
            }
        }

        #endregion

        // ===============================================================================================================================

        #region Public Methods

        /// <summary>
        /// Memulai fade in dari scene.
        /// Setelah selesai, objek ini akan dinonaktifkan.
        /// Gunakan metode ini untuk memulai transisi masuk ke scene baru.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IEnumerator FadeInCoroutine(float duration) {
            Color startColor = new Color(_sceneFadeImage.color.r, _sceneFadeImage.color.g, _sceneFadeImage.color.b, 1);
            Color targetColor = new Color(_sceneFadeImage.color.r, _sceneFadeImage.color.g, _sceneFadeImage.color.b, 0);

            yield return FadeCoroutine(startColor, targetColor, duration);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Memulai fade out dari scene.
        /// Setelah selesai, objek ini akan tetap aktif untuk transisi keluar.
        /// Gunakan metode ini untuk memulai transisi keluar dari scene saat ini.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IEnumerator FadeOutCoroutine(float duration) {
            Color startColor = new Color(_sceneFadeImage.color.r, _sceneFadeImage.color.g, _sceneFadeImage.color.b, 0);
            Color targetColor = new Color(_sceneFadeImage.color.r, _sceneFadeImage.color.g, _sceneFadeImage.color.b, 1);

            gameObject.SetActive(true);
            yield return FadeCoroutine(startColor, targetColor, duration);
        }

        #endregion
    }
}
