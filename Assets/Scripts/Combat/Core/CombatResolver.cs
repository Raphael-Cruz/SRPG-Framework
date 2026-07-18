using System.Collections.Generic;
using UnityEngine;

public class CombatResolver
{
    public CombatPrediction Resolve(
        CombatContext context,
        List<CombatModifier> modifiers)
    {
        int finalAttack = context.Attack;
        int finalDefense = context.Defense;

        int finalAccuracy = context.Accuracy;
        int finalAvoid = context.Avoid;

        foreach (CombatModifier modifier in modifiers)
        {
            ApplyModifier(
                modifier,
                ref finalAttack,
                ref finalDefense,
                ref finalAccuracy,
                ref finalAvoid
            );
        }

        int finalDamage = Mathf.Max(
            finalAttack - finalDefense,
            1
        );

        int finalHitChance = Mathf.Clamp(
            finalAccuracy - finalAvoid,
            0,
            100
        );

        return new CombatPrediction(
            true,
            finalAttack,
            finalDefense,
            finalDamage,
            finalAccuracy,
            finalAvoid,
            finalHitChance
        );
    }

    private void ApplyModifier(
        CombatModifier modifier,
        ref int attack,
        ref int defense,
        ref int accuracy,
        ref int avoid)
    {
        switch (modifier.Type)
        {
            case CombatModifierType.Attack:
                attack += Mathf.RoundToInt(modifier.Value);
                break;

            case CombatModifierType.Defense:
                defense += Mathf.RoundToInt(modifier.Value);
                break;

            case CombatModifierType.Accuracy:
                accuracy += Mathf.RoundToInt(modifier.Value);
                break;

            case CombatModifierType.Avoid:
                avoid += Mathf.RoundToInt(modifier.Value);
                break;

            case CombatModifierType.Damage:
                attack += Mathf.RoundToInt(modifier.Value);
                break;

            case CombatModifierType.HitChance:
                accuracy += Mathf.RoundToInt(modifier.Value);
                break;
        }
    }
}