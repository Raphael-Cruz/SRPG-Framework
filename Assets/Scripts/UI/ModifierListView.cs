using System.Collections.Generic;
using UnityEngine;

public class ModifierListView : MonoBehaviour
{
    [SerializeField] private ModifierEntryUI entryPrefab;
    [SerializeField] private Transform content;

    private readonly List<ModifierEntryUI> entries = new();

    public void Show(IReadOnlyList<CombatModifier> modifiers)
    {
 
        Clear();

        if (modifiers == null || modifiers.Count == 0)
            return;

        foreach (CombatModifier modifier in modifiers)
        {
            ModifierEntryUI entry =
                Instantiate(entryPrefab, content);

            RectTransform rect =
                entry.GetComponent<RectTransform>();

            rect.localPosition = Vector3.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            entry.SetData(
                GetCategory(modifier),
                modifier.Source,
                GetEffect(modifier));

            entries.Add(entry);
        }
    }

    public void Clear()
    {
        foreach (ModifierEntryUI entry in entries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }

        entries.Clear();
    }

    private string GetCategory(CombatModifier modifier)
    {
        return modifier.Type switch
        {
            CombatModifierType.Attack => "Attack",
            CombatModifierType.Defense => "Defense",
            CombatModifierType.Accuracy => "Accuracy",
            CombatModifierType.Avoid => "Avoid",
            CombatModifierType.Damage => "Damage",
            CombatModifierType.HitChance => "Hit Chance",
            CombatModifierType.CriticalChance => "Critical",
            CombatModifierType.Range => "Range",
            _ => "Other"
        };
    }

    private string GetEffect(CombatModifier modifier)
    {
        string sign = modifier.Value > 0 ? "+" : "";

        return modifier.Type switch
        {
            CombatModifierType.Attack =>
                $"{sign}{modifier.Value} Attack",

            CombatModifierType.Defense =>
                $"{sign}{modifier.Value} Defense",

            CombatModifierType.Accuracy =>
                $"{sign}{modifier.Value} Accuracy",

            CombatModifierType.Avoid =>
                $"{sign}{modifier.Value} Avoid",

            CombatModifierType.Damage =>
                $"{sign}{modifier.Value} Damage",

            CombatModifierType.HitChance =>
                $"{sign}{modifier.Value}% Hit",

            CombatModifierType.CriticalChance =>
                $"{sign}{modifier.Value}% Crit",

            CombatModifierType.Range =>
                $"{sign}{modifier.Value} Range",

            _ =>
                $"{sign}{modifier.Value}"
        };
    }
}