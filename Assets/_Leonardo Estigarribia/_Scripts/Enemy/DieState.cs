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
            stateManager._animator.SetBool("Die", true);
            stateManager._animator.SetFloat("Horizontal", 0f);
            return this;
        }
    }
}