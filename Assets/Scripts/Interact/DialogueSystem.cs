using UnityEngine;

// Satu baris dialog: teksnya apa, siapa yang ngomong (index ke speakerPanels
// di Dialogue.cs), dan apakah otomatis lanjut ke baris berikutnya.
[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)]
    public string text;

    // Index speaker untuk baris ini. Harus match sama urutan
    // speakerPanels/speakerTexts di komponen Dialogue.
    // Contoh: 0 = Player, 1 = NPC.
    public int speakerIndex;

    public bool autoProgress;
}

[CreateAssetMenu]
public class DialogueSystem : ScriptableObject
{
    public DialogueLine[] lines;

    public float autoProgressDelay = 1.5f;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
}