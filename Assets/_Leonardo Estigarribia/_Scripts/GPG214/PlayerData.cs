using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    /// <summary>
    /// This class contains general data of the player. The purpose of the script is to serve as a bridge between classes that might need access to specific data.
    /// </summary>
    public class PlayerData : MonoBehaviour
    {
        private GameObject playerGameObject;
        
        [Header("- General player information")]
        [SerializeField] private string playerName;
        [SerializeField] private Vector3 storedPlayerPosition;
        [SerializeField] private Image playerIcon;
        
        [Header("- Player in game stats")]
        public EntityStats playerEntityStats;

        private void Start()
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerEntityStats = playerGameObject.GetComponent<EntityStats>();
            UpdateStoredPosition();
        }

        /// <summary>
        /// Updates the stored position of the player.
        /// </summary>
        public void UpdateStoredPosition()
        {
            storedPlayerPosition = playerGameObject.transform.position;
        }

        public Vector3 GetStoredPlayerPosition()
        {
            return storedPlayerPosition;
        }

        /// <summary>
        /// Sets the player position data via this method.
        /// </summary>
        /// <param name="positionToSet">The position data to be set in this class.</param>
        /// <param name="setInWorld">When called this function, if this bool is true, the player will be teleported to the positionToSet vector, otherwise it won't.</param>
        public void SetPlayerPosition(Vector3 positionToSet, bool setInWorld)
        {
            storedPlayerPosition = positionToSet;
            
            if (setInWorld)
            {
                SetPlayerPositionInWorld(positionToSet.x, positionToSet.y, positionToSet.z);
            }
        } 

        public string GetPlayerName()
        {
            return playerName;
        }

        public void SetPlayerName(string nameToSet)
        {
            playerName = nameToSet;
        }

        public Image GetPlayerIcon()
        {
            return playerIcon;
        }

        public void SetPlayerIcon(Image iconToSet)
        {
            playerIcon = iconToSet;
        }

        public void SetPlayerIcon(Sprite iconToSet)
        {
            playerIcon.sprite = iconToSet;
        }

        private void SetPlayerPositionInWorld(float x, float y, float z)
        {
            if (playerGameObject != null)
            {
                Rigidbody rb = playerGameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.position = new Vector3(x, y, z);
                    playerGameObject.transform.position = new Vector3(x, y, z);
                }
                else
                {
                    playerGameObject.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}