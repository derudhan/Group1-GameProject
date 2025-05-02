using UnityEngine;

/// <summary>
/// Skrip untuk mengontrol gerakan pemain (top-down 2D) serta aksi lainnya.
/// </summary>

namespace HellVillage
{
    public class PlayerControl : MonoBehaviour
    {
        State _state;
        public PlayerIdle idleState;
        public PlayerWalk walkState;
        public PlayerRun runState;

        public Vector2 _movementInput { get; private set; }
        public bool _runIsHeld { get; private set; }
        public int _idleAnimatorHash { get; private set; }
        public int _walkAnimatorHash { get; private set; }

        [SerializeField] Rigidbody2D _rb;
        [SerializeField] PlayerStats _playerStats;
        [SerializeField] Animator _animator;

        const string _animatorHorizontal = "Horizontal";
        const string _animatorVertical = "Vertical";
        const string _animatorLastVertical = "LastVertical";
        const string _animatorLastHorizontal = "LastHorizontal";

        private void Start()
        {
            _idleAnimatorHash = Animator.StringToHash("Idle");
            _walkAnimatorHash = Animator.StringToHash("Walk");

            idleState.Setup(_rb, _playerStats, _animator, this);
            walkState.Setup(_rb, _playerStats, _animator, this);
            runState.Setup(_rb, _playerStats, _animator, this);
            _state = idleState;
        }

        private void Update()
        {
            CheckInput();
            HandleAnimator();

            SelectState();
            _state.DoUpdate();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            ApplyFriction();
        }

        private void CheckInput()
        {
            _movementInput = InputManager.Movement.normalized;
            _runIsHeld = InputManager.RunIsHeld;
        }

        /// <summary>
        /// Function untuk mengatur state pemain berdasarkan input.
        /// </summary>
        private void SelectState()
        {
            State oldState = _state;

            if (_movementInput.magnitude != 0)
            {
                if (_runIsHeld)
                {
                    _state = runState;
                }
                else
                {
                    _state = walkState;
                }
            }
            else
            {
                _state = idleState;
            }

            if (_state != oldState || _state.IsComplete)
            {
                oldState.OnExit();
                _state.Initialize();
                _state.OnEnter();
            }

        }

        private void HandleMovement()
        {
            if (Mathf.Abs(_movementInput.magnitude) > 0)
            {
                // Tentukan kecepatan berdasarkan apakah pemain berlari atau tidak
                float maxSpeed = _runIsHeld ? _playerStats.MaxRunSpeed : _playerStats.MaxWalkSpeed;

                // Akselerasi velocity dengan variabel dari _playerStats lalu clamp ke max speed
                Vector2 incremental = _movementInput * _playerStats.Acceleration;
                Vector2 newSpeed = Vector2.ClampMagnitude(_rb.linearVelocity + incremental, maxSpeed);
                _rb.linearVelocity = newSpeed;
            }
        }

        private void ApplyFriction()
        {
            // Jika tidak ada input, maka terapkan friction
            if (_movementInput == Vector2.zero)
            {
                _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, _playerStats.Deceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleAnimator()
        {
            _animator.SetFloat(_animatorVertical, _movementInput.y);
            _animator.SetFloat(_animatorHorizontal, _movementInput.x);

            if (_movementInput != Vector2.zero)
            {
                _animator.SetFloat(_animatorLastVertical, _movementInput.y);
                _animator.SetFloat(_animatorLastHorizontal, _movementInput.x);
            }
        }

        #region Debugging Area
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                UnityEditor.Handles.Label(
                    new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z),
                    $"Speed: {_rb.linearVelocity.magnitude:F2} m/s\n" +
                    $"Input: {_movementInput}\n" +
                    $"Current State: {_state.GetType().Name}");
            }
        }
#endif
        #endregion
    }
}
