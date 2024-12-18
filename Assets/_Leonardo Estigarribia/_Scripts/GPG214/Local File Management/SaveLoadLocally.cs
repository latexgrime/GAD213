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
                // Save player data locally.
                SaveLocalData();
            }

            if (Input.GetKeyDown(localLoadKeyCode))
            {
                // Load player data locally.
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