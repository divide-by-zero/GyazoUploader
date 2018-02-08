using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DivideByZero.Gyazo
{
    public class GyazoUploader : CustomYieldInstruction
    {
        public GyazoResponse Result { private set; get; }
        public string ErrorMessage { private set; get; }

        public bool IsError
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage) == false;
            }
        }
        public override bool keepWaiting { get { return Result == null && IsError == false; } }

        public GyazoUploader UploadScreenShotAsync(Action<GyazoResponse,string> callback = null)
        {
            GyazoUploadManager.Instance.StartCoroutine(UploadSSIterator(callback));
            return this;
        }

        private IEnumerator UploadSSIterator(Action<GyazoResponse,string> callback)
        {
            yield return new WaitForEndOfFrame();
            var tex = ScreenCapture.CaptureScreenshotAsTexture();
            yield return UploadJpegByteDataAsync(tex.EncodeToJPG(),callback);
            UnityEngine.Object.Destroy(tex);
        }

        public GyazoUploader UploadJpegByteDataAsync(byte[] jpegBytes, Action<GyazoResponse,string> callback = null)
        {
            GyazoUploadManager.Instance.StartCoroutine(UploadImageBytesIterator(jpegBytes, callback));
            return this;
        }

        private IEnumerator UploadImageBytesIterator(byte[] byteData, Action<GyazoResponse,string> callback)
        {
            var gyazoUploadUrl = "https://upload.gyazo.com/api/upload";

            var form = new WWWForm();
            form.AddField("access_token", GyazoUploadManager.Instance.AccessToken);
            form.AddBinaryData("imagedata", byteData, "screenshot.jpg", System.Net.Mime.MediaTypeNames.Image.Jpeg);
            using (var request = UnityWebRequest.Post(gyazoUploadUrl, form))
            {
                yield return request.SendWebRequest();
                var response = request.downloadHandler.text;
                ErrorMessage = request.error;

                if (GyazoUploadManager.Instance.IsDebugLogOutput)
                {
                    Debug.Log("Gyazo Upload Response:\n" + response);
                    if (string.IsNullOrEmpty(ErrorMessage) == false)
                    {
                        Debug.Log("Gyazo Upload Error:\n" + ErrorMessage);
                    }
                }

                if (request.responseCode == 200)
                {
                    var responseParse = JsonUtility.FromJson<GyazoResponse>(request.downloadHandler.text);
                    Result = responseParse;
                }
                if (callback != null) callback.Invoke(Result,ErrorMessage);
            }
        }
    }
}