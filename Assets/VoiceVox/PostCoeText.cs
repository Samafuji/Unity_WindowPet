using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

public class PostCoeText : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _voice;

    public string text = "こんにちは"; // 発話する文章

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine(CallCoeiroink(text));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CallCoeiroink(string text = "こんにちは", string speaker_id = "0", string core_version = "2.0.0")
    {
        // 音声合成のクエリ作成

        // URIを作る
        var queryString_query = System.Web.HttpUtility.ParseQueryString("");
        queryString_query.Add("text", text);
        queryString_query.Add("speaker", speaker_id);
        queryString_query.Add("core_version", core_version);

        var uriBuilder_query = new System.UriBuilder("http://localhost:50032/")
        {
            Query = queryString_query.ToString()
        };

        // WebRequestをPostする
        WWWForm form = new WWWForm();
        UnityWebRequest query_req = UnityWebRequest.Post(uriBuilder_query.Uri, form);
        yield return query_req.SendWebRequest();

        if (query_req.isNetworkError || query_req.isHttpError) // 失敗
        {
            Debug.Log("Network error:" + query_req.error);
        }
        else // 成功
        {
            Debug.Log("Succeeded:" + query_req.downloadHandler.text);
        }

        // .wavを生成する


        // URIを作る
        var queryString_synth = System.Web.HttpUtility.ParseQueryString("");
        queryString_synth.Add("speaker", speaker_id);
        queryString_synth.Add("core_version", core_version);
        queryString_synth.Add("enable_interrogative_upspeak", "true");

        var uriBuilder_synth = new System.UriBuilder("http://localhost:50032/v1/synthesis")
        {
            Query = queryString_synth.ToString()
        };

        // 上のURIでPOSTする

        var synth_req = new UnityWebRequest(uriBuilder_synth.Uri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(query_req.downloadHandler.text);
        synth_req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        synth_req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        synth_req.SetRequestHeader("Content-Type", "application/json");
        yield return synth_req.SendWebRequest();

        if (synth_req.isHttpError || synth_req.isNetworkError)
        {
            Debug.Log(synth_req.error);
        }
        else
        {
            // 変換して再生
            byte[] results = synth_req.downloadHandler.data; // 返ってくるのは.wavファイルのbyte列
            LoadAndPlayWavFile(results); // byte列をAudioClipに変換してAudioSourceに渡し、再生
        }
    }

    public void LoadAndPlayWavFile(byte[] wavfile)
    {
        AudioClip audioClip = WavUtility.ToAudioClip(wavfile);
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

}

