using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.PlayFab_Systems
{
    public class CloudSavingSystem : MonoBehaviour
    {
        [SerializeField] private KeyCode saveKeyButton = KeyCode.S;

        private PlayerData playerData;
        public Transform playerTransform;
        public string playerName = "Default";
        // public string playerIconID;

        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(saveKeyButton))
            {
                SavePlayerDataToCloud();
                // Debug.
                SavePlayerToLocalInstance();
            }
        }

        public void SavePlayerDataToCloud()
        {
            var x = playerTransform.position.x.ToString();
            var y = playerTransform.position.y.ToString();
            var z = playerTransform.position.z.ToString();

            var data = new Dictionary<string, string>
            {
                { " PlayerName", playerName },
                { " PlayerPosX", x },
                { " PlayerPosY", y },
                { " PlayerPosZ", z }
            };

            var request = new UpdateUserDataRequest
            {
                Data = data
            };

            PlayFabClientAPI.UpdateUserData(request, OnSuccessDataSend, OnFailDataSend);
        }

        private void OnSuccessDataSend(UpdateUserDataResult obj)
        {
            Debug.Log("Successfully saved player data to the cloud.");
        }

        private void OnFailDataSend(PlayFabError obj)
        {
            Debug.LogError($"Failed to save player data, error: {obj.GenerateErrorReport()}");
        }
        
        // Local save for Debug Purposes.
        public void SavePlayerToLocalInstance()
        {
            playerData.playerName = playerName;
            playerData.playerPosition = playerTransform.position;
        }
    }
}