using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SampleSceneController : MonoBehaviour
{
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void Start()
    {
        OnClickedButton();
        button?.onClick.AddListener(OnClickedButton);
    }

    private void OnClickedButton()
    {
        StartCoroutine(GetRequest("https://www.jma.go.jp/bosai/forecast/data/overview_forecast/130000.json"));
    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                // Error.
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + webRequest.error);
                        break;
                    default:
                        // ここにはこない.
                        break;
                }
                yield break;
            }

            var response = JsonUtility.FromJson<Response>(webRequest.downloadHandler.text);
            Debug.Log("データ配信元: " + response.publishingOffice);
            Debug.Log("報告日時: " + response.reportDatetime);
            Debug.Log("対象の地域: " + response.targetArea);
            Debug.Log("ヘッドライン: " + response.headlineText);
            Debug.Log("詳細: " + response.text);
            yield return null;
        }
    }
}

[System.Serializable]
public class Response
{
    public string publishingOffice;
    public string reportDatetime;
    public string targetArea;
    public string headlineText;
    public string text;
}