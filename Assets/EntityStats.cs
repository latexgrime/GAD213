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
    [Header("- Health points")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    
    [Header("- Particle effects")]
    [SerializeField] private ParticleSystem[] damageParticlesPool;
    [SerializeField] private ParticleSystem[] healParticles;

    private void Start()
    {
        // Make the player have full health at the beginning of the game.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Play some particle effects.
        int randomParticleIndex = Random.Range(0, damageParticlesPool.Length);
        damageParticlesPool[randomParticleIndex].Play();
        
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
