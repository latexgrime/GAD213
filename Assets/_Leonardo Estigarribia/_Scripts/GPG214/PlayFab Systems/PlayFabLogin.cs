using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.PlayFab_Systems
{
    public class PlayFabLogin : MonoBehaviour
    {
        public string PlayFabTitleId = "DefaultID";

        private void Start()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
        }
        
        void OnLoginSuccess(LoginResult result)
        {
            Debug.Log($"PlayFab login successful! PlayFabId: {result.PlayFabId}");
        }

        void OnLoginError(PlayFabError obj)
        {
            Debug.LogError($"PlayFab login failed: {obj.GenerateErrorReport()}");
        }
        
    }


}