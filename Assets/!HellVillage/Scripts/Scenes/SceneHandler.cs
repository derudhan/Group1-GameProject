using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.Scenes {
    public class SceneHandler : MonoBehaviour {
        public static SceneHandler Instance { get; private set; }

        [Header("References"),
        Tooltip("Referensi ke loading screen yang akan ditampilkan saat scene sedang dimuat. Jika tidak diisi, maka akan menggunakan komponen pertama yang ditemukan di dalam children dari GameObject ini.")]
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private SceneFader _sceneFade;

        [Header("FadeIn Settings")]
        [SerializeField] private bool _fadeInOnStart = true;
        [HideInInspector, SerializeField] private float _sceneFadeDuration = 1f;

        private Queue<SceneOperation> _sceneQueue = new Queue<SceneOperation>();
        private Coroutine _fadingCoroutine;
        private bool _isOperating;

        // =========================================================================================================

        #region BuiltIn Functions

        private void Awake() {
            if (Instance == null) Instance = this;

            if (_sceneFade == null) _sceneFade = GetComponentInChildren<SceneFader>();
            if (_loadingScreen != null) _loadingScreen.SetActive(false);
        }

        private void Start() {
            if (_fadeInOnStart) {
                if (_fadingCoroutine != null) {
                    StopCoroutine(_fadingCoroutine);
                }
                _fadingCoroutine = StartCoroutine(StartFadeIn(_sceneFadeDuration));
            }
            ;
        }

        #endregion

        // =========================================================================================================

        #region Private Methods

        private IEnumerator StartFadeIn(float _fadeDuration) {
            if (_fadingCoroutine != null) {
                StopCoroutine(_fadingCoroutine);
            }
            yield return _sceneFade.FadeInCoroutine(_fadeDuration);
        }

        private IEnumerator StartFadeOut(float _fadeDuration) {
            if (_fadingCoroutine != null) {
                StopCoroutine(_fadingCoroutine);
            }
            yield return _sceneFade.FadeOutCoroutine(_fadeDuration);
        }

        /// <summary>
        /// Metode ini akan menjalankan operasi scene yang ada di dalam antrian.
        /// Ini akan memuat atau meng-unload scene sesuai dengan operasi yang ada di dalam antrian.
        /// Operasi ini akan terus berjalan hingga tidak ada lagi operasi yang tersisa dalam antrian.
        /// Jika ada operasi scene yang sedang berjalan, maka metode ini akan menunggu hingga operasi tersebut selesai sebelum melanjutkan ke operasi berikutnya.
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunSceneOperationsCoroutine() {
            // Mengecek apakah ada operasi scene yang sedang berjalan
            // Mengecek juga apakah ada setidaknya satu operasi yang sedang dalam antrian
            if (_isOperating) yield break;

            // Menandai bahwa operasi scene sedang berjalan lalu mengaktifkan loading screen atau Fading
            _isOperating = true;
            if (_loadingScreen != null) _loadingScreen.SetActive(true);

            // Debug.Log("Starting scene operations with total operations: " + _sceneQueue.Count);
            // Debug.Log("List of operations in queue: " + string.Join(", ", _sceneQueue));
            while (_sceneQueue.Count > 0) {
                SceneOperation operationData = _sceneQueue.Dequeue();
                AsyncOperation loadingOperation;
                // Debug.Log($"Operation Type: {operationData.UseSceneField}, Loading: {operationData.IsLoadingOperation}, Scene Index: {operationData.SceneIndex}, Scene Field: {operationData.SceneField?.SceneName}");

                // Membuat kemudian melaksanakan operasi scene berdasarkan apakah itu operasi loading atau unloading
                if (operationData.UseSceneField) {
                    // Jika menggunakan SceneField, maka kita akan menggunakan SceneField untuk memuat atau meng-unload scene
                    {
                        loadingOperation = operationData.IsLoadingOperation
                            ? SceneManager.LoadSceneAsync(operationData.SceneField, LoadSceneMode.Additive)
                            : SceneManager.UnloadSceneAsync(operationData.SceneField);
                    }
                } else {
                    // Jika tidak menggunakan SceneField, maka kita akan menggunakan indeks scene untuk memuat atau meng-unload scene
                    {
                        loadingOperation = operationData.IsLoadingOperation
                            ? SceneManager.LoadSceneAsync(operationData.SceneIndex, LoadSceneMode.Additive)
                            : SceneManager.UnloadSceneAsync(operationData.SceneIndex);
                    }
                }

                // Menunggu hingga operasi scene selesai
                yield return loadingOperation;

                // Setelah operasi selesai, mengatur scene aktif ke scene yang ditentukan
                if (operationData.ActiveSceneIndex != 0) {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(operationData.ActiveSceneIndex));
                } else if (operationData.ActiveSceneField != null) {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(operationData.ActiveSceneField.SceneName));
                }
            }

            // Menandai bahwa operasi scene telah selesai dan menonaktifkan loading screen
            _isOperating = false;
            if (_loadingScreen != null) _loadingScreen.SetActive(false);

            // Jika masih ada operasi scene yang tersisa dalam antrian, jalankan kembali metode ini
            // _ = RunSceneOperationsCoroutine();
        }

        /// <summary>
        /// Metode ini akan memulai operasi scene dengan efek fade in dan fade out.
        /// Ini akan memulai fade out, memuat scene baru, meng-unload scene saat ini, dan kemudian memulai fade in.
        /// Ini akan mengatur scene aktif ke scene yang ditentukan setelah operasi selesai.
        /// Jika ada operasi scene yang sedang berjalan, maka metode ini akan menunggu hingga operasi tersebut selesai sebelum melanjutkan.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="fadeDuration"></param>
        /// <returns></returns>
        private IEnumerator StartSceneOperationsWithFade(int sceneIndex, float fadeDuration) {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (_fadingCoroutine != null) {
                StopCoroutine(_fadingCoroutine);
            }

            LoadScene(sceneIndex, true);
            UnLoadScene(currentSceneIndex, true);

            yield return StartFadeOut(fadeDuration);

            yield return RunSceneOperationsCoroutine();

            yield return StartFadeIn(fadeDuration);
        }

        private IEnumerator StartSceneOperationsWithFade(SceneField sceneField, float fadeDuration) {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (_fadingCoroutine != null) {
                StopCoroutine(_fadingCoroutine);
            }

            LoadScene(sceneField, true);
            UnLoadScene(currentSceneIndex, true);

            yield return StartFadeOut(fadeDuration);

            yield return RunSceneOperationsCoroutine();

            yield return StartFadeIn(fadeDuration);
        }

        #endregion

        // =========================================================================================================

        #region Public Methods

        /// <summary>
        /// Memuat scene berdasarkan indeks scene yang diberikan.
        /// Ini akan menambahkan operasi scene ke dalam antrian dan menjalankan operasi tersebut.
        /// Jika `onlyAddToQueue` diatur ke true, maka hanya akan menambahkan operasi ke antrian tanpa menjalankannya.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="onlyAddToQueue"></param>
        /// <param name="setActiveScene"></param>
        public void LoadScene(int sceneIndex, bool onlyAddToQueue = false, int setActiveScene = 0) {
            SceneOperation operation = new SceneOperation {
                ID = GUID.Generate().ToString(),
                SceneIndex = sceneIndex,
                ActiveSceneIndex = setActiveScene == 0 ? sceneIndex : setActiveScene, // Kalau setActiveScene tidak diberikan, gunakan sceneIndex sebagai setActiveScene
                IsLoadingOperation = true
            };

            _sceneQueue.Enqueue(operation);

            if (onlyAddToQueue) return;
            _ = StartCoroutine(RunSceneOperationsCoroutine());
        }

        public void LoadScene(SceneField sceneIndex, bool onlyAddToQueue = false, SceneField setActiveScene = null) {
            SceneOperation operation = new SceneOperation {
                ID = GUID.Generate().ToString(),
                UseSceneField = true,
                SceneField = sceneIndex,
                ActiveSceneField = setActiveScene ?? sceneIndex, // Kalau setActiveScene tidak diberikan, gunakan sceneIndex sebagai setActiveScene
                IsLoadingOperation = true
            };

            _sceneQueue.Enqueue(operation);

            if (onlyAddToQueue) return;
            _ = StartCoroutine(RunSceneOperationsCoroutine());
        }

        /// <summary>
        /// Meng-unload scene berdasarkan indeks scene yang diberikan.
        /// Ini akan menambahkan operasi scene ke dalam antrian dan menjalankan operasi tersebut.
        /// Jika `onlyAddToQueue` diatur ke true, maka hanya akan menambahkan operasi ke antrian tanpa menjalankannya.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="onlyAddToQueue"></param>
        /// <param name="setActiveScene"></param>
        public void UnLoadScene(int sceneIndex, bool onlyAddToQueue = false, int setActiveScene = 0) {
            SceneOperation operation = new SceneOperation {
                ID = GUID.Generate().ToString(),
                SceneIndex = sceneIndex,
                ActiveSceneIndex = setActiveScene,
                IsLoadingOperation = false
            };

            _sceneQueue.Enqueue(operation);

            if (onlyAddToQueue) return;
            _ = StartCoroutine(RunSceneOperationsCoroutine());
        }

        public void UnLoadScene(SceneField sceneIndex, bool onlyAddToQueue = false, SceneField setActiveScene = null) {
            SceneOperation operation = new SceneOperation {
                ID = GUID.Generate().ToString(),
                UseSceneField = true,
                SceneField = sceneIndex,
                ActiveSceneField = setActiveScene,
                IsLoadingOperation = false
            };

            _sceneQueue.Enqueue(operation);

            if (onlyAddToQueue) return;
            _ = StartCoroutine(RunSceneOperationsCoroutine());
        }

        /// <summary>
        /// Langsung beralih ke scene yang ditentukan berdasarkan indeks scene.
        /// Ini akan memuat scene baru dan meng-unload scene saat ini tanpa menunggu operasi scene lainnya.
        /// </summary>
        /// <param name="sceneIndex"></param>
        public void SwitchSceneDirect(int sceneIndex) {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            LoadScene(sceneIndex);
            UnLoadScene(currentSceneIndex, true);
        }

        public void SwitchSceneDirect(SceneField sceneField) {
            int currentSceneField = SceneManager.GetActiveScene().buildIndex;

            LoadScene(sceneField);
            UnLoadScene(currentSceneField, true);
        }

        /// <summary>
        /// Beralih ke scene yang ditentukan dengan efek fade in dan fade out.
        /// Ini akan memulai fade in, menjalankan operasi scene, dan kemudian memulai fade out.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="fadeDuration"></param>
        public void SwitchSceneWithFade(int sceneIndex, float fadeDuration) {
            _ = StartCoroutine(StartSceneOperationsWithFade(sceneIndex, fadeDuration));
        }

        public void SwitchSceneWithFade(SceneField sceneField, float fadeDuration) {
            _ = StartCoroutine(StartSceneOperationsWithFade(sceneField, fadeDuration));
        }

        #endregion

        // =========================================================================================================

#if UNITY_EDITOR
        [CustomEditor(typeof(SceneHandler))]
        public class SceneHandlerEditor : Editor {
            private SceneHandler _sceneHandler;

            private void OnEnable() {
                _sceneHandler = (SceneHandler)target;
            }

            public override void OnInspectorGUI() {
                DrawDefaultInspector();

                if (_sceneHandler._fadeInOnStart) {
                    _sceneHandler._sceneFadeDuration = EditorGUILayout.FloatField(
                        "Scene Fade Duration",
                        _sceneHandler._sceneFadeDuration
                    );
                }

                if (GUI.changed) {
                    EditorUtility.SetDirty(_sceneHandler);
                }
            }
        }

        [ContextMenu("Find all references that needed")]
        private void FindReferences() {
            Undo.RecordObject(this, "Find All References");
            _sceneFade = GetComponentInChildren<SceneFader>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
