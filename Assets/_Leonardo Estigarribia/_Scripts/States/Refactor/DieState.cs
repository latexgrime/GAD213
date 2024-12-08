using _Leonardo_Estigarribia._Scripts.States;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class DieState : State
    {
        private StateManager stateManager;

        private void Start()
        {
            stateManager = GetComponent<StateManager>();
        }

        public override State RunCurrentState()
        {
            stateManager.animator.SetBool("Die", true);
            stateManager.animator.SetFloat("Horizontal", 0f);
            return this;
        }
    }
}