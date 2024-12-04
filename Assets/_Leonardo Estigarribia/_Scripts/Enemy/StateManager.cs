using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] private State currentState;

        void Update()
        {
            RunStateMachine();
        }

        private void RunStateMachine()
        {
            // If currentState is not null, run current state.
            State nextState = currentState?.RunCurrentState();

            if (nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }

        private void SwitchToNextState(State nextState)
        {
            currentState = nextState;
        }
}
}
