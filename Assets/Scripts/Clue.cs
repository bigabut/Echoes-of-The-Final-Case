using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Clue : MonoBehaviour, EffectScripts
{
    public int priority = 0;
    public int Priority => priority;

    
    public static event Action<Clue> onClueFounded;
    [SerializeField] private GameObject clue;
    [SerializeField] private bool isFounded;

    public bool IsFounded => isFounded;



    public IEnumerator playEffects()
    {
        nambahClue();
        yield return null;
    }

    public void nambahClue()
    {
        if (isFounded) return;
        
        
        clue.SetActive(true);

        isFounded = true;

        JournalEvents.NotifyJournalUpdated();
        onClueFounded?.Invoke(this);
        
    }
}
