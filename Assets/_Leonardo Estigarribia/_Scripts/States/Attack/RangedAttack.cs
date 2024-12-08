using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _Leonardo_Estigarribia._Scripts.States.Attack
{
    public class RangedAttack : BaseAttackState
    {
        private bool isAttacking;
        [SerializeField] private int attackDamage = 2;
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private Vector3 attackOffset = new Vector3(0, 0.5f, 0.5f);
        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;
        
        protected override void PerformAttack()
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackCoroutine());
            }
            
        }

        private IEnumerator AttackCoroutine()
        {
            isAttacking = true;
            stateManager.animator.SetTrigger("attack");
            
            yield return new WaitForSeconds(attackingEventDelay);
            
            // Change this to be ranged attack. This was set for testing purposes.
            Vector3 attackPos = transform.position + transform.forward * attackOffset.z + transform.up * attackOffset.y;
            Collider[] hitColliders = Physics.OverlapSphere(attackPos, attackRadius, LayerMask.GetMask("Player"));

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<EntityStats>(out var entityStats))
                {
                    entityStats.TakeDamage(attackDamage);
                    break;
                }
            }
            //
            attackingEvent.Invoke();
            isAttacking = false;
            
        }

        protected override void ResetAttackAnimation()
        {
            
        }
    }
}