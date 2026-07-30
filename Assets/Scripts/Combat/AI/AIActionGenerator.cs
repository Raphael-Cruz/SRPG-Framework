using System.Collections.Generic;

public class AIActionGenerator
{
    private readonly AttackRangeCalculator rangeCalculator;
    private readonly AttackTargetSelector targetSelector;
    private readonly CombatSimulator simulator;

    public AIActionGenerator(
        AttackRangeCalculator rangeCalculator,
        AttackTargetSelector targetSelector,
        CombatSimulator simulator)
    {
        this.rangeCalculator = rangeCalculator;
        this.targetSelector = targetSelector;
        this.simulator = simulator;
    }

    public List<IAIAction> GenerateActions(Unit unit)
    {
        List<IAIAction> actions = new List<IAIAction>();

        List<GridTile> attackTiles =
            rangeCalculator.CalculateRange(unit);

        targetSelector.BuildTargetList(unit, attackTiles);

        foreach (Unit target in targetSelector.ValidTargets)
        {
            actions.Add(
                new AttackAction(unit, target, simulator)
            );
        }

        return actions;
    }
}