using System;
using System.Collections;
using UnityEngine;

public class CaseConclusion : MonoBehaviour
{
    [Serializable]
    public class ConclusionData
    {
        public Case targetCase;
        public Dialogue dialogue;
        public GameObject journalConclusion;
    }

    [Header("Conclusions")]
    [SerializeField] private ConclusionData[] conclusions;

    public static event Action onConclusion;

    [Header("Timing")]
    [SerializeField] private float delayBeforeMonologue = 2f;

    private bool isPlayingConclusion;

    private void OnEnable()
    {
        if (conclusions == null || conclusions.Length == 0)
        {
            Debug.LogError(
                "[CaseConclusion] Daftar Conclusions belum diisi!",
                this
            );
            return;
        }

        foreach (ConclusionData data in conclusions)
        {
            if (data != null && data.targetCase != null)
            {
                data.targetCase.OnConclusion += PlayConclusion;
            }
        }
    }

    private void OnDisable()
    {
        if (conclusions == null)
        {
            return;
        }

        foreach (ConclusionData data in conclusions)
        {
            if (data != null && data.targetCase != null)
            {
                data.targetCase.OnConclusion -= PlayConclusion;
            }
        }
    }

    private ConclusionData FindActiveConclusion()
    {
        if (conclusions == null)
        {
            return null;
        }

        foreach (ConclusionData data in conclusions)
        {
            if (data != null &&
                data.targetCase != null &&
                data.targetCase.CurrentState == State.Conclusion)
            {
                return data;
            }
        }

        return null;
    }

    private void PlayConclusion()
    {
        if (isPlayingConclusion)
        {
            return;
        }

        ConclusionData data = FindActiveConclusion();

        if (data == null)
        {
            Debug.LogError(
                "[CaseConclusion] Tidak ada Case dengan state Conclusion di daftar!",
                this
            );
            return;
        }

        StartCoroutine(ConclusionSequence(data));
    }

    private IEnumerator ConclusionSequence(ConclusionData data)
    {
        isPlayingConclusion = true;

        if (data.journalConclusion == null)
        {
            Debug.LogError(
                $"[CaseConclusion] Journal Conclusion {data.targetCase.name} belum diisi!",
                this
            );

            isPlayingConclusion = false;
            yield break;
        }

        // Aktifkan kesimpulan di Journal.
        data.journalConclusion.SetActive(true);

        // Panggil notifikasi Journal.
        JournalEvents.NotifyJournalUpdated();

        // Beri jeda sebelum monolog Conclusion.
        yield return new WaitForSeconds(delayBeforeMonologue);

        if (data.dialogue == null)
        {
            Debug.LogError(
                $"[CaseConclusion] Dialogue {data.targetCase.name} belum diisi!",
                this
            );

            isPlayingConclusion = false;
            yield break;
        }

        // Mulai monolog setelah jeda.
        yield return data.dialogue.playEffects();


        onConclusion?.Invoke();

        // Case tetap Conclusion sampai gameplay menyelesaikannya.
        isPlayingConclusion = false;

    }

    public void CompleteCurrentCase()
    {
        ConclusionData data = FindActiveConclusion();

        if (data == null)
        {
            Debug.LogError(
                "[CaseConclusion] Tidak ada Case dengan state Conclusion untuk diselesaikan!",
                this
            );

            return;
        }

        data.targetCase.CompleteCase();
    }
}