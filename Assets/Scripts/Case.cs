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
    private State currentState;

    public State CurrentState => currentState;

    public event Action OnConclusion;
    public event Action OnCompleted;

    [SerializeField] private Clue[] clues;


    private void OnEnable()
    {
        Clue.onClueFounded += CheckCompletion;
    }

    private void OnDisable()
    {
        Clue.onClueFounded -= CheckCompletion;
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
            return;
        }

        ChangeState(State.InProgress);
    }


    private void CheckCompletion(Clue clueFounded)
    {
        if (currentState != State.InProgress)
        {
            return;
        }

        for (int i = 0; i < clues.Length; i++)
        {
            if (!clues[i].IsFounded)
            {
                return;
            }
        }

        ChangeState(State.Conclusion);
        Conclusion();
    }


    private void Conclusion()
    {
        if (currentState != State.Conclusion)
        {
            return;
        }

        OnConclusion?.Invoke();
    }


    public void CompleteCase()
    {
        if (currentState != State.Conclusion)
        {
            return;
        }

        ChangeState(State.Completed);
        OnCompleted?.Invoke();
    }


    private void ChangeState(State state)
    {
        currentState = state;
    }
}