using UnityEngine;
using UnityEngine.UI;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    /// <summary>
    ///     This class contains general data of the player. The purpose of the script is to serve as a bridge between classes
    ///     that might need access to specific data.
    /// </summary>
    public class PlayerData : MonoBehaviour
    {
        private GameObject playerGameObject;

        [Header("- General player information")] [SerializeField]
        private string playerName;

        [SerializeField] private Vector3 storedPlayerPosition;
        [SerializeField] private Image playerIcon;

        [Header("- Player in game stats")] public EntityStats playerEntityStats;

        private void Start()
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            playerEntityStats = playerGameObject.GetComponent<EntityStats>();
            UpdateStoredPosition();
        }

        #region Managing Position Data

        public Vector3 GetStoredPlayerPosition()
        {
            return storedPlayerPosition;
        }

        public void UpdateStoredPosition()
        {
            storedPlayerPosition = playerGameObject.transform.position;
        }

        /// <summary>
        ///     Sets the player position data via this method.
        /// </summary>
        /// <param name="positionToSet">The position data to be set in this class.</param>
        /// <param name="setInWorld">
        ///     When called this function, if this bool is true, the player will be teleported to the
        ///     positionToSet vector, otherwise it won't.
        /// </param>
        public void SetPlayerPosition(Vector3 positionToSet, bool setInWorld)
        {
            storedPlayerPosition = positionToSet;

            if (setInWorld) SetPlayerPositionInWorld(positionToSet.x, positionToSet.y, positionToSet.z);
        }

        private void SetPlayerPositionInWorld(float x, float y, float z)
        {
            if (playerGameObject != null)
            {
                var rb = playerGameObject.GetComponent<Rigidbody>();
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

        #endregion

        #region Managing Name Data

        public string GetPlayerName()
        {
            return playerName;
        }

        public void SetPlayerName(string nameToSet)
        {
            playerName = nameToSet;
        }

        #endregion

        #region Managing Icon Data

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

        #endregion

        #region Managing Player Health Data

        public int GetCurrentPlayerHealth()
        {
            return playerEntityStats.GetCurrentHealth();
        }

        public int GetCurrentPlayerMaxHealth()
        {
            return playerEntityStats.GetMaxHealth();
        }

        public void SetCurrentPlayerHealth(int value)
        {
            playerEntityStats.SetCurrentHealth(value);
        }

        public void SetCurrentPlayerMaxHealth(int value)
        {
            playerEntityStats.SetMaxHealth(value);
        }

        #endregion
    }
}