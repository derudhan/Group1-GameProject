using UnityEngine;

namespace HellVillage
{
    public class PlayerWalk : State
    {
        public override void OnEnter()
        {
            animator.CrossFade(playerControl._walkAnimatorHash, 0f);
        }

        public override void DoUpdate()
        {
        }

        public override void OnExit() { }
    }
}
