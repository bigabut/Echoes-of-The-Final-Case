using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject target;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private float typeSpeed = 0.03f;

    private Coroutine typingRoutine;

    private void Awake()
    {
        if (targetText == null && target != null)
        {
            targetText = target.GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void OnEnable()
    {
        Hide();
    }

    private void OnDisable()
    {
        Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }

    private void Show()
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(true);

        if (targetText == null)
        {
            return;
        }

        StopTyping();
        targetText.maxVisibleCharacters = 0;
        typingRoutine = StartCoroutine(TypeText());
    }

    private void Hide()
    {
        StopTyping();

        if (target != null)
        {
            target.SetActive(false);
        }
    }

    private void StopTyping()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }
    }

    private IEnumerator TypeText()
    {
        targetText.ForceMeshUpdate();
        int total = targetText.textInfo.characterCount;

        for (int i = 1; i <= total; i++)
        {
            targetText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        typingRoutine = null;
    }
}