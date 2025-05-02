using UnityEngine;

namespace HellVillage
{
    public class PlayerRun : State
    {

        public override void OnEnter()
        {
            animator.CrossFade(playerControl._walkAnimatorHash, 0f);
        }

        public override void DoUpdate()
        {
            if (!playerControl._runIsHeld)
            {
                IsComplete = true;
            }
        }

        public override void OnExit() { }
    }
}
