using System.Collections.Generic;
using HellVillage.Data;
using UnityEngine;

namespace HellVillage.StateMachine {
    public abstract class StateCore : MonoBehaviour {
        public Rigidbody2D rigidBody;
        public MovementStats movementStats;
        public Animator animator;

        public StateMachine machine;

        public State state => machine.state;

        protected void Set(State newState, bool force = false) {
            machine.Set(newState, force);
        }

        public void SetupInstances() {
            machine = new StateMachine();

            State[] allChildStates = GetComponentsInChildren<State>();
            foreach (State state in allChildStates) {
                state.SetCore(this);
            }
        }

        #region Debugging Area
#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (Application.isPlaying && state != null) {
                List<State> activeStates = machine.GetActiveStateBranch();
                UnityEditor.Handles.Label(
                    new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z),
                    $"Speed: {rigidBody.linearVelocity.magnitude:F2} m/s\n" +
                    $"Velocity: {rigidBody.linearVelocity}\n" +
                    $"Current State: " + string.Join(" > ", activeStates));
            }
        }
#endif
        #endregion
    }
}
