using UnityEngine;

public class TestModifierEvaluator : ICombatEvaluator
{
  public CombatModifier Evaluate(
    CombatContext context,
    CombatFacts facts)
{
    // Debug.Log("TestModifierEvaluator executed");

    return new CombatModifier(
        CombatModifierType.HitChance,
        -15,
        "Forest Terrain"
    );
}
}

public class TestFlankingEvaluator : ICombatEvaluator
{
    public CombatModifier Evaluate(
        CombatContext context,
        CombatFacts facts)
    {
        return new CombatModifier(
            CombatModifierType.CriticalChance,
            100,
            "Flanking"
        );
    }
}