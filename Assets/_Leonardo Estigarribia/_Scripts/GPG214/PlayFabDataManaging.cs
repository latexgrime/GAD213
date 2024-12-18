using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityGoogleDrive;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class PlayFabDataManaging : ISavingLoadingData
    {
        
        public void SaveData(PlayerSaveData data)
        {
            var cloudDataToSave = new Dictionary<string,string>
            {
                { " PlayerName", data.PlayerName },
                { " PlayerPosX", data.Position.x.ToString() },
                { " PlayerPosY", data.Position.y.ToString() },
                { " PlayerPosZ", data.Position.z.ToString() },
                { "CurrentHealth", data.CurrentHealth.ToString() },
                { "MaxHealth", data.MaxHealth.ToString()}
            };

            var request = new UpdateUserDataRequest { Data = cloudDataToSave };
            PlayFabClientAPI.UpdateUserData(request, OnSavingDataSuccess, OnSavingDataFailure);
        }
        
        private void OnSavingDataSuccess(UpdateUserDataResult obj)
        {
            Debug.Log("Successfully saved player data to the cloud.");
        }
        
        private void OnSavingDataFailure(PlayFabError obj)
        {
            Debug.LogError($"Failed to save player data, error: {obj.GenerateErrorReport()}");
        }
        
        public PlayerSaveData LoadData()
        {
            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData(request, result =>
            {
                if (result.Data == null)
                {
                    Debug.LogError($"No data found in PlayFab.");
                    return;
                }
                
                var data = new PlayerSaveData
                {
                    PlayerName = result.Data.ContainsKey("PlayerName").ToString(),
                    Position = new Vector3(
                        float.Parse(result.Data["PlayerPosX"].Value),
                        result.Data["PlayerPosY"].Value,
                        result.Data["PlayerPosZ"].Value,
                        ),
                    CurrentHealth = int.
                    
                }
            });
        }
        
    }
}
