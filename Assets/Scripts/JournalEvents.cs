using System;

public static class JournalEvents
{
    public static event Action OnJournalUpdated;

    public static void NotifyJournalUpdated()
    {
        OnJournalUpdated?.Invoke();
    }
}