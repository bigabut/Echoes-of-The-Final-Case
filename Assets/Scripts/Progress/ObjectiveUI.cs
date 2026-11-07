using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;

    private void OnEnable()
    {
        StoryProgress.OnObjectiveChanged += UpdateText;
        UpdateText(StoryProgress.CurrentObjective);
    }

    private void OnDisable()
    {
        StoryProgress.OnObjectiveChanged -= UpdateText;
    }

    private void UpdateText(string objective)
    {
        objectiveText.text = objective;
    }
}