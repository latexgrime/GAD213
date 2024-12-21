using System;
using _Leonardo_Estigarribia._Scripts.GPG214.Coins;
using TMPro;
using UnityEngine;

public class CoinDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentCoinsText;
    private CoinsManager coinsManager;

    private void Start()
    {
        coinsManager = CoinsManager.Instance;
        coinsManager.OnCoinsChanged += UpdateCoinsDisplay;
        UpdateCoinsDisplay(coinsManager.GetCurrentCoins());
    }

    private void UpdateCoinsDisplay(int coinsAmount)
    {
        currentCoinsText.text = $"Coins: {coinsAmount}";
    }
}