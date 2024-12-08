using _Leonardo_Estigarribia._Scripts.Enemy;
using _Leonardo_Estigarribia._Scripts.States.Attack;
using _Leonardo_Estigarribia._Scripts.States.Idle;
using _Leonardo_Estigarribia._Scripts.States.Movement;
using _Leonardo_Estigarribia._Scripts.States.Refactor;
using _Leonardo_Estigarribia._Scripts.States.Refactor.Idle;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using State = _Leonardo_Estigarribia._Scripts.Enemy.State;

namespace _Leonardo_Estigarribia._Scripts.States
{
    public class StateManager : MonoBehaviour
    {
        public Animator animator;
        public Transform playerTransform;

        public State idleState;
        public State movementState;
        public State attackState;
        public State dieState;
        
        [SerializeField] private State currentState;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            
            idleState = GetComponent<BaseIdleState>();
            movementState = GetComponent<BaseMovementState>();
            attackState = GetComponent<BaseAttackState>();
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