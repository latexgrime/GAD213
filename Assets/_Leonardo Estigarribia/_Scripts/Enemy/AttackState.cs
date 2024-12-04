using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _Leonardo_Estigarribia._Scripts.Enemy
{
    public class AttackState : State
    {
        private StateManager stateManager;
        private ChaseState chaseState;

        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;
        
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackDamage = 10f;
        private float nextAttackTime;

        private void Start()
        {
            stateManager = GetComponent<StateManager>();
            chaseState = GetComponent<ChaseState>();
        }

        public override State RunCurrentState()
        {
            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(PerformAttack());
                nextAttackTime = Time.time + attackCooldown;
            }
        
            return chaseState;
        }
    
        private IEnumerator PerformAttack()
        {
            Debug.Log($"Attacking for {attackDamage} damage!");
            stateManager._animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackingEventDelay);
            
            attackingEvent.Invoke();
        }
    }
}