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
        [SerializeField] private Vector3 lookDirectionOffset = new Vector3(0, 0.5f, 0.5f);
        [SerializeField] private float attackingEventDelay;
        [SerializeField] private UnityEvent attackingEvent;

        protected override void Start()
        {
            base.Start();
            if (projectilePool == null)
            {
                projectilePool = GetComponentInChildren<ProjectilePool>();
            }
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
            transform.LookAt(stateManager.playerTransform.position);

            yield return new WaitForSeconds(attackingEventDelay);

            Vector3 shootPosition = transform.position + transform.forward * lookDirectionOffset.z + transform.up * lookDirectionOffset.y;
            Vector3 directionToPlayer = (stateManager.playerTransform.position - shootPosition).normalized;

            transform.LookAt(stateManager.playerTransform.position);
            
            Projectile projectile = projectilePool.GetProjectile();
            projectile.transform.position = shootPosition;
            projectile.ShootToDirection(directionToPlayer, transform);
            
            attackingEvent.Invoke();
            isAttacking = false;
        }

        protected override void ResetAttackAnimation()
        {
        }
    }
}