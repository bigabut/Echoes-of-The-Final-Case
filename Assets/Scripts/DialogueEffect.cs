using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Dialogue))]
public class DialogueEffect : MonoBehaviour, EffectScripts
{
    [SerializeField] private int priority = 0;

    private Dialogue dialogueComponent;

    public int Priority => priority;

    private void Awake()
    {
        dialogueComponent = GetComponent<Dialogue>();
    }

    public IEnumerator PlayEffects()
    {
        if (dialogueComponent == null) yield break;

        dialogueComponent.StartDialogue();

        // Tunggu hingga percakapan selesai sebelum mengeksekusi efek berikutnya
        while (dialogueComponent.IsDialogueActive)
        {
            yield return null;
        }
    }
}