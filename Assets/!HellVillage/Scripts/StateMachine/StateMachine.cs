using System.Collections.Generic;

namespace HellVillage.StateMachine {
    public class StateMachine {
        public State state;

        public void Set(State newState, bool forceReset = false) {
            if (state != newState || forceReset) {
                state?.OnExit();
                state = newState;
                state.Initialize(this);
                state.OnEnter();
            }
        }


        public List<State> GetActiveStateBranch(List<State> list = null) {
            if (list == null) {
                list = new List<State>();
            }

            if (state == null) {
                return list;
            } else {
                list.Add(state);
                return state.machine.GetActiveStateBranch(list);
            }
        }
    }
}
