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
            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform);
                projectiles[i] = projectiles[i].GetComponent<Projectile>();
                projectile.SetActive(false);
            }
        }

        public Projectile GetProjectile()
        {
            Projectile projectile = projectiles[currentIndex];
            currentIndex = (currentIndex + 1);
            return projectile;
        }
        
        
    }
}