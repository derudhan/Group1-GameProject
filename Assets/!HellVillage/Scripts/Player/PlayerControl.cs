using DG.Tweening;
using UnityEngine;

/// <summary>
/// Skrip untuk mengontrol gerakan pemain (top-down 2D) serta aksi lainnya.
/// </summary>

namespace HellVillage {
    public class PlayerControl : StateCore {
        public Transform CameraTargetTransform;
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
        }

        private void FixedUpdate() {
            HandleMovement();
            ApplyFriction();
            HandleAnimator();
            HandleCameraBias();
        }

        private void CheckInput() {
            _movementInput = InputManager.Movement.normalized;
            _runIsHeld = InputManager.RunIsHeld;
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
                Vector2 target = state == runState ? _movementInput * 2 : _movementInput;
                CameraTargetTransform.DOLocalMove(target, CameraBiasTime).SetEase(Ease.InOutSine);
            }
        }
    }
}
