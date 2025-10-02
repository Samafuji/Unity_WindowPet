// using System.Collections;
// using UnityEngine;
// using UnityEngine.Networking;

// public class gettest : MonoBehaviour
// {
//     private string url = "{取得したURL}";
//     //関数から呼び出す。
//     private void Gettingdata()
//     {
//         StartCoroutine("GetData", url);
//     }
//     //コルーチン内で記述
//     IEnumerator GetData(string URL)
//     {
//         //ここでAPIを叩いてrequestに保存する。
//         UnityWebRequest request = UnityWebRequest.Get(URL);
//         //urlに接続してデータが帰ってくるまで待機状態にする。
//         yield return webRequest.SendWebRequest();
//         //エラー確認
//         switch (response.result)
//         {
//             case UnityWebRequest.Result.InProgress:
//                 Debug.Log("リクエスト中");
//                 break;
//             case UnityWebRequest.Result.Success:
//                 //成功
//                 Debug.Log(response.downloadHandler.text);
//                 break;
//         }
//     }
// }