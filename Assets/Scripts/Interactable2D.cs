using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable2D : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string interactionText = "[E] Interact";
    [SerializeField] private bool interactOnce = false;

    private bool isComplete = false;
    private bool isInteracting = false;

    public string InteractionText => interactionText;

    public bool CanInteract()
    {
        return !isComplete && !isInteracting;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        StartCoroutine(PlayEffects());
    }

    private IEnumerator PlayEffects()
    {
        isInteracting = true;

        var effects = GetComponents<MonoBehaviour>()
            .OfType<EffectScripts>()
            .OrderBy(effect => effect.Priority);

        foreach (var effect in effects)
        {
            yield return effect.PlayEffects();
        }

        if (interactOnce)
        {
            isComplete = true;
        }

        isInteracting = false;
    }

    // Menginformasikan ke Interactor saat player masuk/keluar area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Interactor2D>(out var interactor))
        {
            interactor.RegisterInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Interactor2D>(out var interactor))
        {
            interactor.UnregisterInteractable(this);
        }
    }
}