using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

namespace _Leonardo_Estigarribia._Scripts.States.Refactor.Attack
{
    public class MeleeAttack : BaseAttackState
    {
        private bool isAttacking;
        [SerializeField] private int attackDamage = 2;
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private Vector3 attackOffset = new Vector3(0, 1f, 1f);
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
            stateManager.animator.SetTrigger("Attack");

            yield return new WaitForSeconds(attackingEventDelay);
            
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
            
            attackingEvent.Invoke();
            isAttacking = false;
        }

        protected override void ResetAttackAnimation()
        {
            stateManager.animator.SetFloat("Horizontal", 0f);
        }
        
        
    }
}