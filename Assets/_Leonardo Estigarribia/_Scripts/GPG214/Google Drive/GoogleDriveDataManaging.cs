using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Xsl;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Google_Drive
{
    public class GoogleDriveDataManaging : MonoBehaviour, ISavingLoadingData
    {
        [Header("- Saved Icons List")] public List<PlayerIconInfo> savedIcons = new();

        [Header("- Manual Icon Loading")] [SerializeField]
        private string iconIdToLoad;

        [SerializeField] private KeyCode loadSpecificIconKeyCode = KeyCode.Keypad5;

        private const string IconDataNamePrefix = "SavedPlayerIcon_";
        private string currentIconId;

        private void Start()
        {
            LoadIconList();
        }

        private void Update()
        {
            if (Input.GetKeyDown(loadSpecificIconKeyCode) && !string.IsNullOrEmpty(iconIdToLoad))
            {
                StartCoroutine(LoadSpecificIconCoroutine(iconIdToLoad));
            }
        }

        IEnumerator LoadSpecificIconCoroutine(string iconID)
        {
            Debug.Log($"Attempting to load Icon with ID: {iconID}");

            var iconInfo = savedIcons.Find(icon => icon.IconId == iconID);
            if (iconInfo != null)
            {
                Debug.Log($"Icon found in saved list: {iconInfo.FileName}");
            }
            else
            {
                Debug.LogError("This ID wasn't found in the saved icons list. Attempting download from cloud.");
            }

            var request = GoogleDriveFiles.Download(iconID);
            yield return request.Send();

            if (request.IsError)
            {
                Debug.LogError($"{request.Error}");
                yield break;
            }

            var saveData = new PlayerSaveData
            {
                IconData = request.ResponseData.Content
            };

            var savingSystem = FindObjectOfType<GeneralSavingLoadingSystem>();
            savingSystem.ApplyIconData(saveData.IconData);
        }
        
        public async Task<PlayerSaveData> LoadSpecificIcon(string iconIdToLoad)
        {
            var taskCompletionSource = new TaskCompletionSource<PlayerSaveData>();

            StartCoroutine(DownloadPlayerIcon(iconIdToLoad, downloadedIconData =>
            {
                var saveData = new PlayerSaveData
                {
                    IconData = downloadedIconData
                };
                taskCompletionSource.SetResult(saveData);
            }));

            return await taskCompletionSource.Task;
        }

        
        public void SaveData(PlayerSaveData data)
        {
            if (data.IconData != null) StartCoroutine(UploadIcon(data.IconData));
        }

        public PlayerSaveData LoadData()
        {
            // Not used in this script.
            return null;
        }

        public async Task<PlayerSaveData> LoadDataAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<PlayerSaveData>();
            StartCoroutine(DownloadPlayerIcon(currentIconId, downloadedIconData =>
            {
                var saveData = new PlayerSaveData
                {
                    IconData = downloadedIconData
                };
                taskCompletionSource.SetResult(saveData);
            }));
            return await taskCompletionSource.Task;
        }

        private IEnumerator UploadIcon(byte[] iconData)
        {
            Debug.Log("Starting player icon upload to Google Drive...");

            // This is to create a unique name file for the icon in google drive.
            var fileName = $"PlayerIcon_{DateTime.Now}";

            var file = new File
            {
                Name = fileName,
                Content = iconData
            };

            var request = GoogleDriveFiles.Create(file);
            request.Fields = new List<string> { "id" };

            yield return request.Send();

            if (request.IsError)
            {
                Debug.LogError($"Failed to upload player icon, exception: {request.Error}");
                yield break;
            }

            // If everything goes well, store the ID in the list for future (and better debugging) purposes.\
            var newIconId = request.ResponseData.Id;
            AddNewIcon(newIconId, fileName);

            currentIconId = newIconId;

            Debug.Log($"Successfully uploaded player icon. File ID: {currentIconId}.");
        }
        

        // The Action next to the byte array parameter its like setting an instruction after a task its done.
        private IEnumerator DownloadPlayerIcon(string iconId, Action<byte[]> onComplete)
        {
            if (string.IsNullOrEmpty(iconId))
            {
                Debug.LogError("No player icon ID set, a download can't start without one.");
                onComplete.Invoke(null);
                yield break;
            }

            Debug.Log($"Starting player icon download from Google Drive, file ID: {currentIconId}");

            var request = GoogleDriveFiles.Download(currentIconId);
            yield return request.Send();

            if (request.IsError)
            {
                Debug.LogError($"Could not download player icon, exception: {request.Error}");
                onComplete.Invoke(null);
                yield break;
            }

            Debug.Log("Successfully downloaded player icon");
            onComplete.Invoke(request.ResponseData.Content);
        }

        private void SaveIconList()
        {
            PlayerPrefs.SetInt($"{IconDataNamePrefix}Count", savedIcons.Count);

            for (var i = 0; i < savedIcons.Count; i++)
            {
                // This is to differentiate different kinds of data.
                var baseName = $"{IconDataNamePrefix}{i}_";

                PlayerPrefs.SetString($"{baseName}ID", savedIcons[i].IconId);
                PlayerPrefs.SetString($"{baseName}FileName", savedIcons[i].FileName);
                PlayerPrefs.SetString($"{baseName}Date", savedIcons[i].SaveDate.Ticks.ToString());

                PlayerPrefs.Save();

                Debug.Log($"Saved {savedIcons.Count} icons data to playerPrefs.");
            }
        }

        private void LoadIconList()
        {
            // This is to refresh the list.
            savedIcons.Clear();

            var count = PlayerPrefs.GetInt($"{IconDataNamePrefix}Count", 0);

            for (var i = 0; i < count; i++)
            {
                var baseName = $"{IconDataNamePrefix}{i}_";
                var id = PlayerPrefs.GetString($"{baseName}ID");
                var fileName = PlayerPrefs.GetString($"{baseName}FileName");
                // I have no idea why this needs to be a long but it works so leave it like that.
                var date = long.Parse(PlayerPrefs.GetString($"{baseName}Date"));

                var iconInfo = new PlayerIconInfo(id, fileName)
                {
                    SaveDate = new DateTime(date)
                };

                savedIcons.Add(iconInfo);
            }

            Debug.Log($"Loaded {savedIcons.Count} icons data from PlayerPrefs.");
        }

        public void AddNewIcon(string iconID, string fileName)
        {
            savedIcons.Add(new PlayerIconInfo(iconID, fileName));
            SaveIconList();
            Debug.Log($"Added new icon: {fileName}, ID: {iconID}.");
        }

        public List<PlayerIconInfo> GetSavedIcons()
        {
            return new List<PlayerIconInfo>(savedIcons);
        }
    }
}

