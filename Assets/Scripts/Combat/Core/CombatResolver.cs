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
            // Passamos o 'context' para que o modificador possa avaliar o ambiente
            ApplyModifier(
                context, 
                modifier,
                ref finalAttack,
                ref finalDefense,
                ref finalAccuracy,
                ref finalAvoid
            );
        }

        // Temporary calculation for damage range and crit
        int maxDamage = Mathf.Max(finalAttack - finalDefense, 1);
        int minDamage = Mathf.Max(maxDamage - 4, 1); // Variando um pouco o dano, provisório
        int finalCritChance = Mathf.Clamp(context.Crit, 0, 100);

        int finalHitChance = Mathf.Clamp(
            finalAccuracy - finalAvoid,
            0,
            100
        );

        // -------------------------
        // HP Prediction
        // -------------------------

        HPGaugeState attackerGauge =
            new HPGaugeState(
                context.Attacker.CurrentHP,
                context.Attacker.Data.MaxHP,
                0
            );

        HPGaugeState defenderGauge =
            new HPGaugeState(
                context.Defender.CurrentHP,
                context.Defender.Data.MaxHP,
                maxDamage // showing max potential damage
            );

        // -------------------------
        // SP Prediction
        // -------------------------

        SPGaugeState attackerSPGauge =
            new SPGaugeState(
                context.Attacker.CurrentSP,
                context.Attacker.Data.MaxSP,
                0
            );

        SPGaugeState defenderSPGauge =
            new SPGaugeState(
                context.Defender.CurrentSP,
                context.Defender.Data.MaxSP,
                0
            );

        CombatPrediction prediction =
            new CombatPrediction(
                true,
                finalAttack,
                finalDefense,
                minDamage,
                maxDamage,
                finalAccuracy,
                finalAvoid,
                finalHitChance,
                finalCritChance,
                attackerGauge,
                defenderGauge,
                attackerSPGauge,
                defenderSPGauge
            );

        foreach (CombatModifier modifier in modifiers)
        {
            prediction.AddModifier(modifier);
        }

        return prediction;
    }

    private void ApplyModifier(
        CombatContext context,
        CombatModifier modifier,
        ref int attack,
        ref int defense,
        ref int accuracy,
        ref int avoid)
    {
        // O valor base (ou dinâmico, se for um terreno como a Grama à noite) é resolvido aqui
        int modValue = modifier.GetContextualValue(context);

        switch (modifier.Type)
        {
            case CombatModifierType.Attack:
                attack += modValue;
                break;

            case CombatModifierType.Defense:
                defense += modValue;
                break;

            case CombatModifierType.Accuracy:
                accuracy += modValue;
                break;

            case CombatModifierType.Avoid:
                avoid += modValue; 
                break;

            case CombatModifierType.Damage:
                attack += modValue;
                break;

            case CombatModifierType.HitChance:
                accuracy += modValue;
                break;
        }
    }
}