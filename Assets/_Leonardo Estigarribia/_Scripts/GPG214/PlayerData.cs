using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class PlayerData : MonoBehaviour
    {
        public GameObject player;
        public EntityStats playerEntityStats;
        
        public Image playerIcon;
        public Vector3 playerPosition;
        public string playerName;
        public string playerId;

        private void Start()
        {
            playerEntityStats = player.GetComponent<EntityStats>();
        }

        private void Update()
        {
            playerPosition = player.transform.position;
        }

        public void SetPlayerPosition(float x, float y, float z)
        {
            player.transform.position = new Vector3(x, y, z);
        }
    }
}