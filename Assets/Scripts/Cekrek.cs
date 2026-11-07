
using System.Collections;
using UnityEngine;

public class Cekrek : MonoBehaviour, EffectScripts
{
    [SerializeField] private int priority;
    [SerializeField] private GameObject cekrek;
    private bool isCekrek = false;

    private Animator animator;

    public int Priority => priority;

    private void Awake()
    {
        animator = cekrek.GetComponent<Animator>();
        cekrek.SetActive(false);
    }

    public IEnumerator playEffects()
    {
        play();

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
            && !animator.IsInTransition(0)
        );

        stop();
    }

    private void play()
    {
        if (isCekrek) return;
        
        cekrek.SetActive(true);
        animator.SetTrigger("Cekrek");
        AudioManager.Instance.PlaySFX("Cekrek");
        
    }

    private void stop()
    {
        cekrek.SetActive(false);
    }
}
