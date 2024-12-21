using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    [Serializable]
    public class PlayerSaveData
    {
        // General.
        public Vector3 Position { get; set; }
        public string PlayerName { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public byte[] IconData { get; set; }

        // Specific.
        public int CollectedCoins { get; set; }
        public bool IsDoubleJumpUnlocked { get; set; }
        
        public PlayerSaveData() { }
        
        public PlayerSaveData(Vector3 position, string name, Texture2D icon, int currentHealth, int maxHealth, int collectedCoins, bool isDoubleJumpUnlocked)
        {
            Position = position;
            PlayerName = name;
            IconData = icon.EncodeToPNG();
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            CollectedCoins = collectedCoins;
            IsDoubleJumpUnlocked = isDoubleJumpUnlocked;
        }
    }
}