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
        private EntityStats playerStats;

        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;
        
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private int attackDamage = 10;
        private float nextAttackTime;
        private bool isPlayerNull;
        
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private Vector3 attackOffset = new Vector3(0, 1f, 1f);

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform;
            isPlayerNull = player == null;
            stateManager = GetComponent<StateManager>();
            chaseState = GetComponent<ChaseState>();
        }
        
        public override State RunCurrentState()
        {
            if (isPlayerNull) return this;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer > attackRange)
            {
                stateManager.animator.SetFloat("Horizontal", 0f);
                return chaseState;
            }

            if (Time.time >= nextAttackTime)
            {
                StartCoroutine(Attack());
                nextAttackTime = Time.time + attackCooldown;
            }
    
            return this;
        }
    
        private IEnumerator Attack()
        {
            stateManager.animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackingEventDelay);
            
            // Create attack sphere in front of enemy
            Vector3 attackPos = transform.position + transform.forward * attackOffset.z + transform.up * attackOffset.y;
            Collider[] hitColliders = Physics.OverlapSphere(attackPos, attackRadius, LayerMask.GetMask("Player"));

            // Check if player was hit
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<EntityStats>(out var entityStats))
                {
                    entityStats.TakeDamage(attackDamage);
                    break;
                }
            }
       
            attackingEvent.Invoke();
        }
        
        /*private void OnDrawGizmosSelected()
        {
            Vector3 attackPos = transform.position + transform.forward * attackOffset.z + transform.up * attackOffset.y;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos, attackRadius);
        }*/
    }
}