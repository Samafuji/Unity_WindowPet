using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;

public class GetweatherDay : MonoBehaviour
{
    private VoiceVoxTest voiceVoxTest;
    private string url = "https://api.openweathermap.org/data/2.5/weather?lat=";
    private string APIKEY = "26b74c2559c9628d032b839e3f743989";
    private string callurl;
    private string lat = "25.0851";
    private string lon = "121.6015";
    private const double ONE_HOUR_IN_SECONDS = 3600.0;
    public WeatherResponse weatherResponse;

    private void Start()
    {
        // "voicevox"という名前のオブジェクトを見つける
        GameObject voiceVoxObject = GameObject.Find("Voicevox");

        // そのオブジェクトにアタッチされているVoiceVoxTestコンポーネントを取得する
        if (voiceVoxObject != null)
        {
            voiceVoxTest = voiceVoxObject.GetComponent<VoiceVoxTest>();

            if (voiceVoxTest == null)
            {
                Debug.LogError("VoiceVoxTestコンポーネントが見つかりません。");
            }
        }
        else
        {
            Debug.LogError("voicevoxオブジェクトが見つかりません。");
        }

        LoadJsonToFile();

        NotifyWeather(weatherResponse);
    }

    public void Getting()
    {
        callurl = url + lat + "&lon=" + lon + "&units=metric&appid=" + APIKEY;
        Debug.Log("呼び出し中");
        Debug.Log(callurl);
        StartCoroutine(GetData(callurl));
    }

    private IEnumerator GetData(string URL)
    {
        UnityWebRequest response = UnityWebRequest.Get(URL);
        yield return response.SendWebRequest();

        switch (response.result)
        {
            case UnityWebRequest.Result.InProgress:
                Debug.Log("リクエスト中");
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Success!");
                SaveJsonToFile(response.downloadHandler.text);
                Debug.Log(response.downloadHandler.text);

                // 新しいデータを取得した後、再度データをロードしてVoiceVoxを実行する
                LoadJsonToFile();
                break;
        }
    }

    private void SaveJsonToFile(string jsonData)
    {
        string path = Application.persistentDataPath + "/WeatherNow.json";
        File.WriteAllText(path, jsonData);
        Debug.Log("Weather data saved to " + path);
    }

    public void LoadJsonToFile()
    {
        string path = Application.persistentDataPath + "/WeatherNow.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            weatherResponse = JsonUtility.FromJson<WeatherResponse>(json);
            if (weatherResponse != null)
            {
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.dt).DateTime;
                Debug.Log("Date and time: " + dateTime.ToString("yyyy-MM-dd HH:mm:ss"));

                // 現在の日時と比較して、1時間以上経過しているか確認
                double elapsedTime = (DateTime.UtcNow - dateTime).TotalSeconds;
                if (elapsedTime > ONE_HOUR_IN_SECONDS)
                {
                    Debug.Log("Weather data is older than 1 hour. Fetching new data.");
                    Getting(); // 1時間以上経過していたら、新しいデータを取得
                }
                else
                {
                    Debug.Log("Weather data is up-to-date.");
                    // データが最新であればVoiceVoxで通知する
                }
            }
            else
            {
                Debug.LogError("Failed to parse the weather data.");
            }
        }
        else
        {
            Debug.LogError("WeatherNow.json file not found.");
            Getting(); // ファイルが存在しない場合、新しいデータを取得
        }
    }

    private void NotifyWeather(WeatherResponse weatherResponse)
    {
        Debug.Log("現在の気温: " + weatherResponse.main.temp);
        Debug.Log("最高気温は: " + weatherResponse.main.temp_max);
        Debug.Log("湿度は: " + weatherResponse.main.humidity);
        Debug.Log("天気は: " + weatherResponse.weather[0].main);
        string weathermain = "";
        if (weatherResponse.weather[0].main == "clear")
        {
            weathermain = "晴れ";
        }
        if (weatherResponse.weather[0].main == "rain")
        {
            weathermain = "雨";
        }
        if (weatherResponse.weather[0].main == "clouds")
        {
            weathermain = "雲り";
        }
        Debug.Log(weatherResponse.weather[0].description);

        string Comment = "";
        voiceVoxTest.PlayReminderVoice("おはようございます！現在の気温は" + weatherResponse.main.temp + "度です！最高気温は" + weatherResponse.main.temp_max + "度です！湿度は" + weatherResponse.main.humidity + "です！天気は" + weathermain + weatherResponse.weather[0].description + Comment);
    }
}

[System.Serializable]
public class WeatherResponse
{
    public Coord coord;
    public Weather[] weather;

    public string baseName; // `base` を `baseName` に変更している
    public Main main;
    public int visibility;
    public Wind wind;
    public Rain rain;
    public Clouds clouds;
    public long dt;
    public Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;
}
