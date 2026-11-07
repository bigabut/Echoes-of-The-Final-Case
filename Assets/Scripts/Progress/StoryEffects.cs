using System.Collections;
using UnityEngine;

public class StoryEffect : MonoBehaviour, EffectScripts
{
    [SerializeField] private int priority;
    [SerializeField] private string flagToSet;
    [SerializeField] private string newObjective;

    public int Priority => priority;

    public IEnumerator playEffects()
    {
        if (!string.IsNullOrEmpty(flagToSet) && StoryProgress.HasFlag(flagToSet))
        {
            yield break;
        }

        if (!string.IsNullOrEmpty(newObjective))
        {
            StoryProgress.SetObjective(newObjective);
        }

        StoryProgress.SetFlag(flagToSet);

        yield return null;
    }
}