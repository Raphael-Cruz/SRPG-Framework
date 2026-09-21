using UnityEngine;

public class PositionEvaluator : ICombatEvaluator
{
    public CombatModifier Evaluate(CombatContext context, CombatFacts facts)
    {
        // FLANKING OPOSTO: +75% de dano base (ou multiplicador no resolver), para simplicidade
        // vamos aplicar um bonus flat massivo se o ataque base for pequeno, ou tentar simular um multiplicador.
        // O Modifier do jogo suporta valores absolutos atualmente. Para simular multiplicador,
        // calcularíamos o bonus baseado no contexto.Attack.
        if (facts.Has("IsFlankedOpposite"))
        {
            int extraDamage = Mathf.FloorToInt(context.Attack * 0.75f);
            return new CombatModifier(CombatModifierType.Damage, extraDamage, "Opposite Flank", "x1.75");
        }
        
        // FLANKING LATERAL/DIAGONAL: +25% de dano base
        if (facts.Has("IsFlankedSide"))
        {
            int extraDamage = Mathf.FloorToInt(context.Attack * 0.25f);
            return new CombatModifier(CombatModifierType.Damage, extraDamage, "Side Flank", "x1.25");
        }

        return null;
    }
}

public class BackstabCritEvaluator : ICombatEvaluator
{
    public CombatModifier Evaluate(CombatContext context, CombatFacts facts)
    {
        if (facts.Has("IsBackstabbed"))
        {
            // +30% Crit chance for backstabs
            return new CombatModifier(CombatModifierType.CriticalChance, 30, "Backstab", "+30% CRIT");
        }
        
        return null;
    }
}
