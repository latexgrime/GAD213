using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] protected int damage = 2;
        private Rigidbody projectileRb;

        protected virtual void Awake()
        {
            projectileRb = GetComponent<Rigidbody>();
        }

        public virtual void ShootToDirection(Vector3 direction)
        {
            gameObject.SetActive(true);
            projectileRb.velocity = direction * speed;
        }

        public virtual void Deactivate()
        {
            projectileRb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EntityStats>(out var entityStats))
            {
                OnHitEntity(entityStats);
            }
        }

        protected abstract void OnHitEntity(EntityStats entityStats);
    }
}