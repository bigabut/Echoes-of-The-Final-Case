using System;
using TMPro;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    [Serializable]
    public class CaseTab
    {
        public Case targetCase;
        public Button tabButton;
        public GameObject[] pages;
    }

    [SerializeField] private GameObject journalPanel;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;
    [SerializeField] private CaseTab[] tabs;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private TMP_Text pageLabel;
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;

    private Animator animator;

    private int currentTab;
    private int currentPage;
    private bool isOpen;

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;

            if (tabs[i] != null && tabs[i].tabButton != null)
            {
                tabs[i].tabButton.onClick.AddListener(() => ShowTab(index));
            }
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }

        if (prevButton != null)
        {
            prevButton.onClick.AddListener(PreviousPage);
        }
        animator = journalPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(toggleKey))
        {
            return;
        }

        if (isOpen)
        {
            Close();
        }
        else if (!Dialogue.IsAnyDialogueActive)
        {
            Open();
        }
    }

    public void Open()
    {
        
        RefreshTabs();
        HideAllPages();
        animator.SetBool("IsOpening", true);
        isOpen = true;
        ShowTab(FindDefaultTab());
        onOpen?.Invoke();
    }

    public void Close()
    {
        animator.SetBool("IsOpening", false);
        isOpen = false;
        onClose?.Invoke();
    }

    private bool IsUnlocked(CaseTab tab)
    {
        if (tab == null || tab.targetCase == null)
        {
            return false;
        }

        State state = tab.targetCase.CurrentState;

        return state == State.InProgress ||
               state == State.Conclusion ||
               state == State.Completed;
    }

    private void RefreshTabs()
    {
        foreach (CaseTab tab in tabs)
        {
            if (tab != null && tab.tabButton != null)
            {
                tab.tabButton.gameObject.SetActive(IsUnlocked(tab));
            }
        }
    }

    private int FindDefaultTab()
    {
        int lastUnlocked = 0;

        for (int i = 0; i < tabs.Length; i++)
        {
            if (!IsUnlocked(tabs[i]))
            {
                continue;
            }

            lastUnlocked = i;

            State state = tabs[i].targetCase.CurrentState;

            if (state == State.InProgress || state == State.Conclusion)
            {
                return i;
            }
        }

        return lastUnlocked;
    }

    private void ShowTab(int index)
    {
        if (index < 0 || index >= tabs.Length)
        {
            return;
        }

        HideAllPages();

        if (!IsUnlocked(tabs[index]))
        {
            SetNavigation(false, false, "");
            return;
        }

        currentTab = index;
        currentPage = 0;
        ShowPage();
    }

    private void ShowPage()
    {
        GameObject[] pages = tabs[currentTab].pages;
        bool hasPages = pages != null && pages.Length > 0;

        if (hasPages)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i] != null)
                {
                    pages[i].SetActive(i == currentPage);
                }
            }
        }

        SetNavigation(
            hasPages && currentPage > 0,
            hasPages && currentPage < pages.Length - 1,
            hasPages ? $"{currentPage + 1}/{pages.Length}" : ""
        );
    }

    private void HideAllPages()
    {
        foreach (CaseTab tab in tabs)
        {
            if (tab == null || tab.pages == null)
            {
                continue;
            }

            foreach (GameObject page in tab.pages)
            {
                if (page != null)
                {
                    page.SetActive(false);
                }
            }
        }
    }

    private void SetNavigation(bool canPrev, bool canNext, string label)
    {
        if (prevButton != null)
        {
            prevButton.interactable = canPrev;
        }

        if (nextButton != null)
        {
            nextButton.interactable = canNext;
        }

        if (pageLabel != null)
        {
            pageLabel.text = label;
        }
    }

    private void NextPage()
    {
        currentPage++;
        ShowPage();
    }

    private void PreviousPage()
    {
        currentPage--;
        ShowPage();
    }
}