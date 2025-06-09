namespace HellVillage.StateMachine {
    public class Enemy : StateCore {
        public Patrol patrol;
        public Chase chase;

        void Start() {
            SetupInstances();
            Set(patrol);
        }

        void Update() {
            if (state.IsComplete) {
                if (state == chase) {
                    Set(patrol);
                }
            }

            if (state == patrol) {
                chase.CheckForTarget();

                if (chase.target != null) {
                    Set(chase);
                }
            }

            state.DoUpdateBranch();
        }

        private void FixedUpdate() {
            state.DoFixedUpdateBranch();
        }
    }
}
