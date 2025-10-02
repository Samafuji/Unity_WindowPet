using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    private const string TimerFileName = "timers.json";

    private VoiceVoxTest voiceVoxTest;

    [Header("Input References")]
    public TMP_InputField titleInputField;
    public TMP_InputField timeInputField;
    public Button applyButton;
    public Button addButton;

    [Header("List References")]
    public Transform timerListParent;
    public GameObject timerPrefab;

    [Header("Timer Settings")]
    [Tooltip("Interval in seconds for refreshing the timer labels.")]
    public float uiRefreshInterval = 0.5f;

    private readonly List<Timer> timers = new List<Timer>();
    private readonly Dictionary<Timer, GameObject> timerUIMap = new Dictionary<Timer, GameObject>();
    private readonly Dictionary<GameObject, Color> timerItemDefaultColors = new Dictionary<GameObject, Color>();

    private Timer selectedTimer;
    private float uiRefreshElapsed;

    private void Awake()
    {
        LocateVoiceVox();
    }

    private void OnEnable()
    {
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(OnApplyButtonClicked);
        }

        if (addButton != null)
        {
            addButton.onClick.AddListener(OnAddButtonClicked);
        }

        LoadTimersFromJson();
        RebuildTimerList();
    }

    private void OnDisable()
    {
        if (applyButton != null)
        {
            applyButton.onClick.RemoveListener(OnApplyButtonClicked);
        }

        if (addButton != null)
        {
            addButton.onClick.RemoveListener(OnAddButtonClicked);
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        TickTimers(deltaTime);

        uiRefreshElapsed += deltaTime;

        if (uiRefreshElapsed >= Mathf.Max(0.1f, uiRefreshInterval))
        {
            RefreshTimerRemainingLabels();
            uiRefreshElapsed = 0f;
        }
    }

    private void LocateVoiceVox()
    {
        GameObject voiceVoxObject = GameObject.Find("Voicevox");

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
            Debug.LogWarning("voicevoxオブジェクトが見つかりません。");
        }
    }

    private void OnApplyButtonClicked()
    {
        if (selectedTimer == null)
        {
            return;
        }

        if (!TryParseDuration(timeInputField.text, out float durationSeconds))
        {
            Debug.LogWarning($"不正な時間形式です: {timeInputField.text}");
            return;
        }

        selectedTimer.title = titleInputField != null ? titleInputField.text : string.Empty;
        selectedTimer.duration = Mathf.Max(1f, durationSeconds);
        selectedTimer.remainingTime = selectedTimer.duration;

        UpdateTimerListEntry(selectedTimer);
        SaveTimersToJson();
    }

    private void OnAddButtonClicked()
    {
        if (!TryParseDuration(timeInputField.text, out float durationSeconds))
        {
            Debug.LogWarning($"不正な時間形式です: {timeInputField.text}");
            return;
        }

        Timer newTimer = new Timer
        {
            title = titleInputField != null ? titleInputField.text : string.Empty,
            duration = Mathf.Max(1f, durationSeconds),
            remainingTime = Mathf.Max(1f, durationSeconds),
            isEnabled = true
        };

        timers.Add(newTimer);
        RebuildTimerList();
        OnTimerSelected(newTimer);
        SaveTimersToJson();
    }

    private void TickTimers(float deltaTime)
    {
        if (timers.Count == 0)
        {
            return;
        }

        bool shouldPersist = false;

        foreach (Timer timer in timers)
        {
            if (timer == null || !timer.isEnabled)
            {
                continue;
            }

            if (timer.remainingTime > 0f)
            {
                timer.remainingTime = Mathf.Max(0f, timer.remainingTime - deltaTime);
            }

            if (timer.remainingTime <= 0f)
            {
                HandleTimerCompleted(timer);
                shouldPersist = true;
            }
        }

        if (shouldPersist)
        {
            SaveTimersToJson();
        }
    }

    private void HandleTimerCompleted(Timer timer)
    {
        if (timer == null)
        {
            return;
        }

        Debug.Log($"Timer: {timer.title} has finished.");

        if (voiceVoxTest != null)
        {
            voiceVoxTest.PlayReminderVoice(timer.title);
        }

        timer.remainingTime = timer.duration;
    }

    private void RebuildTimerList()
    {
        foreach (GameObject item in timerUIMap.Values)
        {
            if (item != null)
            {
                Destroy(item);
            }

            timerItemDefaultColors.Remove(item);
        }

        timerUIMap.Clear();

        if (timerPrefab == null || timerListParent == null)
        {
            return;
        }

        foreach (Timer timer in timers)
        {
            if (timer == null)
            {
                continue;
            }

            timer.remainingTime = Mathf.Clamp(timer.remainingTime, 0f, Mathf.Max(timer.duration, timer.remainingTime));

            GameObject timerObject = Instantiate(timerPrefab, timerListParent);
            ConfigureTimerItem(timer, timerObject);
            timerUIMap[timer] = timerObject;
        }

        UpdateSelectedTimerVisuals();
        RefreshTimerRemainingLabels();
    }

    private void ConfigureTimerItem(Timer timer, GameObject timerObject)
    {
        if (timerObject == null)
        {
            return;
        }

        TextMeshProUGUI titleText = timerObject.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();

        if (titleText != null)
        {
            titleText.text = $"{timer.title} - {FormatTime(timer.duration)}";
        }

        TextMeshProUGUI remainingText = timerObject.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();

        if (remainingText != null)
        {
            remainingText.text = FormatTime(timer.remainingTime);
        }

        Image background = timerObject.GetComponent<Image>();

        if (background != null)
        {
            timerItemDefaultColors[timerObject] = background.color;
        }

        Button itemButton = timerObject.GetComponent<Button>();

        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnTimerSelected(timer));
        }

        Button toggleButton = timerObject.transform.Find("ToggleButton")?.GetComponent<Button>();

        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(() => ToggleTimer(timer, toggleButton));
            UpdateToggleButtonColor(toggleButton, timer.isEnabled);
        }

        Button deleteButton = timerObject.transform.Find("DeleteButton")?.GetComponent<Button>();

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => DeleteTimer(timer));
        }
    }

    private void UpdateTimerListEntry(Timer timer)
    {
        if (timer == null || !timerUIMap.TryGetValue(timer, out GameObject timerObject) || timerObject == null)
        {
            return;
        }

        TextMeshProUGUI titleText = timerObject.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();

        if (titleText != null)
        {
            titleText.text = $"{timer.title} - {FormatTime(timer.duration)}";
        }

        TextMeshProUGUI remainingText = timerObject.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();

        if (remainingText != null)
        {
            remainingText.text = FormatTime(timer.remainingTime);
        }

        UpdateSelectedTimerVisuals();
    }

    private void RefreshTimerRemainingLabels()
    {
        foreach (KeyValuePair<Timer, GameObject> entry in timerUIMap)
        {
            if (entry.Key == null || entry.Value == null)
            {
                continue;
            }

            TextMeshProUGUI remainingText = entry.Value.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();

            if (remainingText != null)
            {
                remainingText.text = FormatTime(entry.Key.remainingTime);
            }
        }
    }

    private void ToggleTimer(Timer timer, Button button)
    {
        if (timer == null)
        {
            return;
        }

        timer.isEnabled = !timer.isEnabled;

        if (button != null)
        {
            UpdateToggleButtonColor(button, timer.isEnabled);
        }

        if (!timer.isEnabled)
        {
            timer.remainingTime = timer.duration;
        }

        SaveTimersToJson();
    }

    private void UpdateToggleButtonColor(Button button, bool isEnabled)
    {
        if (button == null)
        {
            return;
        }

        Color enabledColor = Color.green;
        Color disabledColor = Color.red;

        ColorBlock colors = button.colors;
        colors.normalColor = isEnabled ? enabledColor : disabledColor;
        colors.highlightedColor = colors.normalColor;
        colors.pressedColor = colors.normalColor;
        colors.selectedColor = colors.normalColor;
        button.colors = colors;
    }

    private void DeleteTimer(Timer timer)
    {
        if (timer == null)
        {
            return;
        }

        if (timerUIMap.TryGetValue(timer, out GameObject timerObject) && timerObject != null)
        {
            Destroy(timerObject);
        }

        timerUIMap.Remove(timer);

        if (timerObject != null)
        {
            timerItemDefaultColors.Remove(timerObject);
        }
        timers.Remove(timer);

        if (selectedTimer == timer)
        {
            selectedTimer = null;

            if (titleInputField != null)
            {
                titleInputField.text = string.Empty;
            }

            if (timeInputField != null)
            {
                timeInputField.text = string.Empty;
            }
        }

        UpdateSelectedTimerVisuals();
        SaveTimersToJson();
    }

    private void OnTimerSelected(Timer timer)
    {
        selectedTimer = timer;

        if (selectedTimer == null)
        {
            return;
        }

        if (titleInputField != null)
        {
            titleInputField.text = selectedTimer.title;
        }

        if (timeInputField != null)
        {
            timeInputField.text = FormatTime(selectedTimer.duration);
        }

        UpdateSelectedTimerVisuals();
    }

    private void UpdateSelectedTimerVisuals()
    {
        foreach (KeyValuePair<Timer, GameObject> entry in timerUIMap)
        {
            if (entry.Value == null)
            {
                continue;
            }

            Image background = entry.Value.GetComponent<Image>();

            if (background == null)
            {
                continue;
            }

            Color baseColor = timerItemDefaultColors.TryGetValue(entry.Value, out Color storedColor) ? storedColor : background.color;
            Color selectedColor = Color.Lerp(baseColor, new Color(0.85f, 0.95f, 1f, baseColor.a), 0.5f);
            background.color = entry.Key == selectedTimer ? selectedColor : baseColor;
        }
    }

    private void SaveTimersToJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, TimerFileName);
        TimerListWrapper wrapper = new TimerListWrapper(timers);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    private void LoadTimersFromJson()
    {
        timers.Clear();

        string filePath = Path.Combine(Application.persistentDataPath, TimerFileName);

        if (!File.Exists(filePath))
        {
            return;
        }

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        TimerListWrapper wrapper = JsonUtility.FromJson<TimerListWrapper>(json);

        if (wrapper != null && wrapper.timers != null)
        {
            timers.AddRange(wrapper.timers);
        }
    }

    private static bool TryParseDuration(string durationText, out float seconds)
    {
        seconds = 0f;

        if (string.IsNullOrWhiteSpace(durationText))
        {
            return false;
        }

        string trimmed = durationText.Trim();

        if (trimmed.Contains(":"))
        {
            string[] parts = trimmed.Split(':');

            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out int minutesValue) &&
                    int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out int secondsValue))
                {
                    seconds = Mathf.Max(0, minutesValue * 60 + secondsValue);
                    return true;
                }
            }
            else if (parts.Length == 3)
            {
                if (int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out int hoursValue) &&
                    int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out int minutesValue) &&
                    int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.CurrentCulture, out int secondsValue))
                {
                    seconds = Mathf.Max(0, hoursValue * 3600 + minutesValue * 60 + secondsValue);
                    return true;
                }
            }

            return false;
        }

        char suffix = char.ToLowerInvariant(trimmed[trimmed.Length - 1]);

        if (!char.IsDigit(suffix))
        {
            string numericPortion = trimmed.Substring(0, trimmed.Length - 1);

            if (!float.TryParse(numericPortion, NumberStyles.Float, CultureInfo.CurrentCulture, out float value))
            {
                return false;
            }

            switch (suffix)
            {
                case 'h':
                    seconds = Mathf.Max(0f, value * 3600f);
                    return true;
                case 'm':
                    seconds = Mathf.Max(0f, value * 60f);
                    return true;
                case 's':
                    seconds = Mathf.Max(0f, value);
                    return true;
                default:
                    return false;
            }
        }

        if (float.TryParse(trimmed, NumberStyles.Float, CultureInfo.CurrentCulture, out float parsedSeconds))
        {
            seconds = Mathf.Max(0f, parsedSeconds);
            return true;
        }

        return false;
    }

    private static string FormatTime(float timeInSeconds)
    {
        timeInSeconds = Mathf.Max(0f, timeInSeconds);

        int hours = Mathf.FloorToInt(timeInSeconds / 3600f);
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);

        if (hours > 0)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        return string.Format(CultureInfo.CurrentCulture, "{0:0}:{1:00}", minutes, seconds);
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
