using UnityEngine;

namespace HellVillage {
    public class RunState : State {
        [SerializeField] protected string animationStateName;
        protected int animationStateHash => Animator.StringToHash(animationStateName);

        public override void OnEnter() {
            animator.CrossFade(animationStateHash, 0f);
        }

        public override void DoUpdate() {
            animator.speed = Helpers.Map(core.movementStats.MaxRunSpeed, 0, 1, 0, 1.5f, true);
        }
    }
}
