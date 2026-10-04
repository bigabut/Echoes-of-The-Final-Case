using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour, EffectScripts
{
    public DialogueSystem dialogueData;

    // Panel + text tiap speaker. Index-nya HARUS cocok dengan
    // DialogueLine.speakerIndex di DialogueSystem.
    // Contoh: speakerPanels[0] = panel Player, speakerPanels[1] = panel NPC.
    public GameObject[] speakerPanels;
    public TMP_Text[] speakerTexts;

    public int priority = 0;
    public int Priority => priority;

    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;

    public IEnumerator playEffects()
    {
        if (isDialogueActive)
        {
            Debug.Log("NEXT LINE");
            nextLine();
        }
        else if (speakerPanels != null && speakerPanels.Length > 0 && !isDialogueActive)
        {
            Debug.Log("START DIALOGUE");
            startDialogue();
        }

        while (isDialogueActive)
            yield return null;
    }

    void startDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        StartCoroutine(typeline());
    }

    void nextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            DialogueLine current = dialogueData.lines[dialogueIndex];
            speakerTexts[current.speakerIndex].SetText(current.text);
            isTyping = false;
        }
        else if (dialogueIndex + 1 < dialogueData.lines.Length)
        {
            dialogueIndex++;
            StartCoroutine(typeline());
        }
        else
        {
            endDialogue();
        }
    }

    void endDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;

        // Matiin semua panel speaker pas dialog selesai
        foreach (var panel in speakerPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    // Nyalain panel milik speaker yang lagi ngomong, matiin sisanya.
    // Ini bagian yang bikin efek "gantian" antara Player/NPC/dst.
    void ShowSpeaker(int speakerIndex)
    {
        for (int i = 0; i < speakerPanels.Length; i++)
        {
            if (speakerPanels[i] == null) continue;
            speakerPanels[i].SetActive(i == speakerIndex);
        }
    }

    IEnumerator typeline()
    {
        isTyping = true;

        DialogueLine current = dialogueData.lines[dialogueIndex];

        // Switch panel ke speaker baris ini SEBELUM mulai ngetik.
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
            nextLine();
        }
    }
}