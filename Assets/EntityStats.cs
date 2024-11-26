using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.AI;

public class EntityStats : MonoBehaviour
{
    [Header("- Health points")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    private void Start()
    {
        // Make the player have full health at the beginning of the game.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CheckIfDead();
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
