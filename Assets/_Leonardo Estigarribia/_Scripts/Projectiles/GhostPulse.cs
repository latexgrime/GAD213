using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class GhostPulse : Projectile
    {
        [Header("- Stats.")]
        [SerializeField] private float explosionRadius = 3f;
        
        [Header("- Visual effects.")]
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private float explosionDestructionAfterTime = 2f; 
        private GameObject _particleEffect;

        [Header("- Sound effects.")] [SerializeField]
        private AudioClip explosionSoundEffect;
        private AudioSource audioSource;
        
        protected override void OnHitEntity(EntityStats entityStats)
        {
        }

        protected override void OnGeneralImpact()
        {
            PlayVisualEffectsOnPosition(transform, explosionEffect);
            
            var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
                if (hitCollider.TryGetComponent<EntityStats>(out var hitStats))
                    hitStats.TakeDamage(damage);
            

            Deactivate();        
        }

        protected override void PlayVisualEffectsOnPosition(Transform position, GameObject particleEffects)
        {
            _particleEffect = Instantiate(explosionEffect, transform.position, quaternion.identity);
            
            audioSource = _particleEffect.GetComponent<AudioSource>();
            audioSource.PlayOneShot(explosionSoundEffect);
            
            StartCoroutine(DestroyVisualEffectAfterTime(explosionDestructionAfterTime));
        }

        protected override IEnumerator DestroyVisualEffectAfterTime(float destructionTime)
        {
            yield return new WaitForSeconds(destructionTime);
            Destroy(_particleEffect);
        }
    }
}