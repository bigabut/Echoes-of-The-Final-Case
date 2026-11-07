using System;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
    [Header("Cases")]
    [SerializeField] private Case[] cases = new Case[5];

    private Case activeCase;

    public Case ActiveCase => activeCase;

    private void Awake()
    {
        if (!ValidateCases())
        {
            enabled = false;
            return;
        }

        // Semua Case dimulai dalam keadaan Locked.
        foreach (Case currentCase in cases)
        {
            currentCase.SetLocked();
            currentCase.OnCompleted += HandleCaseCompleted;
        }

        // Hanya Case 1 yang tersedia pada awal permainan.
        cases[0].SetAvailable();
    }

    private void OnDestroy()
    {
        if (cases == null)
        {
            return;
        }

        foreach (Case currentCase in cases)
        {
            if (currentCase != null)
            {
                currentCase.OnCompleted -= HandleCaseCompleted;
            }
        }
    }

    public bool SelectCase(Case selectedCase)
    {
        if (selectedCase == null)
        {
            Debug.LogWarning("[CaseManager] Case yang dipilih kosong!", this);
            return false;
        }

        if (Array.IndexOf(cases, selectedCase) < 0)
        {
            Debug.LogWarning(
                "[CaseManager] Case ini tidak terdaftar di CaseManager!",
                this
            );
            return false;
        }

        if (activeCase != null)
        {
            Debug.LogWarning(
                $"[CaseManager] Selesaikan {activeCase.name} terlebih dahulu!",
                this
            );
            return false;
        }

        if (selectedCase.CurrentState != State.Available)
        {
            Debug.LogWarning(
                $"[CaseManager] {selectedCase.name} belum tersedia!",
                this
            );
            return false;
        }

        selectedCase.StartCase();
        activeCase = selectedCase;

        Debug.Log(
            $"[CaseManager] Memulai {selectedCase.name}.",
            this
        );

        return true;
    }

    private void HandleCaseCompleted(Case completedCase)
    {
        if (activeCase == completedCase)
        {
            activeCase = null;
        }

        int caseIndex = Array.IndexOf(cases, completedCase);

        Debug.Log(
            $"[CaseManager] {completedCase.name} selesai!",
            this
        );

        switch (caseIndex)
        {
            case 0:
                // Case 1 selesai: buka Case 2 dan 3.
                UnlockCase(1);
                UnlockCase(2);
                break;

            case 1:
            case 2:
                // Case 2 dan 3 harus sama-sama selesai
                // sebelum Case 4 dibuka.
                if (cases[1].CurrentState == State.Completed &&
                    cases[2].CurrentState == State.Completed)
                {
                    UnlockCase(3);
                }
                break;

            case 3:
                // Case 4 selesai: buka Case 5.
                UnlockCase(4);
                break;

            case 4:
                Debug.Log(
                    "[CaseManager] Semua Case utama telah selesai!",
                    this
                );
                break;
        }
    }

    private void UnlockCase(int index)
    {
        if (index < 0 || index >= cases.Length)
        {
            return;
        }

        if (cases[index].CurrentState != State.Locked)
        {
            return;
        }

        if (!cases[index].gameObject.activeSelf)
        {
            cases[index].gameObject.SetActive(true);
        }

        cases[index].SetAvailable();

        Debug.Log(
            $"[CaseManager] {cases[index].name} sekarang Available!",
            this
        );
    }

    private bool ValidateCases()
    {
        if (cases == null || cases.Length != 5)
        {
            Debug.LogError(
                "[CaseManager] Harus ada tepat 5 Case di Inspector!",
                this
            );
            return false;
        }

        for (int i = 0; i < cases.Length; i++)
        {
            if (cases[i] == null)
            {
                Debug.LogError(
                    $"[CaseManager] Slot Case {i + 1} belum diisi!",
                    this
                );
                return false;
            }

            for (int j = i + 1; j < cases.Length; j++)
            {
                if (cases[i] == cases[j])
                {
                    Debug.LogError(
                        "[CaseManager] Ada Case yang dimasukkan lebih dari sekali!",
                        this
                    );
                    return false;
                }
            }
        }

        return true;
    }
}