using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class AttackState : State
    {
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackDamage = 10f;
        private float nextAttackTime;
    
        public override State RunCurrentState()
        {
            if (Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        
            return this;
        }
    
        private void PerformAttack()
        {
            Debug.Log($"Attacking for {attackDamage} damage!");
        }
    }
}