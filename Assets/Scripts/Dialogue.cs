using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Dialogue Settings")]
    [SerializeField] private string speakerName;
    [TextArea(3, 5)]
    [SerializeField] private List<string> sentences = new List<string>();
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentSentenceIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private PlayerMovement2D playerMovement;

    public bool IsDialogueActive { get; private set; }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void StartDialogue()
    {
        if (sentences.Count == 0) return;

        IsDialogueActive = true;
        currentSentenceIndex = 0;

        // Kunci Pergerakan Player
        playerMovement = FindObjectOfType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            playerMovement.CanMove = false;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (nameText != null)
            nameText.text = speakerName;

        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
    }

    public void OnInteractPressed()
    {
        // Jika teks sedang diketik, langsung tampilkan seluruh kalimat (Skip Effect)
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = sentences[currentSentenceIndex];
            isTyping = false;
            return;
        }

        // Lanjut ke kalimat berikutnya
        currentSentenceIndex++;

        if (currentSentenceIndex < sentences.Count)
        {
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        IsDialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Buka Kembali Pergerakan Player
        if (playerMovement != null)
        {
            playerMovement.CanMove = true;
        }
    }
}