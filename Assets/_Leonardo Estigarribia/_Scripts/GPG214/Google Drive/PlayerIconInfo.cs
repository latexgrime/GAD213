using System;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Google_Drive
{
    [Serializable]
    public class PlayerIconInfo
    {
        public string IconId { get; set; }
        public string FileName { get; set; }
        public DateTime SaveDate { get; set; }

        public PlayerIconInfo(string id, string fileName)
        {
            IconId = id;
            FileName = fileName;
            SaveDate = DateTime.Now;
        }
    }
}