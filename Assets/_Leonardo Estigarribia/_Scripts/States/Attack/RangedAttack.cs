using System;
using System.Collections;
using _Leonardo_Estigarribia._Scripts.Projectiles;
using Cinemachine.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Leonardo_Estigarribia._Scripts.States.Attack
{
    public class RangedAttack : BaseAttackState
    {
        private bool isAttacking;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Vector3 shootPositionOffset = new(0, 0.5f, 0.5f);
        [SerializeField] private float lookDirectionOffsetY = 1f;
        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;

        protected override void Start()
        {
            base.Start();
            projectilePool = GetComponentInChildren<ProjectilePool>();
        }

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
            
            Vector3 shootPosition = transform.position + 
                                    transform.right * shootPositionOffset.x + 
                                    transform.up * shootPositionOffset.y + 
                                    transform.forward * shootPositionOffset.z;
    
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(shootPosition, 0.1f);

            Vector3 targetPosition = stateManager.playerTransform.position + Vector3.up * lookDirectionOffsetY;
            
            Vector3 directionToPlayer = (targetPosition - shootPosition).normalized;

            Projectile projectile = projectilePool.GetProjectile();
            projectile.transform.position = shootPosition;
            projectile.ShootToDirection(directionToPlayer);
            
            attackingEvent.Invoke();
            isAttacking = false;
            
        }

        protected override void ResetAttackAnimation()
        {
            
        }
    }
}