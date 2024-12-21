using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
{
    public class GoogleDriveAssetBundleManager : MonoBehaviour
    {
        private static GoogleDriveAssetBundleManager _instance;

        public static GoogleDriveAssetBundleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GoogleDriveAssetBundleManager>();
                    if (_instance == null)
                    {
                        var GoogleDriveAssetBundleManagerGameObject = new GameObject("Google Drive Asset Bundle Manager");
                        _instance =
                            GoogleDriveAssetBundleManagerGameObject.AddComponent<GoogleDriveAssetBundleManager>();
                    }
                }

                return _instance;
            }
            
        }

        private List<AssetBundleInfo> savedBundles = new List<AssetBundleInfo>();
        private string BundleDataNamePrefix = "SavedAssetBundle_";

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                LoadBundleList();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void LoadBundleList()
        {
            // Handle the loading of asset bundles kinda the same way I did it with player prefs and player icons.
            savedBundles.Clear();
            
        }
    }
    
}