using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214.AssetBundles
{
    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManager _instance;

        public static AssetBundleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AssetBundleManager>();
                    if (_instance == null)
                    {
                        var assetBundleManagerGameObject = new GameObject("AssetBundleManager");
                        _instance = assetBundleManagerGameObject.AddComponent<AssetBundleManager>();
                    }
                }

                return _instance;
            }
        }

        private Dictionary<string, AssetBundle> loadedBundles = new();

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

        public async Task<GameObject> LoadCoinVariant(string variantName)
        {
            const string bundleName = "coin_variants";

            try
            {
                if (!loadedBundles.ContainsKey(bundleName))
                {
                    byte[] bundleData = await GoogleDriveAssetBundleManager.Instance.DownloadAssetBundle(bundleName);
                    if (bundleData == null)
                    {
                        Debug.LogError($"Failed to download bundle data.");
                        return null;
                    }
                    
                    AssetBundle bundle = AssetBundle.LoadFromMemory(bundleData);
                    if (bundle == null)
                    {
                        Debug.LogError("Failed to load asset bundle from memory.");
                        return null;
                    }

                    loadedBundles[bundleName] = bundle;
                }

                GameObject coinPrefab = loadedBundles[bundleName].LoadAsset<GameObject>(variantName);
                if (coinPrefab == null)
                {
                    Debug.LogError($"Failed to load coin variant: {variantName}");
                    return null;
                }

                return coinPrefab;
            }
            catch
            {
                Debug.LogError($"Error loading coin variant.");
                return null;
            }
        }

        public async void SpawnCoinVariant(string variantName, Vector3 position)
        {
            GameObject coinPrefab = await LoadCoinVariant(variantName);
            if (coinPrefab != null)
            {
                Instantiate(coinPrefab, position, Quaternion.identity);
            }
        }
    }
}