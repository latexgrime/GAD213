using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int poolSize = 5;
        private Projectile[] projectiles;
        private int currentIndex = 0;

        private void Start()
        {
            projectiles = new Projectile[poolSize];
            for (int i = 0; i < UPPER; i++)
            {
                
            }
        }
        
        
    }
}