using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class GeneralSavingLoadingSystem : MonoBehaviour
    {
        [SerializeField] private KeyCode localSaveKeyCode = KeyCode.F1;
        [SerializeField] private KeyCode localLoadingKeyCode = KeyCode.F3;
        [SerializeField] private KeyCode cloudSaveKeyCode = KeyCode.F5;
        [SerializeField] private KeyCode cloudLoadKeyCode = KeyCode.F7;

        [SerializeField] private string localSaveFileName = "PlayerSave.xml";

        private PlayerData playerData;
        private ISavingLoadingData localDataManaging;
        private ISavingLoadingData playFabDataManaging;

        
        #region General Script Logic
        // Region start -------------------------------------------------------------------------------
        private void Start()
        {
            playerData = FindObjectOfType<PlayerData>();
            localDataManaging = new LocalDataManaging(localSaveFileName);
            playFabDataManaging = new PlayFabDataManaging();
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
            ApplyLoadedData(loadedData);
        }
        // Region ends -------------------------------------------------------------------------------
        #endregion

        #region Cloud Data Managing Methods
        // Region start -------------------------------------------------------------------------------

        private void SaveDataToCloud()
        {
            playerData.UpdateStoredPosition();
            var saveData = CreateSaveData();
            playFabDataManaging.SaveData(saveData);
        }

        private void LoadDataFromCloud()
        {
            var loadedData = playFabDataManaging.LoadData();
            ApplyLoadedData(loadedData);
        }
        // Region ends -------------------------------------------------------------------------------

        #endregion

    }
}