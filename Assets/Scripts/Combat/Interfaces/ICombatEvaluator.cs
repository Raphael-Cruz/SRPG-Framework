public interface ICombatEvaluator
{
    CombatModifier Evaluate(
        CombatContext context,
        CombatFacts facts
    );
}