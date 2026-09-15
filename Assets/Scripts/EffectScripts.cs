using System.Collections;

public interface EffectScripts
{
    int Priority { get; }
    IEnumerator PlayEffects();
}