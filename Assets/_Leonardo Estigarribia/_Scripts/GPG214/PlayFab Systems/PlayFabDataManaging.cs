using System.Collections.Generic;
using System.Threading.Tasks;
using _Leonardo_Estigarribia._Scripts.GPG214.Interfaces;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.PlayFab_Systems
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
                { "MaxHealth", data.MaxHealth.ToString()},
                { "CollectedCoins", data.CollectedCoins.ToString()},
                { "IsDoubleJumpUnlocked",data.IsDoubleJumpUnlocked.ToString()}
            };

            var request = new UpdateUserDataRequest { Data = cloudDataToSave };
            PlayFabClientAPI.UpdateUserData(request, OnSavingDataSuccess, OnSavingDataFailure);
        }

        public PlayerSaveData LoadData()
        {
            return null;
        }

        private void OnSavingDataSuccess(UpdateUserDataResult obj)
        {
            Debug.Log("Successfully saved player data to the cloud.");
        }
        
        private void OnSavingDataFailure(PlayFabError obj)
        {
            Debug.LogError($"Failed to save player data, error: {obj.GenerateErrorReport()}");
        }
        
        public async Task<PlayerSaveData> LoadDataAsync()
        {
            var taskCompletitionSource = new TaskCompletionSource<PlayerSaveData>();

            var request = new GetUserDataRequest();
            PlayFabClientAPI.GetUserData(request,
                result =>
                {
                    if (result.Data == null)
                    {
                        Debug.LogError($"No data found in PlayFab.");
                        taskCompletitionSource.SetResult(null);
                        return;
                    }

                    // Getting the data here.
                    var loadedData = new PlayerSaveData
                    {
                        PlayerName = result.Data["PlayerName"].Value.ToString(),
                        Position = new Vector3(
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosX").Value),
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosY").Value),
                            float.Parse(result.Data.GetValueOrDefault("PlayerPosZ").Value)
                        ),
                        CurrentHealth = int.Parse(result.Data.GetValueOrDefault("CurrentHealth").Value),
                        MaxHealth = int.Parse(result.Data.GetValueOrDefault("MaxHealth").Value),
                        CollectedCoins = int.Parse(result.Data.GetValueOrDefault("CollectedCoins").Value),
                        IsDoubleJumpUnlocked = bool.Parse(result.Data["IsDoubleJumpUnlocked"].Value)
                        
                    };
                    
                    taskCompletitionSource.SetResult(loadedData);
                },
                error =>
                {
                    Debug.LogError($"Data could not be loaded form PlayFab: {error.ErrorMessage}");
                    taskCompletitionSource.SetResult(null);
                }
            );
            
            return await taskCompletitionSource.Task;
        }
        
    }
}
