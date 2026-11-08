using System.Collections;
using UnityEngine;

public class klontang : MonoBehaviour, EffectScripts
{
    [SerializeField] private int priority;
    public int Priority => priority;

    public IEnumerator playEffects()
    {
        AudioManager.Instance.PlaySFX("klontang");
        yield return new WaitForSeconds(2f);
    }
}
