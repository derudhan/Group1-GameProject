using UnityEngine;

namespace HellVillage {
    public class RunState : State {
        [SerializeField] protected string animationStateName;
        protected int animationStateHash => Animator.StringToHash(animationStateName);

        public override void OnEnter() {
            animator.CrossFade(animationStateHash, 0f);
        }

        public override void DoUpdate() {
            Vector2 vel = rb.linearVelocity;
            animator.speed = Helpers.Map(Mathf.Abs(vel.magnitude), 0, 1, 0, 1.5f, true);
        }
    }
}
