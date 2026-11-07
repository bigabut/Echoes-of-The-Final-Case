using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CaseSelectUI : MonoBehaviour, EffectScripts
{
    [Serializable]
    public class CaseButton
    {
        public Button button;
        public Case targetCase;
        public string flagOnSelected;
    }

    [SerializeField] private int priority;
    [SerializeField] private CaseManager caseManager;
    [SerializeField] private GameObject panel;
    [SerializeField] private Button closeButton;
    [SerializeField] private CaseButton[] caseButtons;
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;

    private bool isOpen;

    public int Priority => priority;

    private void Awake()
    {
        foreach (CaseButton entry in caseButtons)
        {
            if (entry == null || entry.button == null)
            {
                continue;
            }

            CaseButton captured = entry;
            captured.button.onClick.AddListener(() => Select(captured));
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    public IEnumerator playEffects()
    {
        Refresh();
        panel.SetActive(true);
        isOpen = true;
        onOpen?.Invoke();

        while (isOpen)
        {
            yield return null;
        }
    }

    private void Select(CaseButton entry)
    {
        if (entry.targetCase == null)
        {
            return;
        }

        if (!caseManager.SelectCase(entry.targetCase))
        {
            return;
        }

        StoryProgress.SetFlag(entry.flagOnSelected);
        Close();
    }

    private void Close()
    {
        if (!isOpen)
        {
            return;
        }

        panel.SetActive(false);
        isOpen = false;
        onClose?.Invoke();
    }

    private void Refresh()
    {
        foreach (CaseButton entry in caseButtons)
        {
            if (entry == null || entry.button == null)
            {
                continue;
            }

            entry.button.interactable =
                entry.targetCase != null &&
                entry.targetCase.CurrentState == State.Available &&
                caseManager.ActiveCase == null;
        }
    }
}