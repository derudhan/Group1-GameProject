using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace HellVillage {
    public class CameraControlTrigger : MonoBehaviour {
        public CustomInspectorObjects customInspectorObjects;

        private Collider2D _coll;

        private void Start() {
            _coll = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("Player")) {
                if (customInspectorObjects.panCameraOnContact) {
                    //pan the camera based on the pan direction in the inspector
                    CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.CompareTag("Player")) {
                Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

                if (customInspectorObjects.swapCameras && customInspectorObjects.cameraSideOne != null && customInspectorObjects.cameraSideTwo != null) {
                    //swap cameras
                    CameraManager.instance.SwapCamera(customInspectorObjects.cameraSideOne, customInspectorObjects.cameraSideTwo, exitDirection);
                }

                if (customInspectorObjects.panCameraOnContact) {
                    //pan the camera back to the starting position
                    CameraManager.instance.LeavePanCamera(customInspectorObjects.panTime);
                }
            }
        }
    }

    [System.Serializable]
    public class CustomInspectorObjects {
        public bool swapCameras = false;
        public bool panCameraOnContact = false;

        [HideInInspector] public CinemachineCamera cameraSideOne;
        [HideInInspector] public CinemachineCamera cameraSideTwo;

        [HideInInspector] public PanDirection panDirection;
        [HideInInspector] public float panDistance = 3f;
        [HideInInspector] public float panTime = 0.35f;
    }

    public enum PanDirection {
        Top,
        Bottom,
        Left,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CameraControlTrigger))]
    public class MyScriptEditor : Editor {
        CameraControlTrigger cameraControlTrigger;

        private void OnEnable() {
            cameraControlTrigger = (CameraControlTrigger)target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (cameraControlTrigger.customInspectorObjects.swapCameras) {
                cameraControlTrigger.customInspectorObjects.cameraSideOne = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.cameraSideOne,
                    typeof(CinemachineCamera), true) as CinemachineCamera;

                cameraControlTrigger.customInspectorObjects.cameraSideTwo = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.cameraSideTwo,
                    typeof(CinemachineCamera), true) as CinemachineCamera;
            }

            if (cameraControlTrigger.customInspectorObjects.panCameraOnContact) {
                cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
                    cameraControlTrigger.customInspectorObjects.panDirection);

                cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
                cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(cameraControlTrigger);
            }
        }
    }
#endif
}