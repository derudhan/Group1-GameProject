using Unity.Cinemachine;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HellVillage.CameraSystem {
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

    /// <summary>
    /// This script is used to control camera behavior when the player enters or exits a trigger area.
    /// It allows for camera panning and swapping between two cameras based on the player's position relative to the trigger.
    /// The script uses a custom inspector to configure the camera behavior, including pan direction, distance, and time.
    /// </summary>
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
                    CameraManager.Instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.CompareTag("Player")) {
                Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

                if (customInspectorObjects.swapCameras && customInspectorObjects.cameraSideOne != null && customInspectorObjects.cameraSideTwo != null) {
                    //swap cameras
                    CameraManager.Instance.SwapCamera(customInspectorObjects.cameraSideOne, customInspectorObjects.cameraSideTwo, exitDirection);
                }

                if (customInspectorObjects.panCameraOnContact) {
                    //pan the camera back to the starting position
                    CameraManager.Instance.LeavePanCamera(customInspectorObjects.panTime);
                }
            }
        }
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