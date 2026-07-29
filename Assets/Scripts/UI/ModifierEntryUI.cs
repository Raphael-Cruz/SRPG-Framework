using TMPro;
using UnityEngine;

public class ModifierEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryText;
    [SerializeField] private TMP_Text sourceText;
    [SerializeField] private TMP_Text effectText;

public void SetData(
    string category,
    string source,
    string effect)
{
    if (categoryText == null)
        Debug.LogError("Category Text is missing on ModifierEntryUI");

    if (sourceText == null)
        Debug.LogError("Source Text is missing on ModifierEntryUI");

    if (effectText == null)
        Debug.LogError("Effect Text is missing on ModifierEntryUI");


    categoryText.text = category;
    sourceText.text = source;
    effectText.text = effect;
}
}

