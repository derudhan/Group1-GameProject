using UnityEngine;

namespace HellVillage
{
    public class PlayerIdle : State
    {
        public override void OnEnter()
        {
            animator.CrossFade(playerControl._idleAnimatorHash, 0f);
        }

        public override void DoUpdate()
        {
        }

        public override void OnExit() { }
    }
}
