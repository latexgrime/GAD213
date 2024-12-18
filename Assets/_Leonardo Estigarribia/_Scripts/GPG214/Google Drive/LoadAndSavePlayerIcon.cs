using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

namespace _Leonardo_Estigarribia._Scripts.Google_Drive
{
    public class LoadAndSavePlayerIcon : MonoBehaviour
    {
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

        /// <summary>
        /// This is the ID of the image from Google Drive that is going to be downloaded and set as the new player icon.
        /// </summary>

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

    }
}