using System;
using System.Collections;
using UnityEngine;

public class VoiceVoxTest : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    public int ID = 47;// 54
    public String Comment = "こんにちは！今日は何をするんですか？";

    void Start()
    {
        // StartCoroutine(SpeakTest(Comment));
    }

    public IEnumerator SpeakTest(string text)
    {
        // VOICEVOXのREST-APIクライアント
        VoiceVoxApiClient client = new VoiceVoxApiClient();

        // テキストからAudioClipを生成（話者は「8:春日部つむぎ」）

        // ナースロボ＿タイプＴ	
        //  ノーマル	47
        //  楽々	48
        //  恐怖	49
        //  内緒話	50
        // yield return client.TextToAudioClip(47, text);
        yield return client.TextToAudioClip(ID, text);

        if (client.AudioClip != null)
        {
            // AudioClipを取得し、AudioSourceにアタッチ
            _audioSource.clip = client.AudioClip;
            // AudioSourceで再生
            _audioSource.Play();
        }
    }

    public void PlayReminderVoice(string reminderText)
    {
        StartCoroutine(SpeakTest(reminderText));
    }
}
