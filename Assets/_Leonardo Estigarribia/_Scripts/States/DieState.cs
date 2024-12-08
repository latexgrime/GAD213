using _Leonardo_Estigarribia._Scripts.Enemy;

namespace _Leonardo_Estigarribia._Scripts.States
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
            return this;
        }
    }
}