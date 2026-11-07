using UnityEngine;
using UnityEngine.Events;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] private string[] requiredFlags;
    [SerializeField] private string newObjective;
    [SerializeField] private string flagToSet;
    [SerializeField] private UnityEvent onMet;

    private bool hasFired;

    private void OnEnable()
    {
        StoryProgress.OnFlagSet += HandleFlagSet;
        Check();
    }

    private void OnDisable()
    {
        StoryProgress.OnFlagSet -= HandleFlagSet;
    }

    private void HandleFlagSet(string flag)
    {
        Check();
    }

    private void Check()
    {
        if (hasFired || requiredFlags == null || requiredFlags.Length == 0)
        {
            return;
        }

        foreach (string flag in requiredFlags)
        {
            if (!StoryProgress.HasFlag(flag))
            {
                return;
            }
        }

        hasFired = true;

        if (!string.IsNullOrEmpty(newObjective))
        {
            StoryProgress.SetObjective(newObjective);
        }

        onMet?.Invoke();
        StoryProgress.SetFlag(flagToSet);
    }
}