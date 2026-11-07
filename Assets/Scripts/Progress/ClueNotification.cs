using UnityEngine;

public class ClueNotification : MonoBehaviour
{

    [SerializeField] private Animator Animator;
    

    private void OnEnable()
    {
        JournalEvents.OnJournalUpdated += playerNotification;
        CaseConclusion.onConclusion += playerNotification;
        
    }

    private void OnDisable()
    {
        JournalEvents.OnJournalUpdated -= playerNotification;
        CaseConclusion.onConclusion -= playerNotification;

    }

    private void playerNotification()
    {
        Animator.SetTrigger("Notif");
    }
}
