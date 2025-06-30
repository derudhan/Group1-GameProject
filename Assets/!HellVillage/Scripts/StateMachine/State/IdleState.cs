using UnityEngine;

namespace HellVillage.StateMachine {
    public class IdleState : State {
        [SerializeField] protected string animationStateName;
        protected int animationStateHash => Animator.StringToHash(animationStateName);

        public override void OnEnter() {
            animator.CrossFade(animationStateHash, 0f);
        }
    }
}
