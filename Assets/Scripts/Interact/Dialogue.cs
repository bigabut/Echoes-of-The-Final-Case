
using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour, EffectScripts
{
    public DialogueSystem dialogueData;

    public GameObject[] speakerPanels;
    public TMP_Text[] speakerTexts;

    public int priority = 0;
    public int Priority => priority;

    public bool IsDialogueActive => isDialogueActive;

    public static Dialogue ActiveDialogue { get; private set; }

    public static bool IsAnyDialogueActive =>
        ActiveDialogue != null && ActiveDialogue.isDialogueActive;

    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;

    private Coroutine typingCoroutine;

    [Header("Dialogue Animation")]
    [SerializeField] private Animator dialogueAnimator;

    

    private void Update()
    {
        if (ActiveDialogue != this || !isDialogueActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            AdvanceDialogue();
        }
    }

    
    public IEnumerator playEffects()
    {
        // Tunggu dialogue lain selesai agar tidak ada dua dialogue bersamaan.
        while (ActiveDialogue != null && ActiveDialogue != this)
        {
            yield return null;
        }

        // Jangan mulai ulang atau memajukan dialogue yang sudah berjalan.
        if (!isDialogueActive)
        {
            StartDialogue();
        }

        while (isDialogueActive)
        {
            yield return null;
        }
    }

    private void StartDialogue()
    {
        if (dialogueData == null ||
            dialogueData.lines == null ||
            dialogueData.lines.Length == 0)
        {
            Debug.LogWarning("[Dialogue] Data dialogue kosong!", this);
            return;
        }

        if (speakerPanels == null || speakerTexts == null)
        {
            Debug.LogError("[Dialogue] Speaker panels/texts belum diisi!", this);
            return;
        }

        ActiveDialogue = this;
        isDialogueActive = true;
        dialogueIndex = 0;


        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetBool("IsDialogue", true);
        }

        typingCoroutine = StartCoroutine(TypeLine());
    }

    private void AdvanceDialogue()
    {
        if (!isDialogueActive)
        {
            return;
        }

        // Tekan F saat mengetik untuk langsung menampilkan seluruh teks.
        if (isTyping)
        {
            StopTypingCoroutine();

            DialogueLine current = dialogueData.lines[dialogueIndex];

            if (IsValidSpeaker(current.speakerIndex))
            {
                speakerTexts[current.speakerIndex].SetText(current.text);
            }

            isTyping = false;
            return;
        }

        // Hentikan waktu tunggu auto-progress jika player menekan F.
        StopTypingCoroutine();

        if (dialogueIndex + 1 < dialogueData.lines.Length)
        {
            dialogueIndex++;
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        if (dialogueData == null ||
            dialogueData.lines == null ||
            dialogueIndex < 0 ||
            dialogueIndex >= dialogueData.lines.Length)
        {
            EndDialogue();
            yield break;
        }

        DialogueLine current = dialogueData.lines[dialogueIndex];

        if (!IsValidSpeaker(current.speakerIndex))
        {
            Debug.LogError(
                $"[Dialogue] Speaker index {current.speakerIndex} tidak valid!",
                this
            );

            EndDialogue();
            yield break;
        }

        isTyping = true;

        ShowSpeaker(current.speakerIndex);

        TMP_Text targetText = speakerTexts[current.speakerIndex];
        targetText.SetText("");

        foreach (char letter in current.text)
        {
            targetText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (current.autoProgress)
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);

            if (isDialogueActive &&
                !isTyping &&
                dialogueIndex >= 0 &&
                dialogueIndex < dialogueData.lines.Length)
            {
                AdvanceDialogue();
            }
        }
    }

    private void StopTypingCoroutine()
    {
        if (typingCoroutine == null)
        {
            return;
        }

        StopCoroutine(typingCoroutine);
        typingCoroutine = null;
    }

    private bool IsValidSpeaker(int speakerIndex)
    {
        return speakerIndex >= 0 &&
               speakerIndex < speakerPanels.Length &&
               speakerIndex < speakerTexts.Length &&
               speakerPanels[speakerIndex] != null &&
               speakerTexts[speakerIndex] != null;
    }

    private void ShowSpeaker(int speakerIndex)
    {
        for (int i = 0; i < speakerPanels.Length; i++)
        {
            if (speakerPanels[i] != null)
            {
                speakerPanels[i].SetActive(i == speakerIndex);
            }
        }
    }

    private void EndDialogue()
    {
        StopTypingCoroutine();

        isTyping = false;
        isDialogueActive = false;

        if (ActiveDialogue == this)
        {
            ActiveDialogue = null;
        }

        if (speakerPanels == null)
        {
            return;
        }

        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetBool("IsDialogue", false);
        }

        foreach (GameObject panel in speakerPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    private void OnDisable()
    {
        if (isDialogueActive)
        {
            EndDialogue();
        }
    }
}