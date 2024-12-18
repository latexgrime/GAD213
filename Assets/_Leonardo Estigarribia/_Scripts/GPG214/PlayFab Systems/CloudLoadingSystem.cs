using System;
using _Leonardo_Estigarribia._Scripts.GPG214;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.PlayFab_Systems
{
    public class CloudLoadingSystem : MonoBehaviour
    {
        private PlayerData playerData;
        [SerializeField] private KeyCode loadDataFromCloudKeycode = KeyCode.Keypad3;

        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(loadDataFromCloudKeycode))
            {
                LoadPlayerDataFromCloud();
            }
        }

        public void LoadPlayerDataFromCloud()
        {
            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData(request, OnDataReceivedSuccess, OnDataReceivedFailure);
        }

        private void OnDataReceivedSuccess(GetUserDataResult result)
        {
            if (result.Data == null || result.Data.Count == 0)
            {
                Debug.Log("No player data found in server.");
                return;
            }

            // Set it to the local instance.
            if (result.Data.ContainsKey("PlayerName"))
            {
                playerData.SetPlayerName(result.Data["PlayerName"].Value);
            }

            float x = playerData.GetPlayerPosition().x;
            float y = playerData.GetPlayerPosition().y;
            float z = playerData.GetPlayerPosition().z;

            if (result.Data.ContainsKey("PlayerPosX"))
            {
                float.TryParse(result.Data["PlayerPosX"].Value, out x);
            }
            
            if (result.Data.ContainsKey("PlayerPosY"))
            {
                float.TryParse(result.Data["PlayerPosY"].Value, out y);
            }
            
            if (result.Data.ContainsKey("PlayerPosZ"))
            {
                float.TryParse(result.Data["PlayerPosZ"].Value, out z);
            }
            
            // Set it to the local instance and set the saved position to the actual player position.
            playerData.SetPlayerPosition(new Vector3(x, y, z), true);
            
        }
        
        private void OnDataReceivedFailure(PlayFabError error)
        {
            Debug.LogError($"Life is not fair, failed to load player data, exception: {error.GenerateErrorReport()}");
        }
    }
}
