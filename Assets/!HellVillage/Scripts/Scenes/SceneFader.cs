using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HellVillage {
    public enum FadeType {
        BlackFade,
    }

    /// <summary>
    /// Mengelola efek transisi antar scene dengan menggunakan UI Image.
    /// Memungkinkan fade in dan fade out dengan durasi yang ditentukan.
    /// </summary>
    public class SceneFader : MonoBehaviour {
        private int _fadeAmount = Shader.PropertyToID("_FadeAmount");

        private int _useBlackFade = Shader.PropertyToID("_UseBlackFade");

        private int? _lastEffect;

        private Image _image;
        private Material _material;

        private void Awake() {
            _image = GetComponent<Image>();
            _material = GetInstancedMaterialFromImage(_image);
            _image.material = _material;
            _lastEffect = _useBlackFade;
        }

        // ===============================================================================================================================

        #region Private Methods

        private IEnumerator FadeCoroutine(float startValue, float targetValue, float duration) {
            float elapsedTime = 0;
            float elapsedPercentage = 0;

            while (elapsedPercentage < 1) {
                elapsedPercentage = elapsedTime / duration;

                float lerpedValue = Mathf.Lerp(startValue, targetValue, elapsedPercentage);
                _material.SetFloat(_fadeAmount, lerpedValue);

                yield return null;
                elapsedTime += Time.deltaTime;
            }
        }

        private void ApplyFadeEffect(int fadeTypeInInteger) {
            _material.SetFloat(fadeTypeInInteger, 1f);
            _lastEffect = fadeTypeInInteger;
        }

        private void SwitchFadeType(FadeType fadeType) {
            if (_lastEffect.HasValue) _material.SetFloat(_lastEffect.Value, 0f);

            switch (fadeType) {
                case FadeType.BlackFade:
                    ApplyFadeEffect(_useBlackFade);
                    break;
            }
        }

        private Material GetInstancedMaterialFromImage(Image image) {
            if (image == null) {
                Debug.LogError("Image component is null. Cannot get instanced material.");
                return null;
            }

            Material mat = image.material;
            return new Material(mat);
        }

        #endregion

        // ===============================================================================================================================

        #region Public Methods

        /// <summary>
        /// Memulai fade in dari scene.
        /// Setelah selesai, objek ini akan dinonaktifkan.
        /// Gunakan metode ini untuk memulai transisi masuk ke scene baru.
        /// </summary>
        public IEnumerator FadeInCoroutine(float duration, FadeType fadeType = FadeType.BlackFade) {
            SwitchFadeType(fadeType);
            float startValue = 1f;
            float targetValue = 0f;

            yield return FadeCoroutine(startValue, targetValue, duration);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Memulai fade out dari scene.
        /// Setelah selesai, objek ini akan tetap aktif untuk transisi keluar.
        /// Gunakan metode ini untuk memulai transisi keluar dari scene saat ini.
        /// </summary>
        public IEnumerator FadeOutCoroutine(float duration, FadeType fadeType = FadeType.BlackFade) {
            SwitchFadeType(fadeType);
            float startValue = 0f;
            float targetValue = 1f;

            gameObject.SetActive(true);
            yield return FadeCoroutine(startValue, targetValue, duration);
        }

        #endregion
    }
}
