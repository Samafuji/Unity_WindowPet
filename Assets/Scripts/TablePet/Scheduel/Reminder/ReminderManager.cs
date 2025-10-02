using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReminderManager : MonoBehaviour
{
    private const string ReminderFileName = "reminders.json";

    private VoiceVoxTest voiceVoxTest;

    [Header("Input References")]
    public TMP_InputField titleInputField;
    public TMP_InputField timeInputField;
    public Button applyButton;
    public Button addButton;

    [Header("List References")]
    public Transform reminderListParent;
    public GameObject reminderPrefab;

    [Header("Reminder Settings")]
    [Tooltip("Interval in seconds at which reminders are checked.")]
    public float checkInterval = 10.0f;

    private readonly List<Reminder> reminders = new List<Reminder>();
    private readonly Dictionary<Reminder, GameObject> reminderUIMap = new Dictionary<Reminder, GameObject>();
    private readonly Dictionary<GameObject, Color> reminderItemDefaultColors = new Dictionary<GameObject, Color>();

    private Reminder selectedReminder;
    private float timeSinceLastCheck;

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

        LoadRemindersFromJson();
        RebuildReminderUI();
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
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck >= Mathf.Max(1.0f, checkInterval))
        {
            CheckReminders();
            timeSinceLastCheck = 0.0f;
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
        if (selectedReminder == null)
        {
            return;
        }

        if (!TryParseReminderTime(timeInputField.text, out TimeSpan reminderTime))
        {
            Debug.LogWarning($"不正な時間形式です: {timeInputField.text}");
            return;
        }

        selectedReminder.title = titleInputField.text;
        selectedReminder.time = FormatReminderTime(reminderTime);
        selectedReminder.lastTriggeredDate = string.Empty;

        RebuildReminderUI();
        SaveRemindersToJson();
    }

    private void OnAddButtonClicked()
    {
        if (!TryParseReminderTime(timeInputField.text, out TimeSpan reminderTime))
        {
            Debug.LogWarning($"不正な時間形式です: {timeInputField.text}");
            return;
        }

        Reminder newReminder = new Reminder
        {
            title = titleInputField != null ? titleInputField.text : string.Empty,
            time = FormatReminderTime(reminderTime),
            isEnabled = true,
            lastTriggeredDate = string.Empty
        };

        reminders.Add(newReminder);
        SortReminders();
        RebuildReminderUI();
        OnReminderSelected(newReminder);
        SaveRemindersToJson();
    }

    private void RebuildReminderUI()
    {
        foreach (GameObject item in reminderUIMap.Values)
        {
            if (item != null)
            {
                Destroy(item);
            }

            reminderItemDefaultColors.Remove(item);
        }

        reminderUIMap.Clear();

        if (reminderPrefab == null || reminderListParent == null)
        {
            return;
        }

        SortReminders();

        foreach (Reminder reminder in reminders)
        {
            if (reminder == null)
            {
                continue;
            }

            GameObject reminderObject = Instantiate(reminderPrefab, reminderListParent);
            ConfigureReminderItem(reminder, reminderObject);
            reminderUIMap[reminder] = reminderObject;
        }

        UpdateSelectedReminderVisuals();
    }

    private void ConfigureReminderItem(Reminder reminder, GameObject reminderObject)
    {
        if (reminderObject == null)
        {
            return;
        }

        TextMeshProUGUI displayText = reminderObject.GetComponentInChildren<TextMeshProUGUI>();

        if (displayText != null)
        {
            displayText.text = $"{reminder.time} - {reminder.title}";
        }

        Image background = reminderObject.GetComponent<Image>();

        if (background != null)
        {
            reminderItemDefaultColors[reminderObject] = background.color;
        }

        Button itemButton = reminderObject.GetComponent<Button>();

        if (itemButton != null)
        {
            itemButton.onClick.RemoveAllListeners();
            itemButton.onClick.AddListener(() => OnReminderSelected(reminder));
        }

        Button toggleButton = reminderObject.transform.Find("ToggleButton")?.GetComponent<Button>();

        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(() => ToggleReminder(reminder, toggleButton));
            UpdateToggleButtonColor(toggleButton, reminder.isEnabled);
        }

        Button deleteButton = reminderObject.transform.Find("DeleteButton")?.GetComponent<Button>();

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(() => DeleteReminder(reminder));
        }
    }

    private void ToggleReminder(Reminder reminder, Button button)
    {
        if (reminder == null)
        {
            return;
        }

        reminder.isEnabled = !reminder.isEnabled;

        if (button != null)
        {
            UpdateToggleButtonColor(button, reminder.isEnabled);
        }

        SaveRemindersToJson();
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

    private void DeleteReminder(Reminder reminder)
    {
        if (reminder == null)
        {
            return;
        }

        if (reminderUIMap.TryGetValue(reminder, out GameObject reminderObject) && reminderObject != null)
        {
            Destroy(reminderObject);
        }

        reminderUIMap.Remove(reminder);

        if (reminderObject != null)
        {
            reminderItemDefaultColors.Remove(reminderObject);
        }
        reminders.Remove(reminder);

        if (selectedReminder == reminder)
        {
            selectedReminder = null;

            if (titleInputField != null)
            {
                titleInputField.text = string.Empty;
            }

            if (timeInputField != null)
            {
                timeInputField.text = string.Empty;
            }
        }

        SaveRemindersToJson();
        RebuildReminderUI();
    }

    private void OnReminderSelected(Reminder reminder)
    {
        selectedReminder = reminder;

        if (selectedReminder == null)
        {
            return;
        }

        if (titleInputField != null)
        {
            titleInputField.text = selectedReminder.title;
        }

        if (timeInputField != null)
        {
            timeInputField.text = selectedReminder.time;
        }

        UpdateSelectedReminderVisuals();
    }

    private void UpdateSelectedReminderVisuals()
    {
        foreach (KeyValuePair<Reminder, GameObject> entry in reminderUIMap)
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

            Color baseColor = reminderItemDefaultColors.TryGetValue(entry.Value, out Color storedColor) ? storedColor : background.color;
            Color selectedColor = Color.Lerp(baseColor, new Color(0.85f, 0.9f, 1f, baseColor.a), 0.5f);
            background.color = entry.Key == selectedReminder ? selectedColor : baseColor;
        }
    }

    private void LoadRemindersFromJson()
    {
        reminders.Clear();

        string filePath = Path.Combine(Application.persistentDataPath, ReminderFileName);

        if (!File.Exists(filePath))
        {
            return;
        }

        string json = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        ReminderListWrapper wrapper = JsonUtility.FromJson<ReminderListWrapper>(json);

        if (wrapper != null && wrapper.reminders != null)
        {
            reminders.AddRange(wrapper.reminders);
        }

        SortReminders();
    }

    private void SaveRemindersToJson()
    {
        string filePath = Path.Combine(Application.persistentDataPath, ReminderFileName);
        ReminderListWrapper wrapper = new ReminderListWrapper(reminders);
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
    }

    private void SortReminders()
    {
        reminders.Sort((a, b) =>
        {
            if (a == null && b == null)
            {
                return 0;
            }

            if (a == null)
            {
                return 1;
            }

            if (b == null)
            {
                return -1;
            }

            bool aParsed = TryParseReminderTime(a.time, out TimeSpan aTime);
            bool bParsed = TryParseReminderTime(b.time, out TimeSpan bTime);

            if (aParsed && bParsed)
            {
                int timeComparison = aTime.CompareTo(bTime);

                if (timeComparison != 0)
                {
                    return timeComparison;
                }
            }
            else if (aParsed)
            {
                return -1;
            }
            else if (bParsed)
            {
                return 1;
            }

            return string.Compare(a.title, b.title, StringComparison.CurrentCulture);
        });
    }

    private void CheckReminders()
    {
        if (reminders.Count == 0)
        {
            return;
        }

        DateTime now = DateTime.Now;
        string todayKey = now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        TimeSpan window = TimeSpan.FromSeconds(Mathf.Max(1.0f, checkInterval));

        bool hasChanges = false;

        foreach (Reminder reminder in reminders)
        {
            if (reminder == null || !reminder.isEnabled)
            {
                continue;
            }

            if (!TryParseReminderTime(reminder.time, out TimeSpan reminderTime))
            {
                continue;
            }

            DateTime scheduledTime = now.Date + reminderTime;

            if (scheduledTime < now - window || scheduledTime > now)
            {
                continue;
            }

            if (!string.Equals(reminder.lastTriggeredDate, todayKey, StringComparison.Ordinal))
            {
                Debug.Log($"Reminder: {reminder.title} at {reminder.time}");

                if (voiceVoxTest != null)
                {
                    voiceVoxTest.PlayReminderVoice(reminder.title);
                }

                reminder.lastTriggeredDate = todayKey;
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            SaveRemindersToJson();
        }
    }

    private static bool TryParseReminderTime(string timeText, out TimeSpan time)
    {
        time = TimeSpan.Zero;

        if (string.IsNullOrWhiteSpace(timeText))
        {
            return false;
        }

        string trimmed = timeText.Trim();
        string[] formats = { "H:mm", "HH:mm", "H:mm:ss", "HH:mm:ss" };

        foreach (string format in formats)
        {
            if (DateTime.TryParseExact(trimmed, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsed))
            {
                time = parsed.TimeOfDay;
                return true;
            }
        }

        return false;
    }

    private static string FormatReminderTime(TimeSpan time)
    {
        if (time.TotalHours >= 1.0f)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}", (int)time.TotalHours, time.Minutes);
        }

        return string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}", time.Hours, time.Minutes);
    }
}
