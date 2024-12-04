using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class AttackState : State
    {
        private ChaseState chaseState;
        
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackDamage = 10f;
        private float nextAttackTime;

        private void Start()
        {
            chaseState = GetComponent<ChaseState>();
        }

        public override State RunCurrentState()
        {
            if (Time.time >= nextAttackTime)
            {
                PerformAttack();
                nextAttackTime = Time.time + attackCooldown;
            }
        
            return chaseState;
        }
    
        private void PerformAttack()
        {
            Debug.Log($"Attacking for {attackDamage} damage!");
        }
    }
}