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
            PlayerSaveData loadedData = null;
            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData(request,
                result =>
                {
                    if (result.Data == null)
                    {
                        Debug.LogError($"No data found in PlayFab.");
                        return;
                    }

                    loadedData = new PlayerSaveData
                    {
                        PlayerName = result.Data.ContainsKey("PlayerName").ToString(),
                        Position = new Vector3(
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosX").Value),
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosY").Value),
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosZ").Value)
                        ),
                        CurrentHealth = int.Parse(result.Data.GetValueOrDefault("CurrentHealth").Value),
                        MaxHealth = int.Parse(result.Data.GetValueOrDefault("MaxHealth").Value)
                    };
                },
                error =>
                {
                    Debug.LogError($"Data could not be loaded form PlayFab: {error.ErrorMessage}");
                }
                
            );
            return loadedData;
        }
        
    }
}
