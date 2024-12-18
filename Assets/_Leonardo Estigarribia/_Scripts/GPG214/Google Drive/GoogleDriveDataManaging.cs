using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

namespace _Leonardo_Estigarribia._Scripts.GPG214.Google_Drive
{
    public class GoogleDriveDataManaging : MonoBehaviour, ISavingLoadingData
    {
        private string currentIconId;

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
            StartCoroutine(DownloadPlayerIcon((downloadedIconData) =>
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

            currentIconId = request.ResponseData.Id;
            Debug.Log($"Successfully uploaded player icon. File ID: {currentIconId}.");
        }

        // The Action next to the byte array parameter its like setting an instruction after a task its done.
        private IEnumerator DownloadPlayerIcon(Action<byte[]> onComplete)
        {
            if (string.IsNullOrEmpty(currentIconId))
            {
                Debug.LogError($"No player icon ID set, a download can't start without one.");
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

            Debug.Log($"Successfully downloaded player icon");
            onComplete.Invoke(request.ResponseData.Content);
            
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
    }
}