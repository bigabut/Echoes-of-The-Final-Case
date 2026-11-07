using System.Collections;
using UnityEngine;

public class Meong : MonoBehaviour, EffectScripts
{
    [SerializeField] private int priority;
    public int Priority => priority;

    public IEnumerator playEffects()
    {
        AudioManager.Instance.PlaySFX("Meow");
        yield return null;
    }
}
