using System;
using System.Collections.Generic;
using _Leonardo_Estigarribia._Scripts.GPG214;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.PlayFab_Systems
{
    public class CloudSavingSystem : MonoBehaviour
    {
        [SerializeField] private KeyCode saveKeyButton = KeyCode.Keypad0;

        private PlayerData playerData;
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
            }
        }

        public void SavePlayerDataToCloud()
        {
            var x = playerData.GetStoredPlayerPosition().x.ToString();
            var y = playerData.GetStoredPlayerPosition().y.ToString();
            var z = playerData.GetStoredPlayerPosition().z.ToString();

            var data = new Dictionary<string, string>
            {
                { " PlayerName", playerData.GetPlayerName() },
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
    }
}