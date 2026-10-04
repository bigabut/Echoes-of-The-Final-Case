using System.Collections;
using UnityEngine;

public class CaseConclusion : MonoBehaviour
{
    [SerializeField] private Case currentCase;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private GameObject journalConclusion;


    private void OnEnable()
    {
        currentCase.OnConclusion += PlayConclusion;
    }

    private void OnDisable()
    {
        currentCase.OnConclusion -= PlayConclusion;
    }


    private void PlayConclusion()
    {
        StartCoroutine(ConclusionSequence());
    }


    private IEnumerator ConclusionSequence()
    {
        yield return dialogue.playEffects();

        journalConclusion.SetActive(true);

        JournalEvents.NotifyJournalUpdated();

        currentCase.CompleteCase();
    }
}