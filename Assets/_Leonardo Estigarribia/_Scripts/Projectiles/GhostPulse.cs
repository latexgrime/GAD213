using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class GhostPulse : Projectile
    {
        [SerializeField] private ParticleSystem hitEffect;
        protected override void OnHitEntity(EntityStats entityStats)
        {
            if (entityStats.gameObject.CompareTag("Player"))
            {
                entityStats.TakeDamage(damage);
                hitEffect.Play();
            }
        }
    }
}