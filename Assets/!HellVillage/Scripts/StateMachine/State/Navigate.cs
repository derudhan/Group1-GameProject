using HellVillage.Utils;
using UnityEngine;

namespace HellVillage.StateMachine {
    public class Navigate : State {
        public Vector2 destination;
        public float speed = 1f;
        public float threshold = 0.1f;
        public State anim;

        private Vector2 _direction;

        public override void OnEnter() {
            Set(anim, true);
        }

        public override void DoUpdate() {
            if (Vector2.Distance((Vector2)core.transform.position, destination) < threshold) {
                IsComplete = true;
            }
            HandleAnimator();
        }

        public override void DoFixedUpdate() {
            _direction = (destination - (Vector2)core.transform.position).normalized;
            rb.linearVelocity = _direction * speed;
        }

        public override void OnExit() {
            animator.SetFloat(Const.animatorLastHorizontal, _direction.x);
            animator.SetFloat(Const.animatorLastVertical, _direction.y);
        }


        private void HandleAnimator() {
            animator.SetFloat(Const.animatorHorizontal, _direction.x);
            animator.SetFloat(Const.animatorVertical, _direction.y);
        }
    }
}
