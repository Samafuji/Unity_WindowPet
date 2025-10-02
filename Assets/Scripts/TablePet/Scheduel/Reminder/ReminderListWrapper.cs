using System;
using System.Collections.Generic;

[System.Serializable]
public class ReminderListWrapper
{
    public List<Reminder> reminders;

    public ReminderListWrapper(List<Reminder> reminders)
    {
        this.reminders = reminders;
    }
}
