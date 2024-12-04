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
        private Transform player;

        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;
        
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float attackDamage = 10f;
        private float nextAttackTime;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            stateManager = GetComponent<StateManager>();
            chaseState = GetComponent<ChaseState>();
        }
        
        [SerializeField] private float attackRange = 2f;
        public override State RunCurrentState()
        {
            if (player == null) return this;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > attackRange)
            {
                stateManager._animator.SetFloat("Horizontal", 0f); // Reset animation
                return chaseState;
            }

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(PerformAttack());
                nextAttackTime = Time.time + attackCooldown;
            }
    
            return this; // Stay in attack state while in range
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