using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Globalization;

public class GetweatherWeek : MonoBehaviour
{
    // private string url = "https://api.openweathermap.org/data/2.5/weather?lat=";
    private string url = "https://api.openweathermap.org/data/2.5/forecast?lat=";
    private string APIKEY = "26b74c2559c9628d032b839e3f743989";
    private string callurl;
    private string lat = "25.0851";
    private string lon = "121.6015";
    public WeatherData weatherData;

    private void Start()
    {
        if (!LoadJsonToFile()) // false is meaning that no data is available
        {
            Getting();
        }
    }
    public void Getting()
    {
        callurl = url + lat + "&lon=" + lon + "&exclude=hourly,daily&units=metric&appid=" + APIKEY;
        Debug.Log("呼び出し中");
        Debug.Log(callurl);
        StartCoroutine("GetData", callurl);
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
                break;
        }
    }


    private void SaveJsonToFile(string jsonData)
    {
        string path = Application.persistentDataPath + "/Weather.json";
        File.WriteAllText(path, jsonData);
        Debug.Log("Weather data saved to " + path);
    }

    public bool LoadJsonToFile() //UTC+8 TW
    {
        string path = Application.persistentDataPath + "/Weather.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            weatherData = JsonUtility.FromJson<WeatherData>(json);
            if (weatherData != null && weatherData.list.Count > 0)
            {
                string firstDate = weatherData.list[0].dt_txt;
                Debug.Log("First date: " + firstDate);

                // Convert firstDate to DateTime
                DateTime firstDateTime;
                if (DateTime.TryParseExact(firstDate, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out firstDateTime))
                {
                    DateTime today = DateTime.Now.Date;
                    if (firstDateTime.Date == today)
                    {
                        Debug.Log("The first date is today.");
                        return true;
                    }
                    else
                    {
                        Debug.Log("The first date is not today.");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Failed to parse the first date.");
                    return false;
                }
            }
            else
            {
                Debug.LogError("No weather data available.");
                return false;
            }
        }
        else
        {
            Debug.LogError("Weather.json file not found.");
            return false;
        }
    }

}

[System.Serializable]
public class WeatherData
{
    public string cod;
    public int message;
    public int cnt;
    public List<WeatherList> list;
    public City city;
}

[System.Serializable]
public class WeatherList
{
    public long dt;
    public Main main;
    public List<Weather> weather;
    public Clouds clouds;
    public Wind wind;
    public int visibility;
    public float pop;
    public Rain rain;
    public Sys sys;
    public string dt_txt;
}

[System.Serializable]
public class Main
{
    public float temp;
    public float feels_like;
    public float temp_min;
    public float temp_max;
    public int pressure;
    public int sea_level;
    public int grnd_level;
    public int humidity;
    public float temp_kf;
}


[System.Serializable]
public class Coord
{
    public float lat;
    public float lon;
}

[System.Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[System.Serializable]
public class Clouds
{
    public int all;
}

[System.Serializable]
public class Wind
{
    public float speed;
    public int deg;
    public float gust;
}

[System.Serializable]
public class Rain
{
    public float _3h;
}

[System.Serializable]
public class Sys
{
    public string pod;
}

[System.Serializable]
public class City
{
    public int id;
    public string name;
    public Coord coord;
    public string country;
    public int population;
    public int timezone;
    public long sunrise;
    public long sunset;
}
