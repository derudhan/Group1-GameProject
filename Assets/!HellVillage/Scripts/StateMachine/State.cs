using UnityEngine;

/// <summary>
/// Skrip abstrak untuk state machine.
/// Class ini digunakan sebagai dasar untuk state-state lain.
/// </summary>
namespace HellVillage
{
    public abstract class State : MonoBehaviour
    {
        public bool IsComplete { get; protected set; }

        protected float startTime;
        public float ElapsedTime => Time.time - startTime;

        [SerializeField] protected string animationName;

        protected Rigidbody2D rb;
        protected PlayerStats playerStats;
        protected Animator animator;
        protected PlayerControl playerControl;

        public virtual void OnEnter() { }
        public virtual void DoUpdate() { }
        public virtual void DoFixedUpdate() { }
        public virtual void OnExit() { }

        public void Initialize()
        {
            startTime = Time.time;
            IsComplete = false;
        }

        public void Setup(Rigidbody2D rb, PlayerStats playerStats, Animator animator, PlayerControl playerControl)
        {
            this.rb = rb;
            this.playerStats = playerStats;
            this.animator = animator;
            this.playerControl = playerControl;
        }
    }
}
