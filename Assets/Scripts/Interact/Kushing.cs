using System.Collections;
using UnityEngine;

public class Kushing : MonoBehaviour,EffectScripts
{
    public int priority = 0;
    public int Priority => priority;

    public IEnumerator playEffects()
    {
        Destroy(gameObject);
        yield return null;
    }
}
