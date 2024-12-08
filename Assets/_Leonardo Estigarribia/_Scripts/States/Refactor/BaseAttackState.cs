using _Leonardo_Estigarribia._Scripts.Enemy;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor
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