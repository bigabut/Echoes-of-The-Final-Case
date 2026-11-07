using System;
using System.Collections;
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
        if (isFounded)
        {
            Debug.Log($"[Clue] {name} sudah ditemukan sebelumnya.", this);
            return;
        }

        if (clue == null)
        {
            Debug.LogError($"[Clue] {name}: GameObject clue belum diisi di Inspector!", this);
            return;
        }

        clue.SetActive(true);
        isFounded = true;

        Debug.Log($"[Clue] {name} ditemukan! IsFounded = {isFounded}", this);

        JournalEvents.NotifyJournalUpdated();
        Debug.Log($"[Clue] {name} mengirim JournalUpdated.", this);

        onClueFounded?.Invoke(this);
        Debug.Log($"[Clue] {name} mengirim event onClueFounded.", this);
    }
}