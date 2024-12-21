using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.Projectiles
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int poolSize = 5;
        private Projectile[] projectiles;
        private int currentIndex;

        private void Start()
        {
            projectiles = new Projectile[poolSize];
            for (var i = 0; i < poolSize; i++)
            {
                var projectile = Instantiate(projectilePrefab, transform);
                projectiles[i] = projectile.GetComponent<Projectile>();
                projectile.SetActive(false);
            }
        }

        public Projectile GetProjectile()
        {
            var projectile = projectiles[currentIndex];
            currentIndex = (currentIndex + 1) % poolSize;
            return projectile;
        }
    }
}