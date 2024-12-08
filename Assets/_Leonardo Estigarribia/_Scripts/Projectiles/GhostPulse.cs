using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class GhostPulse : Projectile
    {
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private ParticleSystem explosionEffect;

        protected override void OnHitEntity(EntityStats entityStats)
        {
            var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            
            foreach (var hitCollider in hitColliders)
                if (hitCollider.TryGetComponent<EntityStats>(out var hitStats))
                    hitStats.TakeDamage(damage);
            
            explosionEffect.Play();
            Deactivate();
        }
    }
}