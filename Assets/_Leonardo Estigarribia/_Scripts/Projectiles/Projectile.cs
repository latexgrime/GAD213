using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] protected int damage = 2;
        private Rigidbody rigidbody;

        protected virtual void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Shoot(Vector3 direction)
        {
            gameObject.SetActive(true);
            rigidbody.velocity = direction * speed;
        }

        public virtual void Deactivate()
        {
            rigidbody.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EntityStats>(out var entityStats))
            {
                OnHitEntity(entityStats);
            }
            Deactivate();
        }

        protected abstract void OnHitEntity(EntityStats entityStats);
    }
}