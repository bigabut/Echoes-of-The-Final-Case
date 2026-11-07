using System;
using UnityEngine;

public enum State
{
    Locked,
    Available,
    InProgress,
    Conclusion,
    Completed
}

public class Case : MonoBehaviour
{
    private State currentState = State.Locked;

    public State CurrentState => currentState;

    public event Action OnConclusion;
    public event Action<Case> OnCompleted;

    [SerializeField] private Clue[] clues;


    private void Awake()
    {
        Debug.Log($"[Case] {name} dibuat. State awal: {currentState}", this);
    }


    private void OnEnable()
    {
        Clue.onClueFounded += CheckCompletion;
        Debug.Log($"[Case] {name} berlangganan event onClueFounded.", this);
    }


    private void OnDisable()
    {
        Clue.onClueFounded -= CheckCompletion;
        Debug.Log($"[Case] {name} berhenti berlangganan event onClueFounded.", this);
    }

    [ContextMenu("DEBUG: Start Case")]
    private void DebugStartCase()
    {
        SetAvailable();
        StartCase();

        Debug.Log($"[DEBUG] {name} sekarang {CurrentState}", this);
    }

    public void SetLocked()
    {
        ChangeState(State.Locked);
    }


    public void SetAvailable()
    {
        ChangeState(State.Available);
    }


    public void StartCase()
    {
        if (currentState != State.Available)
        {
            Debug.LogWarning(
                $"[Case] {name} tidak bisa dimulai. State sekarang: {currentState}",
                this
            );
            return;
        }

        ChangeState(State.InProgress);
    }


    private void CheckCompletion(Clue clueFounded)
    {
        Debug.Log(
            $"[Case] {name} menerima event dari clue {clueFounded.name}. State: {currentState}",
            this
        );

        if (currentState != State.InProgress)
        {
            Debug.Log($"[Case] {name} diabaikan karena belum InProgress.", this);
            return;
        }

        if (clues == null || clues.Length == 0)
        {
            Debug.LogWarning($"[Case] {name} belum memiliki daftar clues!", this);
            return;
        }

        bool belongsToCase = false;

        for (int i = 0; i < clues.Length; i++)
        {
            if (clues[i] == clueFounded)
            {
                belongsToCase = true;
                break;
            }
        }

        if (!belongsToCase)
        {
            Debug.Log($"[Case] {clueFounded.name} bukan clue milik {name}.", this);
            return;
        }

        for (int i = 0; i < clues.Length; i++)
        {
            if (clues[i] == null)
            {
                Debug.LogWarning(
                    $"[Case] {name} memiliki slot clue kosong pada indeks {i}!",
                    this
                );
                return;
            }

            if (!clues[i].IsFounded)
            {
                Debug.Log(
                    $"[Case] {name} belum selesai. Clue yang belum ditemukan: {clues[i].name}",
                    this
                );
                return;
            }
        }

        Debug.Log($"[Case] SEMUA CLUE {name} SUDAH DITEMUKAN!", this);

        ChangeState(State.Conclusion);
        Conclusion();
    }


    private void Conclusion()
    {
        if (currentState != State.Conclusion)
        {
            return;
        }

        Debug.Log($"[Case] {name} MASUK STATE CONCLUSION!", this);

        OnConclusion?.Invoke();
        Debug.Log($"[Case] {name} mengirim event OnConclusion.", this);
    }


    public void CompleteCase()
    {
        if (currentState != State.Conclusion)
        {
            Debug.LogWarning(
                $"[Case] {name} tidak bisa Completed. State sekarang: {currentState}",
                this
            );
            return;
        }

        ChangeState(State.Completed);

        Debug.Log($"[Case] {name} SELESAI! State: {currentState}", this);

        OnCompleted?.Invoke(this);
    }


    private void ChangeState(State state)
    {
        if (currentState == state)
        {
            Debug.Log($"[Case] {name} sudah berada di state {state}.", this);
            return;
        }

        State previousState = currentState;
        currentState = state;

        Debug.Log(
            $"[Case] {name}: {previousState} → {currentState}",
            this
        );
    }
}