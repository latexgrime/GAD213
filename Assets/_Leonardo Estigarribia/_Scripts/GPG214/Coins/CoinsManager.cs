using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Coins
{
    public class CoinsManager : MonoBehaviour
    {
        private static CoinsManager _instance;

        public static CoinsManager CoinsManagerInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CoinsManager>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject("CoinManager");
                        _instance = gameObject.AddComponent<CoinsManager>();
                    }
                }

                return _instance;
            }
        }

        public event Action<int> OnCoinsChanged;
        public event Action OnDoubleJumpUnlocked;

        private int currentCoins;
        [SerializeField] private int coinsForDoubleJump = 2;
        private bool isDoubleJumpUnlocked;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this) Destroy(gameObject);
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

        public void SetCoins(int amount)
        {
            currentCoins = amount;
            OnCoinsChanged?.Invoke(currentCoins);

            if (currentCoins >= coinsForDoubleJump && !isDoubleJumpUnlocked)
            {
                isDoubleJumpUnlocked = true;
                OnDoubleJumpUnlocked?.Invoke();
            }
        }
    }
}