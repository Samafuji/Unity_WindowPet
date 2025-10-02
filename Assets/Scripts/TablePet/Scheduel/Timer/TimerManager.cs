using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TimerManager : MonoBehaviour
{
    private VoiceVoxTest voiceVoxTest;
    public TMP_InputField titleInputField;
    public TMP_InputField timeInputField;
    public Button applyButton;
    public Button addButton;
    public Transform timerListParent;
    public GameObject timerPrefab;

    private List<Timer> timers = new List<Timer>();
    private Timer selectedTimer;
    private Dictionary<Timer, GameObject> timerUIMap = new Dictionary<Timer, GameObject>();


    void Start()
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

        applyButton.onClick.AddListener(OnApplyButtonClicked);
        addButton.onClick.AddListener(OnAddButtonClicked);
        LoadTimersFromJson();
    }

    void Update()
    {
        CheckTimers();
    }

    void OnApplyButtonClicked()
    {
        if (selectedTimer == null) return;

        selectedTimer.title = titleInputField.text;
        selectedTimer.duration = int.Parse(timeInputField.text);
        selectedTimer.remainingTime = selectedTimer.duration; // タイマーをリセット
        UpdateTimer();
        SaveTimersToJson();
    }

    void OnAddButtonClicked()
    {
        int duration;
        if (int.TryParse(timeInputField.text, out duration))
        {
            Timer newTimer = new Timer
            {
                title = titleInputField.text,
                duration = duration,
                remainingTime = duration,
                isEnabled = true
            };
            timers.Add(newTimer);
            AddTimerToUI(newTimer);
            SaveTimersToJson();
        }
    }

    void AddTimerToUI(Timer timer)
    {
        timer.remainingTime = timer.duration;

        GameObject timerObject = Instantiate(timerPrefab, timerListParent);
        TextMeshProUGUI timerText = timerObject.transform.Find("TitleText").GetComponent<TextMeshProUGUI>();
        timerText.text = timer.title + " - " + FormatTime(timer.duration);

        Button toggleButton = timerObject.transform.Find("ToggleButton").GetComponent<Button>();
        UpdateToggleButtonColor(toggleButton, timer.isEnabled);
        toggleButton.onClick.AddListener(() => ToggleTimer(timer, toggleButton));

        Button deleteButton = timerObject.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => DeleteTimer(timer, timerObject));

        TextMeshProUGUI RemainingText = timerObject.transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
        RemainingText.text = FormatTime(timer.remainingTime);

        timerObject.GetComponent<Button>().onClick.AddListener(() => OnTimerSelected(timer));

        // Track the UI element for the timer
        timerUIMap[timer] = timerObject;
    }

    void ToggleTimer(Timer timer, Button button)
    {
        timer.isEnabled = !timer.isEnabled;
        UpdateToggleButtonColor(button, timer.isEnabled);
        SaveTimersToJson();
    }

    void UpdateToggleButtonColor(Button button, bool isEnabled)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = isEnabled ? Color.green : Color.red;
        button.colors = colors;
    }

    void DeleteTimer(Timer timer, GameObject timerObject)
    {
        timers.Remove(timer);
        Destroy(timerObject);
        SaveTimersToJson();
    }

    void OnTimerSelected(Timer timer)
    {
        selectedTimer = timer;
        titleInputField.text = timer.title;
        timeInputField.text = timer.duration.ToString();
    }


    void UpdateTimer()
    {
        foreach (Transform child in timerListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Timer timer in timers)
        {
            AddTimerToUI(timer);
        }
    }
    void UpdateTimerUI()
    {
        foreach (var timer in timers)
        {
            if (timerUIMap.TryGetValue(timer, out GameObject timerObject))
            {
                TextMeshProUGUI remainingText = timerObject.transform.Find("TimerText").GetComponent<TextMeshProUGUI>();
                remainingText.text = FormatTime(timer.remainingTime);
            }
        }
    }


    void SaveTimersToJson()
    {
        string json = JsonUtility.ToJson(new TimerListWrapper(timers), true);
        File.WriteAllText(Application.persistentDataPath + "/timers.json", json);
    }

    void LoadTimersFromJson()
    {
        string filePath = Application.persistentDataPath + "/timers.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            TimerListWrapper wrapper = JsonUtility.FromJson<TimerListWrapper>(json);
            timers = wrapper.timers;
            UpdateTimer();
        }
    }

    void CheckTimers()
    {
        foreach (var timer in timers)
        {
            if (timer.isEnabled && timer.remainingTime > 0)
            {
                timer.remainingTime -= Time.deltaTime;
                if (timer.remainingTime <= 0)
                {
                    timer.remainingTime = timer.duration;
                    Debug.Log($"Timer: {timer.title} has finished.");
                    voiceVoxTest.PlayReminderVoice(timer.title);
                }
            }
        }
        UpdateTimerUI();
    }

    string FormatTime(float timeInSeconds)
    {
        int hours = Mathf.FloorToInt(timeInSeconds / 3600F);
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        if (hours > 0)
        {
            return string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            return string.Format("{0:0}:{1:00}", minutes, seconds);
        }
    }
}

[System.Serializable]
public class Timer
{
    public string title;
    public float duration;
    public float remainingTime;
    public bool isEnabled;
}

[System.Serializable]
public class TimerListWrapper
{
    public List<Timer> timers;

    public TimerListWrapper(List<Timer> timers)
    {
        this.timers = timers;
    }
}
