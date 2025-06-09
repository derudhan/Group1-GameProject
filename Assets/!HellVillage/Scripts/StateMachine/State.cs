using HellVillage.Data;
using UnityEngine;

/// <summary>
/// Skrip abstrak untuk state machine.
/// Class ini digunakan sebagai dasar untuk state-state lain.
/// </summary>
namespace HellVillage.StateMachine {
    public abstract class State : MonoBehaviour {
        public bool IsComplete { get; protected set; }

        protected float startTime;
        public float ElapsedTime => Time.time - startTime;

        protected StateCore core;

        protected Rigidbody2D rb => core.rigidBody;
        protected MovementStats playerStats => core.movementStats;
        protected Animator animator => core.animator;

        public StateMachine machine;
        public StateMachine parent;
        public State state => machine.state;

        protected void Set(State newState, bool force = false) {
            machine.Set(newState, force);
        }

        public void SetCore(StateCore _core) {
            machine = new StateMachine();
            core = _core;
        }

        public virtual void OnEnter() { }
        public virtual void DoUpdate() { }
        public virtual void DoFixedUpdate() { }
        public virtual void OnExit() { }

        public void DoUpdateBranch() {
            DoUpdate();
            state?.DoUpdateBranch();
        }

        public void DoFixedUpdateBranch() {
            DoFixedUpdate();
            state?.DoFixedUpdateBranch();
        }

        public void Initialize(StateMachine _parent) {
            parent = _parent;
            startTime = Time.time;
            IsComplete = false;
        }
    }
}
