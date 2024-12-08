using _Leonardo_Estigarribia._Scripts.Enemy;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Attack
{
    public abstract class BaseAttackState : State
    {
        protected abstract void PerformAttack(); 
        
        public override State RunCurrentState()
        {
            PerformAttack();
            return this;
        }
    }
}