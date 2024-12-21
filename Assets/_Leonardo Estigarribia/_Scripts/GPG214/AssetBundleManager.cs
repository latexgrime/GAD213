using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Leonardo_Estigarribia._Scripts.GPG214
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
                    byte[] bundleData = await 
                }
            }
        }
    }
}