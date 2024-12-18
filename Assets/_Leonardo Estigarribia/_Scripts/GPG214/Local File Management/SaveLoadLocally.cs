using System;
using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Local_File_Management
{
    public class SaveLoadLocally : MonoBehaviour
    {
        private PlayerData playerData;
        [SerializeField] private PlayerSaveData playerSaveData;
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
            if (Input.GetKeyDown(localSaveKeyCode))
            {
                SaveLocalData();
            }

            if (Input.GetKeyDown(localLoadKeyCode))
            {
                LoadLocalData();
            }
        }

        private void LoadLocalData()
        {
            if (!File.Exists(savePath))
            {
                Debug.LogError($"No save file found at {savePath}");
                return;
            }
            else
            {
                PlayerSaveData loadedData;
                using (FileStream stream = new FileStream(savePath, FileMode.Open))
                {
                    loadedData = (PlayerSaveData)serializer.Deserialize(stream);
                }

                Vector3 loadedPosition = new Vector3(loadedData.positionX, loadedData.positionY, loadedData.positionZ);

                playerData.playerPosition = loadedPosition;
                // This moves the player to the position.
                playerData.SetPlayerPosition(loadedPosition.x, loadedPosition.y, loadedPosition.z);

                playerData.playerName = loadedData.playerName;

                if (loadedData.playerIconBytes != null)
                {
                    Texture2D loadedTexture = new Texture2D(2, 2);
                    if (loadedTexture.LoadImage(loadedData.playerIconBytes))
                    {
                        Sprite newSprite = Sprite.Create(
                            loadedTexture,
                            new Rect(0,0,loadedTexture.width, loadedTexture.height),
                            // Setting the pivot in the center for now.
                            new Vector2(0.5f,0.5f)
                            );
                        playerData.playerIcon.sprite = newSprite;
                    }
                }
                Debug.Log($"Success in loading player data from: {savePath}.");
            }
        }

        private void SaveLocalData()
        {
            string directory = Path.GetDirectoryName(savePath);
            
            // Check if the directory exits, otherwise create it.
            if (!Directory.Exists(directory))
            {
                if (directory != null) Directory.CreateDirectory(directory);
            }
            
            Texture2D playerIconTexture = null;
            
            // Check if the player has an Icon set.
            if (playerData.playerIcon != null)
            {
                playerIconTexture = playerData.playerIcon.sprite.texture;
            }

            PlayerSaveData dataToSave = new PlayerSaveData(playerData.playerPosition, playerData.playerName, playerIconTexture);
            
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                serializer.Serialize(stream, dataToSave);
            }
            
            Debug.Log($"Saved player data at: {savePath}");
        }
    }
}