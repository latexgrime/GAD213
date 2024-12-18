/*using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Local_File_Management
{
    public class SaveLoadLocally : MonoBehaviour
    {
        private PlayerData playerData;
        private XmlSerializer serializer;

        [SerializeField] private KeyCode localSaveKeyCode = KeyCode.F1;
        [SerializeField] private KeyCode localLoadKeyCode = KeyCode.F3;

        [SerializeField] private string saveFileName = "PlayerSave.xml";
        [SerializeField] private string savePath;


        private void Start()
        {
            InitializeScript();
        }

        private void InitializeScript()
        {
            playerData = FindObjectOfType<PlayerData>();
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            serializer = new XmlSerializer(typeof(PlayerSaveData));

            Debug.Log($"Save file will be located at: {savePath}");
        }

        private void Update()
        {
            if (Input.GetKeyDown(localSaveKeyCode)) SaveLocalData();

            if (Input.GetKeyDown(localLoadKeyCode)) LoadLocalData();
        }

        private void LoadLocalData()
        {
            if (!File.Exists(savePath))
            {
                Debug.LogError($"No save file found at {savePath}");
                return;
            }

            PlayerSaveData loadedData;
            using (var stream = new FileStream(savePath, FileMode.Open))
            {
                loadedData = (PlayerSaveData)serializer.Deserialize(stream);
            }

            var loadedPosition = new Vector3(loadedData.positionX, loadedData.positionY, loadedData.positionZ);
            playerData.SetPlayerPosition(loadedPosition, true);

            playerData.SetPlayerName(loadedData.playerName);

            playerData.SetCurrentPlayerHealth(loadedData.playerCurrentHealth);

            playerData.SetCurrentPlayerMaxHealth(loadedData.playerMaxHealth);

            if (loadedData.playerIconBytes != null)
            {
                var loadedTexture = new Texture2D(2, 2);
                if (loadedTexture.LoadImage(loadedData.playerIconBytes))
                {
                    var newSprite = Sprite.Create(
                        loadedTexture,
                        new Rect(0, 0, loadedTexture.width, loadedTexture.height),
                        // Setting the pivot in the center for now.
                        new Vector2(0.5f, 0.5f)
                    );
                    playerData.SetPlayerIcon(newSprite);
                    ;
                }
            }

            Debug.Log($"Success in loading player data from: {savePath}.");
        }

        private void SaveLocalData()
        {
            var directory = Path.GetDirectoryName(savePath);

            // Check if the directory exits, otherwise create it.
            if (!Directory.Exists(directory))
                if (directory != null)
                    Directory.CreateDirectory(directory);

            playerData.UpdateStoredPosition();

            Texture2D playerIconTexture = null;
            // Check if the player has an Icon set.
            if (playerData.GetPlayerIcon() != null) playerIconTexture = playerData.GetPlayerIcon().sprite.texture;

            var dataToSave = new PlayerSaveData(playerData.GetStoredPlayerPosition(), playerData.GetPlayerName(),
                playerIconTexture, playerData.GetCurrentPlayerHealth(), playerData.GetCurrentPlayerMaxHealth());

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                serializer.Serialize(stream, dataToSave);
            }

            Debug.Log($"Saved player data at: {savePath}");
        }
    }
}*/