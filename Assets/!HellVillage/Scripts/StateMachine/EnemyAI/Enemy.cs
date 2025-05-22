namespace HellVillage {
    public class Enemy : StateCore {
        public Patrol patrol;

        void Start() {
            SetupInstances();
            Set(patrol);
        }

        void Update() {
            if (state.IsComplete) { }

            state.DoUpdateBranch();
        }

        private void FixedUpdate() {
            state.DoFixedUpdateBranch();
        }
    }
}
