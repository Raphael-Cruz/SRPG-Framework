using System.Collections.Generic;

public class AIActionGenerator
{
    private readonly AttackRangeCalculator rangeCalculator;
    private readonly AttackTargetSelector targetSelector;
    private readonly MovementRangeCalculator movementCalculator;
    private readonly CombatSimulator simulator;
    private readonly ThreatMapCalculator threatMapCalculator;

    public AIActionGenerator(
        AttackRangeCalculator rangeCalculator,
        AttackTargetSelector targetSelector,
        MovementRangeCalculator movementCalculator,
        CombatSimulator simulator,
        ThreatMapCalculator threatMapCalculator)
    {
        this.rangeCalculator = rangeCalculator;
        this.targetSelector = targetSelector;
        this.movementCalculator = movementCalculator;
        this.simulator = simulator;
        this.threatMapCalculator = threatMapCalculator;
    }

    public List<IAIAction> GenerateActions(Unit unit, IReadOnlyList<Unit> enemies)
    {
        List<IAIAction> actions = new List<IAIAction>();

        Dictionary<GridTile, int> threatMap =
            threatMapCalculator.GetThreatMap(enemies);

        actions.AddRange(GenerateAttackActions(unit));
        GenerateMoveCandidates(unit, threatMap, actions);
        actions.Add(GenerateWaitAction(unit, threatMap));

        return actions;
    }

    // Exposed separately so AITurnController can re-run just this half
    // after a unit has moved but can still act this turn.
    public List<IAIAction> GenerateAttackActions(Unit unit)
    {
        List<IAIAction> actions = new List<IAIAction>();

        List<GridTile> attackTiles =
            rangeCalculator.CalculateRange(unit);

        targetSelector.BuildTargetList(unit, attackTiles);

        foreach (Unit target in targetSelector.ValidTargets)
        {
            actions.Add(new AttackAction(unit, target, simulator));
        }

        return actions;
    }

    // Same as GenerateAttackActions, but includes an explicit Wait
    // candidate so a follow-up attack pass can decline to attack if
    // every option scores worse than doing nothing.
    public List<IAIAction> GenerateAttackActionsWithWait(
        Unit unit,
        IReadOnlyList<Unit> enemies)
    {
        List<IAIAction> actions = GenerateAttackActions(unit);

        Dictionary<GridTile, int> threatMap =
            threatMapCalculator.GetThreatMap(enemies);

        actions.Add(GenerateWaitAction(unit, threatMap));

        return actions;
    }

    private WaitAction GenerateWaitAction(
        Unit unit,
        Dictionary<GridTile, int> threatMap)
    {
        threatMap.TryGetValue(unit.CurrentTile, out int currentThreat);

        return new WaitAction(unit, currentThreat);
    }

    // A move is only worth proposing if it opens an attack that isn't
    // available from the current tile, or if it's strictly safer than
    // staying put.
    private void GenerateMoveCandidates(
        Unit unit,
        Dictionary<GridTile, int> threatMap,
        List<IAIAction> actions)
    {
        Dictionary<GridTile, int> reachableTiles =
            movementCalculator.CalculateRange(unit);

        threatMap.TryGetValue(unit.CurrentTile, out int currentThreat);

        foreach (KeyValuePair<GridTile, int> entry in reachableTiles)
        {
            GridTile destination = entry.Key;
            int cost = entry.Value;

            unit.SetPreviewTile(destination);

            List<GridTile> hypotheticalAttackTiles =
                rangeCalculator.CalculateRange(unit);

            targetSelector.BuildTargetList(unit, hypotheticalAttackTiles);

            CombatPrediction bestPrediction =
                FindBestPrediction(unit, targetSelector.ValidTargets);

            unit.ClearPreviewTile();

            threatMap.TryGetValue(destination, out int destinationThreat);

            bool opensAttack = bestPrediction != null;
            bool isSafer = destinationThreat < currentThreat;

            if (opensAttack || isSafer)
            {
                actions.Add(
                    new MoveAction(
                        unit,
                        destination,
                        cost,
                        bestPrediction,
                        destinationThreat
                    )
                );
            }
        }
    }

    private CombatPrediction FindBestPrediction(
        Unit attacker,
        IReadOnlyList<Unit> targets)
    {
        CombatPrediction best = null;

        foreach (Unit target in targets)
        {
            CombatContext context =
                new CombatContext(
                    attacker,
                    target,
                    attacker.Data.Attack,
                    target.Data.Defense,
                    attacker.Data.Accuracy,
                    target.Data.Avoid
                );

            CombatPrediction prediction = simulator.Simulate(context);

            if (best == null || prediction.Damage > best.Damage)
            {
                best = prediction;
            }
        }

        return best;
    }
}