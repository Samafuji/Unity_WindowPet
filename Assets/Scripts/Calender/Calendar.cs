using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class CalendarDaySelectedEvent : UnityEvent<string>
{
}

public class Calendar : MonoBehaviour
{
    [Header("Calendar Layout")]
    public GameObject canvas;
    public GameObject prefab;
    public Text monthLabel;
    public Button previousMonthButton;
    public Button nextMonthButton;

    [Header("Visual Settings")]
    public Color currentMonthColor = Color.white;
    public Color otherMonthColor = new Color(1f, 1f, 1f, 0.35f);
    public Color todayHighlightColor = new Color(0.2f, 0.6f, 1f, 0.35f);
    public Color selectedDayColor = new Color(1f, 0.8f, 0.1f, 0.45f);

    [Header("Events")]
    public CalendarDaySelectedEvent onDaySelected = new CalendarDaySelectedEvent();

    private readonly List<CalendarDayCell> dayCells = new List<CalendarDayCell>();
    private DateTime currentMonth;
    private DateTime? selectedDate;

    private void Awake()
    {
        BuildDayCellsIfNeeded();
        currentMonth = DateTime.Today;
    }

    private void OnEnable()
    {
        if (previousMonthButton != null)
        {
            previousMonthButton.onClick.AddListener(OnPreviousMonthClicked);
        }

        if (nextMonthButton != null)
        {
            nextMonthButton.onClick.AddListener(OnNextMonthClicked);
        }

        RefreshCalendar();
    }

    private void OnDisable()
    {
        if (previousMonthButton != null)
        {
            previousMonthButton.onClick.RemoveListener(OnPreviousMonthClicked);
        }

        if (nextMonthButton != null)
        {
            nextMonthButton.onClick.RemoveListener(OnNextMonthClicked);
        }
    }

    public void GoToToday()
    {
        currentMonth = DateTime.Today;
        selectedDate = DateTime.Today;
        RefreshCalendar();
    }

    public void SetMonth(DateTime targetMonth)
    {
        currentMonth = new DateTime(targetMonth.Year, targetMonth.Month, 1);
        RefreshCalendar();
    }

    private void OnPreviousMonthClicked()
    {
        ChangeMonth(-1);
    }

    private void OnNextMonthClicked()
    {
        ChangeMonth(1);
    }

    private void ChangeMonth(int offset)
    {
        currentMonth = currentMonth.AddMonths(offset);
        RefreshCalendar();
    }

    private void BuildDayCellsIfNeeded()
    {
        if (canvas == null || prefab == null)
        {
            Debug.LogWarning("Calendar: Canvas or prefab is not assigned.");
            return;
        }

        if (dayCells.Count > 0)
        {
            return;
        }

        for (int i = 0; i < 42; i++)
        {
            GameObject buttonObject = Instantiate(prefab, canvas.transform);
            Button buttonComponent = buttonObject.GetComponent<Button>();

            if (buttonComponent == null)
            {
                buttonComponent = buttonObject.AddComponent<Button>();
            }

            Text label = buttonObject.GetComponentInChildren<Text>();
            Image background = buttonObject.GetComponent<Image>();

            CalendarDayCell cell = new CalendarDayCell
            {
                button = buttonComponent,
                label = label,
                background = background,
                originalBackgroundColor = background != null ? background.color : Color.white
            };

            buttonComponent.onClick.AddListener(() => OnDayCellClicked(cell));
            dayCells.Add(cell);
        }
    }

    private void RefreshCalendar()
    {
        if (dayCells.Count == 0)
        {
            BuildDayCellsIfNeeded();
        }

        DateTime firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
        DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        int dayOffset = ((int)firstDayOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        DateTime startDate = firstDayOfMonth.AddDays(-dayOffset);

        for (int i = 0; i < dayCells.Count; i++)
        {
            CalendarDayCell cell = dayCells[i];
            DateTime cellDate = startDate.AddDays(i);
            cell.date = cellDate;

            bool isCurrentMonth = cellDate.Month == currentMonth.Month;
            bool isToday = cellDate.Date == DateTime.Today;
            bool isSelected = selectedDate.HasValue && cellDate.Date == selectedDate.Value.Date;

            if (cell.label != null)
            {
                cell.label.text = cellDate.Day.ToString();
                cell.label.color = isCurrentMonth ? currentMonthColor : otherMonthColor;
            }

            UpdateCellBackground(cell, isToday, isSelected);
        }

        if (monthLabel != null)
        {
            monthLabel.text = currentMonth.ToString("yyyy MMMM", CultureInfo.CurrentCulture);
        }
    }

    private void UpdateCellBackground(CalendarDayCell cell, bool isToday, bool isSelected)
    {
        if (cell.background == null)
        {
            return;
        }

        Color targetColor = cell.originalBackgroundColor;

        if (isToday)
        {
            targetColor = Color.Lerp(targetColor, todayHighlightColor, Mathf.Clamp01(todayHighlightColor.a));
        }

        if (isSelected)
        {
            targetColor = Color.Lerp(targetColor, selectedDayColor, Mathf.Clamp01(selectedDayColor.a));
        }

        cell.background.color = targetColor;
    }

    private void OnDayCellClicked(CalendarDayCell cell)
    {
        selectedDate = cell.date.Date;
        RefreshCalendar();

        if (onDaySelected != null)
        {
            onDaySelected.Invoke(cell.date.ToString("yyyy-MM-dd"));
        }
    }

    private class CalendarDayCell
    {
        public Button button;
        public Text label;
        public Image background;
        public Color originalBackgroundColor;
        public DateTime date;
    }
}
