using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Interact : MonoBehaviour
{
    private bool isPlayerInRange = false;
    private bool isComplete = false;
    private bool twoWayInteract = true;
    public Image UiInteract;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && !isComplete)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(!twoWayInteract)
                {
                    isComplete = true;
                }
                StartCoroutine(interact());
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !isComplete)
        {
            isPlayerInRange = true;
            UiInteract.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") )
        {
            isPlayerInRange = false;
            UiInteract.gameObject.SetActive(false);
        }
    }

    IEnumerator interact()
    {
        var effects = GetComponents<MonoBehaviour>()
            .OfType<EffectScripts>()
            .OrderBy(e => e.Priority);

        foreach (var effect in effects)
        {
            yield return effect.playEffects();
        }

        Debug.Log("semua efek selesai");
    }
}
