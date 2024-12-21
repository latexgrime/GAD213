using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Coins.Pooling
{
    public class CoinPoolManager : MonoBehaviour
    {
        private static CoinPoolManager _instance;

        public static CoinPoolManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CoinPoolManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("Coin Pool Manager");
                        _instance = go.AddComponent<CoinPoolManager>();
                    }
                }

                return _instance;
            }
        }

        [Header("- Pool settings")] [SerializeField]
        private GameObject coinPrefab;

        [SerializeField] private int initialPoolSize = 25;
        [SerializeField] private bool isPoolExpansionNeeded;

        private readonly Queue<GameObject> coinPool = new();
        private readonly List<GameObject> activeCoins = new();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializePool();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializePool()
        {
            Profiler.BeginSample("GPG214: Coin pool initialization.");
            for (var i = 0; i < initialPoolSize; i++) CreateNewCoin();
            Profiler.EndSample();
            Debug.Log($"Initialized pool with {initialPoolSize} coins");
        }

        private void CreateNewCoin()
        {
            var coin = Instantiate(coinPrefab);
            coin.transform.parent = transform;
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }

        public GameObject GetCoin(Vector3 position)
        {
            Profiler.BeginSample("GPG214: Get coin from pool.");
            if (coinPool.Count == 0 && isPoolExpansionNeeded)
            {
                Debug.LogWarning("Coin pool empty, creating new coins.");
                CreateNewCoin();
            }

            if (coinPool.Count > 0)
            {
                var coin = coinPool.Dequeue();
                coin.transform.position = position;
                coin.transform.rotation = Quaternion.identity;
                coin.SetActive(true);
                activeCoins.Add(coin);

                return coin;
            }

            Debug.LogWarning("No coins available in pool.");
            Profiler.EndSample();
            return null;
        }

        public void ReturnCoin(GameObject coin)
        {
            Profiler.BeginSample("GPG214: Return coin to pool operation.");
            if (activeCoins.Contains(coin))
            {
                coin.SetActive(false);
                coinPool.Enqueue(coin);
                activeCoins.Remove(coin);
            }

            Profiler.EndSample();
        }

        public void ReturnAllCoins()
        {
            Profiler.BeginSample("GPG214: Return all coins operation.");
            while (activeCoins.Count > 0) ReturnCoin(activeCoins[0]);
            Profiler.EndSample();
        }
    }
}