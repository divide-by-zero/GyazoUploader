# GyazoUploader
画像共有サービスgyazoへのアップロード

以下、使い方になります。

**注意：Unity2017.3以降じゃないと動きません**

# 1.GyazoAPIのAccessToken取得
まず、Gyazoのユーザーが無いとどうしようもないので、登録はしておいてください。
その後 https://gyazo.com/api へ行き、「アプリケーションを登録」
![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/4674a542-97ee-896e-7eec-4a7f0c365c43.png)

「New Application」のボタンを押し、NameとCallbackURLを入れます。
Nameには自分の好きな名前を。今回は「UnityWebGLImageUpload」としました。
CallbackURLは適当に入れました。よくわかってません。
![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/414ce329-a89b-559a-7fc3-9605efee5b9b.png)

Submitして作られたApplication名をクリックして、認証情報表示画面へ。
**Your access token** の下に「Generate」のボタンがあると思うので、クリックして、AccessTokenを生成。
![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/7931a398-8c13-3c8d-ed34-68365f96228d.png)

↓

![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/e7be3ad6-3b74-a300-0c33-59b8359f6538.png)

**このAccessTokenによって画像アップロードの認証をするので、コピーしてメモ帳にでも取っておきます。**
(もちろん、もう一度この画面を開けば確認できますし。「Regenerate」で再生成も出来ます。）

# 2.Unity側準備
https://github.com/divide-by-zero/GyazoUploader/releases から、GyazoUpload.unitypackageをダウンロードして、使いたいプロジェクトにインポートしてください

![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/0afc4de3-1396-f036-02a1-c2d927308971.png)

適当なシーンにGyazoUploader/Prefabs/GyazoUploadManager を配置

![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/a696bcfe-ba39-b7d0-e11f-08c9c5f91f0d.png)

**※空のGameObjectを配置して、GyazoUploader/Scripts/GyazoUploadManager.csを自分でアタッチしても良いです**

inspectorの**AccessToken**に、1.で取得したAccessTokenを記入。

![image.png](https://qiita-image-store.s3.amazonaws.com/0/37184/c18b9ef4-39dc-434c-15d8-ed23a23fb959.png)

**Is Debug Log Output** はチェックを入れると微妙に通信のログが出力されます。デバッグ時にどうぞ。

# 3.さぁ、画像をアップロードしよう
`GyazoUploader`を使って実際に画像(ScreenShot)をアップロード出来ます。

例えば、画面をクリックした瞬間のスクリーンショットを取得してアップロードそアップロード結果をブラウザで表示するのであれば以下のように書きます。

```csharp
using DivideByZero.Gyazo;
using UnityEngine;

public class GyazoUploadTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var uploader = new GyazoUploader();
            uploader.UploadScreenShotAsync((response, error) => {
                if (string.IsNullOrEmpty(error)) //errorがnullなら正常終了
                {
                    Application.OpenURL(response.url); //とりあえずブラウザで開く
                }
            });
        }
    }
}
```

この`UploadScreenShotAsync`は勝手にスクリーンショットを取得してjpgデータにしてgyazoにアップロードまでしてくれます。**やったね。**
なお、アップロード結果はコールバックの１つ目のresponseに色々と入っています。（詳しくはソースを見てね！！）
また、コールバックの２つめのstringにはエラー内容で、正常時はnullなので成否判定に使ってください。
（例では`string.IsNullOrEmpty`使ってますが、ただのnullチェックでもいいはず）

大体の用途はこれだけで済みそうですが、自分で加工したjpgデータなんかも送リたい場合は`UploadJpegByteDataAsync`で、byte[]を渡せれるようになっているので、そちらを使用してください。


#4.オススメな使い方
上記のコールバック形式でもいいんですが、１つコルーチンで囲う方が直感的(UnityEngine.WWWと同じ使い方）なので、オススメです。

```csharp
using System.Collections;
using DivideByZero.Gyazo;
using UnityEngine;

public class GyazoUploadTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(UploadIterator());
        }

    }

    private IEnumerator UploadIterator()
    {
        var uploader = GyazoUploadManager.CreateGyazoUploader();    //これでも作れるけど new GyazoUploader()の方が短いって言う・・・
        yield return uploader.UploadScreenShotAsync();  //待機可能

        if (uploader.IsError)
        {
            Debug.Log(uploader.ErrorMessage);   //エラーメッセージ表示
        }
        else
        {
            Application.OpenURL(uploader.Result.url);
        }
    }
}
```

エラー有無は `IsError`プロパティ  
エラー内容は `ErrorMessage`プロパティ  
成功時の結果は `Result`プロパティ  

に格納されます。

