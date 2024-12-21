using System;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    [Serializable]
    public class AssetBundleInfo : MonoBehaviour
    {
        public string BundleId { get; set; }
        public string BundleName { get; set; }
        public DateTime UploadDate { get; set; }

        public AssetBundleInfo(string id, string name)
        {
            BundleId = id;
            BundleName = name;
            UploadDate = DateTime.Now;
        }
    }
}
