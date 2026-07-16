using System.Collections.Generic;


public class CombatResolver
{
    public CombatPrediction Resolve(
        int baseDamage,
        float baseHitChance,
        List<CombatModifier> modifiers
    )
    {
        int finalDamage = baseDamage;
        float finalHitChance = baseHitChance;


        foreach (CombatModifier modifier in modifiers)
        {
            ApplyModifier(
                modifier,
                ref finalDamage,
                ref finalHitChance
            );
        }


        return new CombatPrediction(
            true,
            finalDamage,
            finalHitChance
        );
    }



    private void ApplyModifier(
        CombatModifier modifier,
        ref int damage,
        ref float hitChance
    )
    {
        switch(modifier.Type)
        {
            case CombatModifierType.Damage:
                damage += (int)modifier.Value;
                break;


            case CombatModifierType.HitChance:
                hitChance += modifier.Value;
                break;
        }
    }
}