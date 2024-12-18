using System.Threading.Tasks;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public interface ISavingLoadingData
    {
        void SaveData(PlayerSaveData data);
        PlayerSaveData LoadData();
        Task<PlayerSaveData> LoadDataAsync();
    }
}