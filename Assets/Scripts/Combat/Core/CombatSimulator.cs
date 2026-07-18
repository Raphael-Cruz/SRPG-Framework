using System.Collections.Generic;

public class CombatSimulator
{
    private readonly List<ICombatFactProvider> factProviders;
    private readonly List<ICombatEvaluator> evaluators;

    private readonly CombatResolver resolver;

    public CombatSimulator(
        List<ICombatFactProvider> factProviders,
        List<ICombatEvaluator> evaluators,
        CombatResolver resolver)
    {
        this.factProviders = factProviders;
        this.evaluators = evaluators;
        this.resolver = resolver;
    }

    public CombatPrediction Simulate(
        CombatContext context)
    {
        CombatFacts facts = new CombatFacts();

        // Step 1:
        // Gather information about the battlefield.
        foreach (ICombatFactProvider provider in factProviders)
        {
            provider.ProvideFacts(
                context,
                facts
            );
        }

        // Step 2:
        // Convert facts into gameplay modifiers.
        List<CombatModifier> modifiers =
            new List<CombatModifier>();

        foreach (ICombatEvaluator evaluator in evaluators)
        {
            CombatModifier modifier =
                evaluator.Evaluate(
                    context,
                    facts
                );

            if (modifier != null)
            {
                modifiers.Add(modifier);
            }
        }

        // Step 3:
        // Produce the predicted outcome.
        return resolver.Resolve(
            context,
            modifiers
        );
    }
}