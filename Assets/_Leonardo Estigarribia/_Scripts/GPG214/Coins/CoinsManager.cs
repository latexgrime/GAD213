using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Coins
{
    public class CoinsManager : MonoBehaviour
    {
        private static CoinsManager instance;

        public static CoinsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CoinsManager>();
                    if (instance == null)
                    {
                        var gameObject = new GameObject("CoinManager");
                        instance = gameObject.AddComponent<CoinsManager>();
                    }
                }

                return instance;
            }
        }

        public event Action<int> OnCoinsChanged;
        public event Action OnDoubleJumpUnlocked;

        private int currentCoins;
        [SerializeField] private int coinsForDoubleJump = 2;
        private bool isDoubleJumpUnlocked;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this) Destroy(gameObject);
        }

        public void AddCoin()
        {
            currentCoins++;
            OnCoinsChanged?.Invoke(currentCoins);

            if (currentCoins >= coinsForDoubleJump && !isDoubleJumpUnlocked)
            {
                isDoubleJumpUnlocked = true;
                OnDoubleJumpUnlocked?.Invoke();
            }
        }

        public bool IsDoubleJumpUnlocked()
        {
            return isDoubleJumpUnlocked;
        }

        public int GetCurrentCoins()
        {
            return currentCoins;
        }
    }
}