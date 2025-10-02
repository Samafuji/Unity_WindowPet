using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ReminderManager : MonoBehaviour
{
    private VoiceVoxTest voiceVoxTest;
    public TMP_InputField titleInputField;
    public TMP_InputField timeInputField;
    public Button applyButton;
    public Button addButton;
    public Transform reminderListParent;
    public GameObject reminderPrefab;

    private List<Reminder> reminders = new List<Reminder>();
    private Reminder selectedReminder;

    public float checkInterval = 10.0f; // 10secごとにチェック
    private float timeSinceLastCheck = 0.0f;

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
        LoadRemindersFromJson();
    }

    void Update()
    {
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            CheckReminders();
            timeSinceLastCheck = 0.0f;
        }
    }

    void OnApplyButtonClicked()
    {
        if (selectedReminder == null) return;

        selectedReminder.title = titleInputField.text;
        selectedReminder.time = timeInputField.text;
        UpdateReminderUI();
        SaveRemindersToJson();
    }
    void OnAddButtonClicked()
    {
        DateTime now = DateTime.Now;
        string currentTime = now.ToString("HH:mm"); // "HH:mm" フォーマットで時間と分を取得

        Reminder newReminder = new Reminder
        {
            title = titleInputField.text,
            time = currentTime, // 現在の時間を設定
            isEnabled = true
        };
        reminders.Add(newReminder);
        AddReminderToUI(newReminder);
        OnReminderSelected(newReminder);
        SaveRemindersToJson();
    }

    void AddReminderToUI(Reminder reminder)
    {
        GameObject reminderObject = Instantiate(reminderPrefab, reminderListParent);
        reminderObject.GetComponentInChildren<TextMeshProUGUI>().text = reminder.time + " - " + reminder.title;
        reminderObject.GetComponent<Button>().onClick.AddListener(() => OnReminderSelected(reminder));

        // オン/オフボタン
        Button toggleButton = reminderObject.transform.Find("ToggleButton").GetComponent<Button>();
        UpdateToggleButtonColor(toggleButton, reminder.isEnabled);
        toggleButton.onClick.AddListener(() => ToggleReminder(reminder, toggleButton));

        // 削除ボタン
        Button deleteButton = reminderObject.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => DeleteReminder(reminder, reminderObject));

        reminderObject.GetComponent<Button>().onClick.AddListener(() => OnReminderSelected(reminder));
    }

    void ToggleReminder(Reminder reminder, Button button)
    {
        reminder.isEnabled = !reminder.isEnabled;
        UpdateToggleButtonColor(button, reminder.isEnabled);
        SaveRemindersToJson();
    }

    void UpdateToggleButtonColor(Button button, bool isEnabled)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = isEnabled ? Color.green : Color.red;
        button.colors = colors;
    }

    void DeleteReminder(Reminder reminder, GameObject reminderObject)
    {
        reminders.Remove(reminder);
        Destroy(reminderObject);
        SaveRemindersToJson();
    }


    void OnReminderSelected(Reminder reminder)
    {
        selectedReminder = reminder;
        titleInputField.text = reminder.title;
        timeInputField.text = reminder.time;
    }

    void UpdateReminderUI()
    {
        foreach (Transform child in reminderListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Reminder reminder in reminders)
        {
            AddReminderToUI(reminder);
        }
    }
    void SaveRemindersToJson()
    {
        string json = JsonUtility.ToJson(new ReminderListWrapper(reminders), true);
        File.WriteAllText(Application.persistentDataPath + "/reminders.json", json);
    }

    void LoadRemindersFromJson()
    {
        string filePath = Application.persistentDataPath + "/reminders.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ReminderListWrapper wrapper = JsonUtility.FromJson<ReminderListWrapper>(json);
            reminders = wrapper.reminders;
            UpdateReminderUI();
        }
    }

    void CheckReminders()
    {
        DateTime now = DateTime.Now;
        string currentTime = now.ToString("HH:mm");

        foreach (var reminder in reminders)
        {
            if (reminder.isEnabled && reminder.time == currentTime)
            {
                Debug.Log($"Reminder: {reminder.title} at {reminder.time}");
                // VoiceVoxで音声を再生
                voiceVoxTest.PlayReminderVoice(reminder.title);
            }
        }
    }
}
