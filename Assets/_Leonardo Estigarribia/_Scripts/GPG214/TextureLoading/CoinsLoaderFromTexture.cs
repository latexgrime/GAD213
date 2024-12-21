using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace _Leonardo_Estigarribia._Scripts.GPG214.TextureLoading
{
    public class CoinsLoaderFromTexture : MonoBehaviour
    {
        private readonly List<Vector3> spawnPoints = new();
        
        [Header("- Spawn Settings")] 
        [SerializeField] private GameObject coinPrefab;

        /// <summary>
        /// Texture from which the coins (or selected prefab) is going to be instantiated from. This NEEDS to be black and white.
        /// </summary>
        [SerializeField] private Texture2D spawnMap;
        
        /// <summary>
        /// The distance in-game between each pixel/point.
        /// </summary>
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private float heightSpawningOffset = 1f;

        [Header("- Sampling Settings")] 
        [SerializeField] [Range(0f, 1f)] private float whiteThresholdToSpawn = 0.5f;

        [Header("- Debugging")]
        [SerializeField] private bool visualizeSpawnPoints;


        private void Start()
        {
            if (spawnMap != null)
            {
                GenerateSpawnPoints();
                SpawnCoins();
            }
            else
            {
                Debug.LogWarning("GPG214: CoinsLoaderFromTexture.cs: Spawn map texture not set.");
            }
        }

        private void GenerateSpawnPoints()
        {
            Profiler.BeginSample("GPG214: Coin spawn point generation from image.");
            spawnPoints.Clear();

            // Calculate world space bounds based on texture size.
            var worldWidth = spawnMap.width * gridSize;
            var worldLength = spawnMap.height * gridSize;

            // Center offset so spawns are centered on the object.
            var centerOffset = new Vector3(-worldWidth / 2f, 0, -worldLength / 2f);

            for (var x = 0; x < spawnMap.width; x++)
            for (var y = 0; y < spawnMap.height; y++)
            {
                var pixel = spawnMap.GetPixel(x, y);
                var brightness = (pixel.r + pixel.g + pixel.b) / 3f;

                if (brightness >= whiteThresholdToSpawn)
                {
                    var worldPos = new Vector3(x * gridSize, heightSpawningOffset, y * gridSize) + centerOffset +
                                   transform.position;

                    // Check if there's ground below
                    if (Physics.Raycast(worldPos + Vector3.up * 5f, Vector3.down, out var hit, 10f))
                    {
                        worldPos.y = hit.point.y + heightSpawningOffset;
                        spawnPoints.Add(worldPos);
                    }
                }
            }
            Profiler.EndSample();
        }

        private void SpawnCoins()
        {
            Profiler.BeginSample("GPG214: Coins instantiating.");
            foreach (var point in spawnPoints)
            {
                Instantiate(coinPrefab, point, Quaternion.identity);
            }
            Profiler.EndSample();
        }

        private void OnDrawGizmos()
        {
            if (visualizeSpawnPoints && spawnPoints != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var point in spawnPoints) Gizmos.DrawWireSphere(point, 0.25f);
            }
        }
    }
}