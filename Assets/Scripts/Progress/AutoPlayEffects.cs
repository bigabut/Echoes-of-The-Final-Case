using System.Collections;
using System.Linq;
using UnityEngine;

public class AutoPlayEffects : MonoBehaviour
{
    public enum Mode
    {
        OnStart,
        OnPlayerEnter
    }

    [SerializeField] private Mode mode = Mode.OnStart;
    [SerializeField] private float startDelay = 0.5f;

    private bool hasPlayed;

    private void Start()
    {
        if (mode == Mode.OnStart)
        {
            Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (mode == Mode.OnPlayerEnter && other.CompareTag("Player"))
        {
            Play();
        }
    }

    private void Play()
    {
        if (hasPlayed)
        {
            return;
        }

        hasPlayed = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        yield return new WaitForSeconds(startDelay);

        while (Dialogue.IsAnyDialogueActive)
        {
            yield return null;
        }

        var effects = GetComponents<MonoBehaviour>()
            .OfType<EffectScripts>()
            .OrderBy(effect => effect.Priority);

        foreach (EffectScripts effect in effects)
        {
            yield return effect.playEffects();
        }
    }
}