using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public class WeatherManager : MonoBehaviour
{
    private GetweatherDay getweatherDay;
    private GetweatherWeek getweatherWeek;

    public Image imageWeatherNow;

    public GameObject imageWeatherPrefab; // The prefab that contains the Image and Text components
    public Transform parentTransform; // The parent object where the prefabs will be instantiated


    public Sprite Sunny;
    public Sprite Rain;
    public Sprite Clouds;
    // Start is called before the first frame update
    void Start()
    {
        // "voicevox"という名前のオブジェクトを見つける
        GameObject Weather = GameObject.Find("Weather");

        // そのオブジェクトにアタッチされているVoiceVoxTestコンポーネントを取得する
        if (Weather != null)
        {
            getweatherDay = Weather.GetComponent<GetweatherDay>();
            getweatherWeek = Weather.GetComponent<GetweatherWeek>();
        }
        else
        {
            Debug.LogError("voicevoxオブジェクトが見つかりません。");
        }
        getweatherDay.LoadJsonToFile();
        getweatherWeek.LoadJsonToFile();

        SetupWeatherUI(14);

        if (getweatherDay.weatherResponse.weather[0].main == "Clear")
        {
            imageWeatherNow.sprite = Sunny;
        }
        else if (getweatherDay.weatherResponse.weather[0].main == "Rain")
        {
            imageWeatherNow.sprite = Rain;
        }
        else if (getweatherDay.weatherResponse.weather[0].main == "Clouds")
        {
            imageWeatherNow.sprite = Clouds;
        }
        else
        {
            imageWeatherNow.sprite = Sunny;
        }
    }
    string GetDateMD(string date)
    {
        DateTime dateTime = DateTime.Parse(date);
        return dateTime.ToString("MM dd"); // "08 11"
    }

    string GetTimeHM(string date)
    {
        DateTime dateTime = DateTime.Parse(date);
        return dateTime.ToString("HH:mm"); // "09:00"
    }

    void SetupWeatherUI(int k)
    {
        if (k > getweatherWeek.weatherData.list.Count) k = getweatherWeek.weatherData.list.Count;

        // Start the loop from k-1 down to 0
        for (int i = 0; i < k; i++)
        {
            // Instantiate the prefab
            GameObject newWeatherItem = Instantiate(imageWeatherPrefab, parentTransform);

            // Find the Button component in the prefab
            Button weatherButton = newWeatherItem.GetComponentInChildren<Button>();
            if (weatherButton == null)
            {
                Debug.LogError("Button component not found on the instantiated prefab.");
                return;
            }

            Image weatherImage = weatherButton.GetComponent<Image>();
            if (weatherImage == null)
            {
                Debug.LogError("Image component not found on the Button.");
                return;
            }

            TextMeshProUGUI MMDD = newWeatherItem.transform.Find("MMDD").GetComponent<TextMeshProUGUI>();
            if (MMDD == null)
            {
                Debug.LogError("MMDD TextMeshProUGUI component not found.");
                return;
            }

            TextMeshProUGUI HRMM = newWeatherItem.transform.Find("HHMM").GetComponent<TextMeshProUGUI>();
            if (HRMM == null)
            {
                Debug.LogError("HRMM TextMeshProUGUI component not found.");
                return;
            }


            // Set the text
            MMDD.text = GetDateMD(getweatherWeek.weatherData.list[i].dt_txt);
            HRMM.text = GetTimeHM(getweatherWeek.weatherData.list[i].dt_txt);

            // Set the image based on the weather condition
            if (getweatherWeek.weatherData.list[i].weather[0].main == "Clear")
            {
                weatherImage.sprite = Sunny;
            }
            else if (getweatherWeek.weatherData.list[i].weather[0].main == "Rain")
            {
                weatherImage.sprite = Rain;
            }
            else if (getweatherWeek.weatherData.list[i].weather[0].main == "Clouds")
            {
                weatherImage.sprite = Clouds;
            }
            else
            {
                weatherImage.sprite = Sunny;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
