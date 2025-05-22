using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace HellVillage {
    public class CameraManager : MonoBehaviour {
        public static CameraManager instance;

        [SerializeField] private CinemachineCamera[] _allVirtualCameras;

        private Coroutine _panCameraCoroutine;

        private CinemachineCamera _currentCamera;
        private CinemachinePositionComposer _positionComposer;
        private Vector2 _positionComposerOffset;

        [SerializeField] private PlayerControl _player;

        private void Awake() {
            if (instance == null) {
                instance = this;
            }

            for (int i = 0; i < _allVirtualCameras.Length; i++) {
                if (_allVirtualCameras[i].enabled) {
                    //set the current active camera
                    _currentCamera = _allVirtualCameras[i];

                    //set the position transposer
                    _positionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
                }
            }

            _positionComposerOffset = _positionComposer.TargetOffset;
        }

        #region Pan Camera

        public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection) {
            Vector2 endPosition = Vector2.zero;
            Vector2 startingPosition = Vector2.zero;

            endPosition = GetPanDirection(panDirection);

            endPosition *= panDistance;
            startingPosition = _positionComposerOffset;
            endPosition += startingPosition;

            DOTween.To(() => (Vector2)_positionComposer.TargetOffset, x => _positionComposer.TargetOffset = x, endPosition, panTime);
        }

        public void LeavePanCamera(float panTime) {
            DOTween.To(() => (Vector2)_positionComposer.TargetOffset, x => _positionComposer.TargetOffset = x, Vector2.zero, panTime);
        }

        private Vector2 GetPanDirection(PanDirection panDirection) {
            switch (panDirection) {
                case PanDirection.Top:
                    return Vector2.up;
                case PanDirection.Bottom:
                    return Vector2.down;
                case PanDirection.Left:
                    return Vector2.left;
                case PanDirection.Right:
                    return Vector2.right;
                case PanDirection.TopLeft:
                    return new Vector2(-1f, 1f);
                case PanDirection.TopRight:
                    return new Vector2(1f, 1f);
                case PanDirection.BottomLeft:
                    return new Vector2(-1f, -1f);
                case PanDirection.BottomRight:
                    return new Vector2(1f, -1f);
            }
            return Vector2.zero;
        }

        #endregion

        #region Swap Cameras

        public void SwapCamera(CinemachineCamera cameraSideOne, CinemachineCamera cameraSideTwo, Vector2 triggerExitDirection) {
            //if the current camera is the camera on the left and our trigger exit direction was on the right
            if (_currentCamera == cameraSideOne && triggerExitDirection.x > 0f) {
                //activate the new camera
                cameraSideTwo.enabled = true;

                //deactivate the old camera
                cameraSideOne.enabled = false;

                //set the new camera as the current camera
                _currentCamera = cameraSideTwo;

                //update our composer variable
                _positionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
            }

            //if the currnet camera is the camera on th eright and our trigger hit direction was on the left
            else if (_currentCamera == cameraSideTwo && triggerExitDirection.x < 0f) {
                //activate the new camera
                cameraSideOne.enabled = true;

                //deactivate the old camera
                cameraSideTwo.enabled = false;

                //set the new camera as the current camera
                _currentCamera = cameraSideOne;

                //update our composer variable
                _positionComposer = _currentCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer; ;
            }
        }

        #endregion
    }
}