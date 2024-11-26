using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EntityStats : MonoBehaviour
{
    private AudioSource audioSource;
    
    [Header("- Health points")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    
    [Header("- Particle VFX")]
    [SerializeField] private ParticleSystem[] takingDamageVFX;
    [SerializeField] private ParticleSystem[] healVFX;

    [Header("- SFX")] 
    [SerializeField] private AudioClip[] takingDamageSFX;
    [SerializeField] private AudioClip[] healSFX;

    private void Start()
    {
        // Make the player have full health at the beginning of the game.
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Play some particle effects.
        int randomVFXIndex = Random.Range(0, takingDamageVFX.Length);
        takingDamageVFX[randomVFXIndex].Play();
        
        //Play some SFX.
        int randomSFXIndex = Random.Range(0, takingDamageSFX.Length);
        audioSource.PlayOneShot(takingDamageSFX[randomSFXIndex]);
        
        // Avoid negative numbers.
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CheckIfDead();
        }
    }

    public void HealEntity(int healAmount)
    {
        currentHealth += healAmount;
        
        // Avoid overhealing.
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void CheckIfDead()
    {
        // If the current health is the same or less than zero, this entity is dead.
        if (currentHealth <= 0)
        {
            // Play dead animation from animation manager or something idk.
            Debug.Log($"{gameObject.transform.name} died.");
        }
    }
}
