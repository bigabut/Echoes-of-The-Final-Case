using System.Collections;
using System.ComponentModel;
using UnityEngine;

public interface EffectScripts 
{
    int Priority { get; }
    public IEnumerator playEffects();


    

}
