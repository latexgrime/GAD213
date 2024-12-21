using _Leonardo_Estigarribia._Scripts.GPG214.Google_Drive;
using _Leonardo_Estigarribia._Scripts.GPG214.Interfaces;
using _Leonardo_Estigarribia._Scripts.GPG214.Local_File_Management;
using _Leonardo_Estigarribia._Scripts.GPG214.PlayFab_Systems;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class GeneralSavingLoadingSystem : MonoBehaviour
    {
        [SerializeField] private KeyCode localSaveKeyCode = KeyCode.F1;
        [SerializeField] private KeyCode localLoadingKeyCode = KeyCode.F3;

        [SerializeField] private KeyCode cloudSaveKeyCode = KeyCode.F5;
        [SerializeField] private KeyCode cloudLoadKeyCode = KeyCode.F7;

        [SerializeField] private KeyCode googleDriveSaveKeyCode = KeyCode.Keypad1;
        [SerializeField] private KeyCode googleDriveLoadKeyCode = KeyCode.Keypad3;

        [SerializeField] private string localSaveFileName = "PlayerSave.xml";

        private PlayerData playerData;
        private ISavingLoadingData localDataManaging;
        private ISavingLoadingData playFabDataManaging;
        private ISavingLoadingData googleDriveDataManaging;

        #region General Script Logic

        // Region start -------------------------------------------------------------------------------
        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
            localDataManaging = new LocalDataManaging(localSaveFileName);
            playFabDataManaging = new PlayFabDataManaging();
            googleDriveDataManaging = GetComponent<GoogleDriveDataManaging>();
        }

        private void Update()
        {
            InputUpdate();
        }

        private void InputUpdate()
        {
            if (Input.GetKeyDown(localSaveKeyCode)) SaveDataLocally();

            if (Input.GetKeyDown(localLoadingKeyCode)) LoadDataLocally();

            if (Input.GetKeyDown(cloudSaveKeyCode)) SaveDataToCloud();

            if (Input.GetKeyDown(cloudLoadKeyCode)) LoadDataFromCloud();

            if (Input.GetKeyDown(googleDriveSaveKeyCode)) SavePlayerIconToGoogleDrive();

            if (Input.GetKeyDown(googleDriveLoadKeyCode)) LoadPlayerIconFromGoogleDrive();
        }

        private PlayerSaveData CreateSaveData()
        {
            Texture2D iconTexture = null;
            if (playerData.GetPlayerIcon() != null) iconTexture = playerData.GetPlayerIcon().sprite.texture;
            playerData.UpdateStoredPosition();
            return new PlayerSaveData(
                playerData.GetStoredPlayerPosition(),
                playerData.GetPlayerName(),
                iconTexture,
                playerData.GetCurrentPlayerHealth(),
                playerData.GetCurrentPlayerMaxHealth()
            );
        }

        private void ApplyLoadedData(PlayerSaveData loadedData)
        {
            playerData.SetPlayerPosition(loadedData.Position, true);
            playerData.SetPlayerName(loadedData.PlayerName);
            playerData.SetCurrentPlayerHealth(loadedData.CurrentHealth);
            playerData.SetCurrentPlayerMaxHealth(loadedData.MaxHealth);

            var texture = new Texture2D(2, 2);
            if (texture.LoadImage(loadedData.IconData))
            {
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector3(0.5f, 0.5f)
                );
                playerData.SetPlayerIcon(sprite);
            }
        }

        // Region ends -------------------------------------------------------------------------------

        #endregion

        #region Local Data Managing Methods

        // Region start -------------------------------------------------------------------------------
        private void SaveDataLocally()
        {
            playerData.UpdateStoredPosition();
            var saveData = CreateSaveData();
            localDataManaging.SaveData(saveData);
        }

        private void LoadDataLocally()
        {
            var loadedData = localDataManaging.LoadData();
            if (loadedData != null) ApplyLoadedData(loadedData);
        }

        // Region ends -------------------------------------------------------------------------------

        #endregion

        #region PlayFab Managing Methods

        // Region start -------------------------------------------------------------------------------

        private void SaveDataToCloud()
        {
            playerData.UpdateStoredPosition();
            var saveData = CreateSaveData();
            playFabDataManaging.SaveData(saveData);
        }

        private async void LoadDataFromCloud()
        {
            var loadedData = await playFabDataManaging.LoadDataAsync();
            if (loadedData != null) ApplyLoadedData(loadedData);
        }
        // Region ends -------------------------------------------------------------------------------

        #endregion

        #region Google Drive Managing Methods

        private void SavePlayerIconToGoogleDrive()
        {
            var saveData = CreateSaveData();
            googleDriveDataManaging.SaveData(saveData);
        }

        private async void LoadPlayerIconFromGoogleDrive(string iconID = null)
        {
            PlayerSaveData loadedData;

            if (iconID != null)
            {
                var googleDriveManager = (GoogleDriveDataManaging)googleDriveDataManaging;
                loadedData = await googleDriveManager.LoadSpecificIcon(iconID);
            }
            else
            {
                loadedData = await googleDriveDataManaging.LoadDataAsync();
            }

            if (loadedData != null && loadedData.IconData != null)
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(loadedData.IconData);
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                playerData.SetPlayerIcon(sprite);
                Debug.Log("Player Icon downloaded from google drive set into scene.");
            }
        }

        /*public List<PlayerIconInfo> GetSavedIcons()
        {
            var googleDriveManager = (GoogleDriveDataManaging)googleDriveDataManaging;
            return googleDriveManager.GetSavedIcons();
        }*/

        public void ApplyIconData(byte[] iconData)
        {
            if (iconData != null)
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(iconData);

                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

                playerData.SetPlayerIcon(sprite);
                Debug.Log("Applied new icon correctly.");
            }
        }

        /*public void LoadSavedIcon(PlayerIconInfo iconInfo)
        {
            if (iconInfo != null)
            {
                LoadPlayerIconFromGoogleDrive(iconInfo.IconId);
            }
        }*/

        #endregion
    }
}