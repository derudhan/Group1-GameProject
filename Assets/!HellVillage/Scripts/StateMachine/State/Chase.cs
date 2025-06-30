using System.Collections.Generic;
using UnityEngine;

namespace HellVillage.StateMachine {
    public class Chase : State {
        public List<Transform> listTargets;

        public Transform target;
        public Navigate navigate;
        public IdleState idle;

        public float checkRadius = 0.1f;
        public float vision = 1;

        public override void OnEnter() {
            navigate.destination = target.position;
            Set(navigate, true);
        }

        public override void DoUpdate() {
            if (state == navigate) {
                if (IsCloseEnough(target.position)) {
                    Set(idle, true);
                    rb.linearVelocity = Vector2.zero;
                    // TODO : Target lost animation
                    return;

                } else if (!InVision(target.position)) {
                    Set(idle, true);
                    rb.linearVelocity = Vector2.zero;

                } else {
                    navigate.destination = target.position;
                    Set(navigate, true);
                }

            } else {
                if (state.ElapsedTime > 1f) {
                    IsComplete = true;
                }
            }

            if (target == null) {
                IsComplete = true;
                return;
            }
        }

        public bool IsCloseEnough(Vector2 _targetPosition) {
            return Vector2.Distance((Vector2)core.transform.position, _targetPosition) < checkRadius;
        }

        public bool InVision(Vector2 _targetPosition) {
            return Vector2.Distance((Vector2)core.transform.position, _targetPosition) < vision;
        }

        public void CheckForTarget() {
            foreach (Transform t in listTargets) {
                if (InVision(t.position) && t.gameObject.activeSelf) {
                    target = t;
                    return;
                }
            }

            target = null;
        }
    }
}
