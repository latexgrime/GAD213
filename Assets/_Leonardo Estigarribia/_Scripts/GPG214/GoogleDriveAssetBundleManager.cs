using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityGoogleDrive;

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

        public async Task<byte[]> DownloadAssetBundle(string bundleName)
        {
            var taskCompletionSource = new TaskCompletionSource<byte[]>();

            var bundleInfo = savedBundles.Find(b => b.BundleName == bundleName);
            if (bundleInfo == null)
            {
                Debug.LogError($"No asset bundle found with name: {bundleName}");
                taskCompletionSource.SetResult(null);
                return await taskCompletionSource.Task;
            }

            StartCoroutine(DownloadBundleCoroutine(bundleInfo.BundleId, downloadedData =>
            {
                taskCompletionSource.SetResult(downloadedData);
            }));
            return await taskCompletionSource.Task;
        }

        private IEnumerator DownloadBundleCoroutine(string bundleId, Action<byte[]> onComplete)
        {
            Debug.Log($"Downloaded Asset Bundle with ID: {bundleId}");

            var request = GoogleDriveFiles.Download(bundleId);
            yield return request.Send();

            if (request.IsError)
            {
                Debug.LogError($"Could not complete asset bundle download. Exception: {request.Error}");
                onComplete?.Invoke(null);
            }

            Debug.Log($"Successfully downloaded asset bundle");
            onComplete?.Invoke(request.ResponseData.Content);
        }

        public async Task<bool> UploadAssetBundle(string bundlePath, string bundleName)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            byte[] bundleData = File.ReadAllBytes(bundlePath);
            StartCoroutine(UploadBundleCoroutine(bundleData, bundleName, success =>
            {
                taskCompletionSource.SetResult(success);
            }));
            return await taskCompletionSource.Task;
        }

        private IEnumerator UploadBundleCoroutine(byte[] bundleData, string bundleName, Action<bool> onComplete)
        {
            Debug.Log($"Uploading Asset Bundle: {bundleName}");
            var file = new UnityGoogleDrive.Data.File()
            {
                Name = bundleName,
                Content = bundleData
            };

            var request = GoogleDriveFiles.Create(file);
            request.Fields = new List<string> { "id" };
            
            yield return request.Send();
            
            if (request.IsError)
            {
                Debug.LogError($"Could not upload Asset Bundle. Exception: {request.Error}");
                onComplete?.Invoke(false);
            }

            var bundleId = request.ResponseData.Id;
            
            AddNewBundle(bundleId, bundleName);
            
            Debug.Log($"Successfully uploaded asset bundle. {bundleId}");
            onComplete?.Invoke(true);
        }

        private void AddNewBundle(string bundleId, string bundleName)
        {
            savedBundles.Add( new AssetBundleInfo(bundleId, bundleName));
            SaveBundleList();
        }

        private void SaveBundleList()
        {
            PlayerPrefs.SetInt($"{BundleDataNamePrefix}Count", savedBundles.Count);

            for (int i = 0; i < savedBundles.Count; i++)
            {
                var baseName = $"{BundleDataNamePrefix}{i}_";
                PlayerPrefs.SetString($"{baseName}ID", savedBundles[i].BundleId);
                PlayerPrefs.SetString($"{baseName}Name", savedBundles[i].BundleName);
                PlayerPrefs.Save();
            }
        }

        private void LoadBundleList()
        {
            savedBundles.Clear();
            var count = PlayerPrefs.GetInt($"{BundleDataNamePrefix}Count", 0);

            for (int i = 0; i < count; i++)
            {
                var baseName = $"{BundleDataNamePrefix}{i}_";
                var bundleId = PlayerPrefs.GetString($"{baseName}ID");
                var bundleName = PlayerPrefs.GetString($"{baseName}Name");

                if (!string.IsNullOrEmpty(bundleId) && !string.IsNullOrEmpty(bundleName))
                {
                    savedBundles.Add(new AssetBundleInfo(bundleId, bundleName));
                }
            }
            
            Debug.Log($"Loaded {savedBundles.Count} asset bundles.");
        }
    }
    
}