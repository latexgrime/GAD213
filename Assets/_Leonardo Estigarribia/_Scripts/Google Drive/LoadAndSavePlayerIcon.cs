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
        public RectTransform playerIcon;

        public Texture2D imageToUpload;
        public RawImage downloadedDriveImage;
        public byte[] downloadContent;

        [SerializeField] private KeyCode saveKeyCode = KeyCode.Keypad7;
        [SerializeField] private KeyCode loadKeyCode = KeyCode.Keypad9;

        private void Start()
        {
            //imageToUpload = playerIcon.GetComponent<Image>().sprite.texture;
        }

        private void Update()
        {
            if (Input.GetKeyDown(saveKeyCode)) StartCoroutine(UploadFile());

            if (Input.GetKeyDown(loadKeyCode))
            {
                // Load image from Drive cloud.
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

        /*public IEnumerator DownloadFile()
        {
            var request = GoogleDriveFiles.Download()
        }*/
    }
}