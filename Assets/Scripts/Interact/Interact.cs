
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    private bool isPlayerInRange;
    private bool isComplete;
    private bool isInteracting;
    private bool twoWayInteract = true;

    public GameObject UiInteract;

    private void Update()
    {
        if (Dialogue.IsAnyDialogueActive)
        {
            return;
        }

        if (!isPlayerInRange || isComplete || isInteracting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!twoWayInteract)
            {
                isComplete = true;
            }

            StartCoroutine(InteractSequence());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isComplete)
        {
            return;
        }

        isPlayerInRange = true;

        if (UiInteract != null)
        {
            UiInteract.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        isPlayerInRange = false;

        if (UiInteract != null)
        {
            UiInteract.SetActive(false);
        }
    }


    private IEnumerator InteractSequence()
    {
        isInteracting = true;

        if (UiInteract != null)
        {
            UiInteract.SetActive(false);
        }

        var effects = GetComponents<MonoBehaviour>()
            .OfType<EffectScripts>()
            .OrderBy(effect => effect.Priority);

        foreach (EffectScripts effect in effects)
        {
            yield return effect.playEffects();
        }

        Debug.Log("Semua efek selesai.");

        isInteracting = false;

        if (UiInteract != null && isPlayerInRange && !isComplete)
        {
            UiInteract.SetActive(true);
        }
    }

}
