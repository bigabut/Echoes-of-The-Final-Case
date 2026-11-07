using System;
using System.Collections.Generic;
using UnityEngine;

public static class StoryProgress
{
    private static readonly HashSet<string> flags = new HashSet<string>();

    public static string CurrentObjective { get; private set; } = "";

    public static event Action<string> OnFlagSet;
    public static event Action<string> OnObjectiveChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        flags.Clear();
        CurrentObjective = "";
        OnFlagSet = null;
        OnObjectiveChanged = null;
    }

    public static bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public static void SetFlag(string flag)
    {
        if (string.IsNullOrEmpty(flag) || !flags.Add(flag))
        {
            return;
        }

        Debug.Log($"[StoryProgress] Flag diset: {flag}");
        OnFlagSet?.Invoke(flag);
    }

    public static void SetObjective(string objective)
    {
        CurrentObjective = objective;

        Debug.Log($"[StoryProgress] Objective: {objective}");
        OnObjectiveChanged?.Invoke(objective);
    }
}