using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Leonardo_Estigarribia._Scripts.GPG214.AssetBundles
{
    public class CoinVariantSpawnerUI : MonoBehaviour
    {
        [Header("- Spawn buttons")] 
        [SerializeField] private Button spawnGoldCoinButton;
        [SerializeField] private Button spawnSilverCoinButton;
        [SerializeField] private Button spawnBronzeCoinButton;

        [Header("- Upload button")] 
        [SerializeField] private Button uploadBundleButton;
        [SerializeField] private TextMeshProUGUI uploadStatusText;

        [Header("- Spawn setting")] 
        [SerializeField] private Vector3 spawnCoinOffset = new Vector3(2,0,0);
        private Transform playerTransform;

        [Header("- Header AssetBundles")] 
        [SerializeField] private string assetBundlePathToUpload = "Assets/AssetBundles/coin_variants";
        [SerializeField] private string assetBundleNameToUpload = "coin_variants";

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            if (spawnGoldCoinButton != null)
            {
                spawnGoldCoinButton.onClick.AddListener(() => SpawnCoinVariant("CoinGold"));
            }
            if (spawnSilverCoinButton != null)
            {
                spawnSilverCoinButton.onClick.AddListener(() => SpawnCoinVariant("CoinSilver"));
            }
            if (spawnBronzeCoinButton != null)
            {
                spawnBronzeCoinButton.onClick.AddListener(() => SpawnCoinVariant("CoinBronze"));
            }

            if (uploadBundleButton != null)
            {
                uploadBundleButton.onClick.AddListener(UploadAssetBundle);
            }
        }

        private void SpawnCoinVariant(string variantName)
        {
            Vector3 spawnPosition = playerTransform.position + spawnCoinOffset;
            AssetBundleManager.Instance.SpawnCoinVariant(variantName, spawnPosition);
        }
        
        private async void UploadAssetBundle()
        {
            uploadStatusText.text = "Uploading bundle...";
            bool success =
                await GoogleDriveAssetBundleManager.Instance.UploadAssetBundle(assetBundlePathToUpload,
                    assetBundleNameToUpload);
            uploadStatusText.text = success ? "Upload complete." : "Upload failed.";
            await Task.CompletedTask;
            uploadStatusText.text = "";
        }
        
    }
    
}