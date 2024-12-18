using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Google_Drive
{
    [Serializable]
    public class PlayerIconInfo
    {

        [SerializeField] private string iconId;
        [SerializeField] private string fileName;
        [SerializeField] private DateTime saveDate;


        public string IconId => iconId;
        public string FileName => fileName;
        public DateTime SaveDate
        {
            get => saveDate;
            set => saveDate = value;
        }

        public PlayerIconInfo(string id, string fileName)
        {
            iconId = id;
            this.fileName = fileName;
            saveDate = DateTime.Now;
        }

        public PlayerIconInfo(string id, string fileName, DateTime date) : this(id, fileName)
        {
            SaveDate = date;
        }
    }
}