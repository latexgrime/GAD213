using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class GhostPulse : Projectile
    {
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private ParticleSystem explosionEffect;
        protected override void OnHitEntity(EntityStats entityStats)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<EntityStats>(out var hitStats))
                {
                    if (entityStats.gameObject.CompareTag("Player"))
                        hitStats.TakeDamage(damage);
                }
            }
            explosionEffect.Play();
        }
    }
}