using System;
using System.Collections;
using DivideByZero.Gyazo;
using UnityEngine;
using UnityEngine.UI;

public class GyazoUploadSample : MonoBehaviour
{
    [SerializeField]
    private Button _sendButton;
    [SerializeField]
    private Text _timeText;

    public void Start()
    {
        //クリックしたらスクリーンショットを撮って、Gyazoにアップロード
        _sendButton.onClick.AddListener(() =>
            StartCoroutine(UploadGyazoIterator())
        );
    }

    private IEnumerator UploadGyazoIterator()
    {
        var uploader = GyazoUploadManager.CreateGyazoUploader();
        yield return uploader.UploadScreenShotAsync();

        if (uploader.IsError == false)
        {
            Application.OpenURL(uploader.Result.permalink_url);
        }
    }

    void Update()
    {
        //現在時間を表示
        _timeText.text = DateTime.Now.ToString();
    }
}
