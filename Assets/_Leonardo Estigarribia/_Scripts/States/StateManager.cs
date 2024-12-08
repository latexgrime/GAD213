using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Serialization;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class StateManager : MonoBehaviour
    {
        public Animator animator;
        
        private State idleState;
        private State chaseState;
        private State attackState;
        private State dieState;
        
        [SerializeField] private State currentState;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();

            idleState = GetComponent<IdleState>();
            chaseState = GetComponent<ChaseState>();
            attackState = GetComponent<AttackState>();
            dieState = GetComponent<DieState>();
        }

        private void Update()
        {
            RunStateMachine();
        }

        private void RunStateMachine()
        {
            // If currentState is not null, run current state.
            var nextState = currentState?.RunCurrentState();

            if (nextState != null) SwitchToNextState(nextState);
        }

        private void SwitchToNextState(State nextState)
        {
            currentState = nextState;
        }

        public void SetStateToDead()
        {
            currentState = dieState;
        }
    }
}