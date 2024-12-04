using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class StateManager : MonoBehaviour
    {
        public Animator _animator;
        [SerializeField] private State currentState;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
            {
                Debug.Log($"No animator found for {gameObject.name}.");
            }
        }

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
