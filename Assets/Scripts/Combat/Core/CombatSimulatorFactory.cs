using System.Collections.Generic;

// Single source of truth for "the standard combat rule set". Both the
// player-facing preview and the AI's evaluation must build their
// CombatSimulator from here, so a prediction never differs depending on
// who's asking.
public static class CombatSimulatorFactory
{
    public static CombatSimulator CreateStandard()
    {
        return new CombatSimulator(
            new List<ICombatFactProvider>(),
            new List<ICombatEvaluator>
            {
                new TestModifierEvaluator(),
                new TestFlankingEvaluator()
            },
            new CombatResolver()
        );
    }
}