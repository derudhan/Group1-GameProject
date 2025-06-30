using UnityEngine;

namespace HellVillage.StateMachine {
    public class Patrol : State {
        public Navigate navigate;
        public IdleState idle;
        public Transform anchor1;
        public Transform anchor2;

        void GoToNextDestination() {
            if (navigate.destination == (Vector2)anchor1.position) {
                navigate.destination = (Vector2)anchor2.position;
            } else {
                navigate.destination = (Vector2)anchor1.position;
            }

            Set(navigate, true);
        }

        public override void OnEnter() {
            GoToNextDestination();
        }

        public override void DoUpdate() {
            if (state == navigate) {
                if (navigate.IsComplete) {
                    Set(idle, true);
                    rb.linearVelocity = new Vector2(0, 0);
                }
            } else {
                if (machine.state.ElapsedTime > 1f) {
                    GoToNextDestination();
                }
            }
        }
    }

}
