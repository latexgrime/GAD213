using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class LocalDataManaging : ISavingLoadingData
    {
        private readonly string savePath;
        private readonly XmlSerializer serializer;

        public LocalDataManaging(string fileName)
        {
            savePath = Path.Combine(Application.persistentDataPath, fileName);
            serializer = new XmlSerializer(typeof(PlayerSaveData));
        }
        
        public void SaveData(PlayerSaveData data)
        {
            var directory = Path.GetDirectoryName(savePath);
            if (directory != null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }

            Debug.Log($"Data locally saved at: {savePath}");
        }

        public PlayerSaveData LoadData()
        {
            if (!File.Exists(savePath))
            {
                Debug.LogError($"Save file not found at: {savePath}");
                return null;
            }
            using (var stream = new FileStream(savePath, FileMode.Open))
            {
                Debug.Log($"File found at: {savePath}");
                
                return (PlayerSaveData)serializer.Deserialize(stream);
            }
        }

        public Task<PlayerSaveData> LoadDataAsync()
        {
            // This is because of the interface, a workaround due to the PlayFabDataManaging operations.
            return Task.FromResult(LoadData());
        }
    }
}