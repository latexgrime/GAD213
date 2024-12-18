using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Local_File_Management
{
    /// <summary>
    /// This class is in charge of containing the saved data of the player.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public string playerName;
        public byte[] playerIconBytes;

        public PlayerSaveData(Vector3 position, string name, Texture2D icon)
        {
            positionX = position.x;
            positionY = position.y;
            positionZ = position.z;
            playerName = name;
            playerIconBytes = icon.EncodeToPNG();
        }
        
        public PlayerSaveData() { }
    }
}