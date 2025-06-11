using DG.Tweening;
using HellVillage.Player2DRPG;
using Unity.Cinemachine;
using UnityEngine;

namespace HellVillage.Cameras {
    public class CameraManager : MonoBehaviour {
        public static CameraManager Instance { get; private set; }

        private GameObject _player;
        private CinemachineCamera _currentActiveCamera;
        private CinemachinePositionComposer _positionComposer;
        private Vector2 _positionComposerOffset;

        private void Awake() {
            if (Instance == null) Instance = this;
        }

        private void OnEnable() {
            CinemachineCore.CameraActivatedEvent.AddListener(OnCameraActivated);
        }
        private void OnDisable() {
            CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraActivated);
        }

        private void OnCameraActivated(ICinemachineCamera.ActivationEventParams arg0) {
            _currentActiveCamera = arg0.IncomingCamera as CinemachineCamera;
            if (_currentActiveCamera != null) {
                _positionComposer = _currentActiveCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
                _positionComposerOffset = _positionComposer.TargetOffset;
            }
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
            if (_currentActiveCamera == cameraSideOne && triggerExitDirection.x > 0f) {
                //activate the new camera
                cameraSideTwo.enabled = true;

                //deactivate the old camera
                cameraSideOne.enabled = false;

                //set the new camera as the current camera
                _currentActiveCamera = cameraSideTwo;

                //update our composer variable
                _positionComposer = _currentActiveCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
            }

            //if the currnet camera is the camera on th eright and our trigger hit direction was on the left
            else if (_currentActiveCamera == cameraSideTwo && triggerExitDirection.x < 0f) {
                //activate the new camera
                cameraSideOne.enabled = true;

                //deactivate the old camera
                cameraSideTwo.enabled = false;

                //set the new camera as the current camera
                _currentActiveCamera = cameraSideOne;

                //update our composer variable
                _positionComposer = _currentActiveCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer; ;
            }
        }

        #endregion
    }
}