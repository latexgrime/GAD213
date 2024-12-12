using System;
using System.Collections;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] protected int damage = 2;
        private Rigidbody projectileRb;
        private Transform shooter;
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            projectileRb = GetComponent<Rigidbody>();
        }

        public virtual void ShootToDirection(Vector3 direction, Transform shooterTransform)
        {
            // Store who shot this projectile.
            shooter = shooterTransform;  
            gameObject.SetActive(true);
            projectileRb.velocity = direction * speed;
            transform.LookAt(direction);
        }

        public virtual void Deactivate()
        {
            projectileRb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        protected void OnTriggerEnter(Collider other)
        {
            // If hitting the shooter, ignore the collision.

            if (shooter == null || other == null || other.transform.root == shooter.root) return;
            
            if (other.TryGetComponent<EntityStats>(out var entityStats))
            {
                OnHitEntity(entityStats);
            }
            
            OnGeneralImpact();
            
        }

        protected abstract void OnHitEntity(EntityStats entityStats);
        protected abstract void OnGeneralImpact();

        protected abstract void PlayVisualEffectsOnPosition(Transform position, GameObject particleEffects);

        protected abstract IEnumerator DestroyVisualEffectAfterTime(float destructionTime);
    }
}