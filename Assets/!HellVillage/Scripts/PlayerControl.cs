using System;
using DG.Tweening;
using HellVillage.Data;
using HellVillage.Input;
using HellVillage.StateMachine;
using HellVillage.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HellVillage.Player2DRPG {
    /// <summary>
    /// Skrip untuk mengontrol gerakan pemain (2D RPG Style) serta aksi lainnya.
    /// </summary>
    public class PlayerControl : StateCore, IBind<PlayerData> {
        public UnityEvent OnPausePressed = new UnityEvent();

        public string Id { get; } = System.Guid.NewGuid().ToString();
        [SerializeField] public PlayerData _playerData;

        public Transform CameraTargetTransform;
        public float CameraWalkDistance = 0.5f;
        public float CameraRunDistance = 1f;
        public float CameraBiasTime = 0.2f;

        public IdleState idleState;
        public WalkState walkState;
        public RunState runState;

        public Vector2 _movementInput { get; private set; }
        public bool _runIsHeld { get; private set; }

        private void Start() {
            SetupInstances();
            Set(idleState);
        }

        private void Update() {
            CheckInput();

            SelectState();
            state.DoUpdateBranch();

            DataUpdate();
        }

        private void FixedUpdate() {
            HandleMovement();
            ApplyFriction();
            HandleAnimator();
            HandleCameraBias();
        }

        private void CheckInput() {
            _movementInput = InputManager.PlayerAction.Move.ReadValue<Vector2>().normalized;
            _runIsHeld = InputManager.PlayerAction.Sprint.IsPressed();
            InputManager.PlayerAction.Pause.performed += OnPausePressed_Action;
        }

        private void OnPausePressed_Action(InputAction.CallbackContext context) {
            DisableInput();
            OnPausePressed.Invoke();
        }

        /// <summary>
        /// Function untuk mengatur state pemain berdasarkan input.
        /// </summary>
        private void SelectState() {
            if (_movementInput.magnitude > 0.1f) {
                if (_runIsHeld) {
                    Set(runState);
                } else {
                    Set(walkState);
                }
            } else {
                Set(idleState);
            }
        }

        private void HandleMovement() {
            if (Mathf.Abs(_movementInput.magnitude) > 0) {
                // Tentukan kecepatan berdasarkan apakah pemain berlari atau tidak
                float maxSpeed = _runIsHeld ? movementStats.MaxRunSpeed : movementStats.MaxWalkSpeed;

                // Akselerasi velocity dengan variabel dari movementStats lalu clamp ke max speed
                Vector2 incremental = _movementInput * movementStats.Acceleration;
                Vector2 newSpeed = Vector2.ClampMagnitude(rigidBody.linearVelocity + incremental, maxSpeed);
                rigidBody.linearVelocity = newSpeed;
            }
        }

        private void ApplyFriction() {
            // Jika tidak ada input, maka terapkan friction
            if (_movementInput == Vector2.zero) {
                rigidBody.linearVelocity = Vector2.Lerp(rigidBody.linearVelocity, Vector2.zero, movementStats.Deceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleAnimator() {
            animator.SetFloat(Const.animatorHorizontal, _movementInput.x);
            animator.SetFloat(Const.animatorVertical, _movementInput.y);

            if (_movementInput != Vector2.zero) {
                animator.SetFloat(Const.animatorLastHorizontal, _movementInput.x);
                animator.SetFloat(Const.animatorLastVertical, _movementInput.y);
                animator.speed = Helpers.Map(movementStats.MaxWalkSpeed, 0, 1, 0, 1, true);
            }
        }

        private void HandleCameraBias() {
            if (CameraTargetTransform != null && _movementInput != Vector2.zero) {
                Vector2 target;

                if (state == runState) {
                    target = _movementInput * CameraRunDistance;
                } else {
                    target = _movementInput * CameraWalkDistance;
                }

                CameraTargetTransform.DOLocalMove(target, CameraBiasTime).SetEase(Ease.InOutSine);
            }
        }

        private void DataUpdate() {
            _playerData.position = transform.position;
        }

        public void EnableInput() {
            InputManager.PlayerAction.Enable();
        }
        public void DisableInput() {
            InputManager.PlayerAction.Disable();
        }

        public void Bind(PlayerData data) {
            this._playerData = data;
            this._playerData.Id = Id;

            transform.position = data.position;
        }
    }

    [Serializable]
    public class PlayerData : ISaveable {
        [field: SerializeField] public string Id { get; set; }
        public Vector2 position;
    }
}