/*
#region OldCode


[SerializeField] private RectTransform playerIcon;

[Header("- Uploading image to Google Drive.")]
[SerializeField] private KeyCode saveKeyCode = KeyCode.Keypad7;
/// <summary>
/// This is going to automatically be set to be the player icon in the top right of the screen.
/// </summary>
[SerializeField] private Texture2D imageToUpload;

[Header("- Downloading image from Google Drive.")]
[SerializeField] private KeyCode loadKeyCode = KeyCode.Keypad9;
/// <summary>
/// This is the ID of the image you want to download.
/// </summary>
[SerializeField] private string googleDriveImageId ;

private byte[] downloadContent;


private void Start()
{
    imageToUpload = playerIcon.GetComponent<Image>().sprite.texture;
}

private void Update()
{
    if (Input.GetKeyDown(saveKeyCode)) StartCoroutine(UploadFile());

    if (Input.GetKeyDown(loadKeyCode))
    {
        StartCoroutine(DownloadFile());
    }
}

private IEnumerator UploadFile()
{
    Debug.Log("Starting uploading file to cloud coroutine.");
    // Encode texture and set request.
    var content = imageToUpload.EncodeToPNG();
    var file = new File { Name = "PlayerIcon", Content = content };
    var request = GoogleDriveFiles.Create(file);

    request.Fields = new List<string> { "id" };
    yield return request.Send();

    if (request.IsError)
    {
        Debug.LogError($"Problem with request, error: {request.Error}");
        yield break;
    }

    Debug.Log($"No problems with request: {request.IsError}");
    Debug.Log($"Request content: {request.ResponseData.Content}");
    Debug.Log($"Request ID: {request.ResponseData.Id}");
}

public IEnumerator DownloadFile()
{
    Debug.Log("Starting DOWNLOADING file to cloud coroutine.");

    var request = GoogleDriveFiles.Download(googleDriveImageId);
    yield return request.Send();

    if (request.IsError)
    {
        Debug.Log($"Error while downloading file. Exception: {request.Error}");
    }

    Debug.Log($"Is request done: {request.IsDone}");
    Debug.Log($"Response data: {request.ResponseData}");
    downloadContent = request.ResponseData.Content;

    Debug.Log($"Setting texture.");
    Texture2D downloadedTexture = new Texture2D(2, 2);
    downloadedTexture.LoadImage(downloadContent);
    downloadedTexture.Apply();
    SetPlayerIcon(downloadedTexture);
}

private void SetPlayerIcon(Texture2D texture)
{
    var rect = new Rect(0, 0, texture.width, texture.height);
    playerIcon.GetComponent<Image>().sprite = Sprite.Create(texture, rect, playerIcon.pivot);
}

#endregion*/