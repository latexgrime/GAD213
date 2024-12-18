using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    [Serializable]
    public class PlayerSaveData
    {
        public Vector3 Position { get; set; }
        public string PlayerName { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public byte[] IconData { get; set; }

        public PlayerSaveData() { }
        
        public PlayerSaveData(Vector3 position, string name, Texture2D icon, int currentHealth, int maxHealth)
        {
            Position = position;
            PlayerName = name;
            IconData = icon.EncodeToPNG();
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }
    }
}