using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactor2D : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup interactionUI;
    [SerializeField] private TMP_Text interactionText;

    public IInteractable currentInteractable;
    private readonly List<IInteractable> availableInteractables = new List<IInteractable>();
    private Dialogue currentDialogue;

    private void Awake()
    {
        HideInteractionUI();
    }

    private void Update()
    {
        UpdateCurrentInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    public void RegisterInteractable(IInteractable interactable)
    {
        if (!availableInteractables.Contains(interactable))
        {
            availableInteractables.Add(interactable);
        }
    }

    public void UnregisterInteractable(IInteractable interactable)
    {
        if (availableInteractables.Contains(interactable))
        {
            availableInteractables.Remove(interactable);
        }
    }

    private void UpdateCurrentInteractable()
    {
        // Sembunyikan UI saat dialog berlangsung
        if (currentDialogue != null && currentDialogue.IsDialogueActive)
        {
            HideInteractionUI();
            return;
        }

        // Ambil objek interaktif valid yang paling akhir dimasuki
        IInteractable validInteractable = null;
        for (int i = availableInteractables.Count - 1; i >= 0; i--)
        {
            if (availableInteractables[i].CanInteract())
            {
                validInteractable = availableInteractables[i];
                break;
            }
        }

        currentInteractable = validInteractable;

        if (currentInteractable != null)
        {
            ShowInteractionUI(currentInteractable.InteractionText);
        }
        else
        {
            HideInteractionUI();
        }
    }

    public void Interact()
    {
        if (currentDialogue != null && currentDialogue.IsDialogueActive)
        {
            currentDialogue.OnInteractPressed();
            HideInteractionUI();
            return;
        }

        if (currentInteractable == null) return;

        MonoBehaviour interactableObject = currentInteractable as MonoBehaviour;
        if (interactableObject != null)
        {
            Dialogue dialogue = interactableObject.GetComponentInParent<Dialogue>();
            if (dialogue != null)
            {
                currentDialogue = dialogue;
                HideInteractionUI();
            }
        }

        currentInteractable.Interact();
    }

    private void ShowInteractionUI(string text)
    {
        if (interactionUI == null) return;
        interactionUI.alpha = 1f;
        if (interactionText != null) interactionText.text = text;
    }

    private void HideInteractionUI()
    {
        if (interactionUI == null) return;
        interactionUI.alpha = 0f;
    }
}