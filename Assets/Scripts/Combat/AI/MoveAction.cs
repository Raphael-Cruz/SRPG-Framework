public class MoveAction : IAIAction
{
    public Unit Actor { get; }

    public GridTile Destination { get; }

    private readonly System.Collections.Generic.List<GridTile> path;
    private readonly int movementCost;
    private readonly CombatPrediction bestReachablePrediction;
    private readonly int threatAtDestination;
    private readonly int minDistanceToEnemy;

    public MoveAction(
        Unit actor,
        GridTile destination,
        System.Collections.Generic.List<GridTile> path,
        int movementCost,
        CombatPrediction bestReachablePrediction,
        int threatAtDestination,
        int minDistanceToEnemy = int.MaxValue)
    {
        Actor = actor;
        Destination = destination;
        this.path = path;
        this.movementCost = movementCost;
        this.bestReachablePrediction = bestReachablePrediction;
        this.threatAtDestination = threatAtDestination;
        this.minDistanceToEnemy = minDistanceToEnemy;
    }

    public AIActionOutcome Predict()
    {
        AIActionOutcome outcome = new AIActionOutcome();

        outcome.Set("Destination", Destination);
        outcome.Set("MovementCost", movementCost);
        outcome.Set("ThreatAtDestination", threatAtDestination);
        outcome.Set("DistanceToEnemy", minDistanceToEnemy);

        bool opensAttack = bestReachablePrediction != null;
        outcome.Set("OpensAttack", opensAttack);

        if (opensAttack)
        {
            outcome.Set("BestReachableDamage", bestReachablePrediction.MaxDamage);
            outcome.Set("BestReachableHitChance", bestReachablePrediction.HitChance);
        }

        return outcome;
    }

    public void Execute(System.Action onComplete)
    {
        // Use MoveAlongPath to animate AI movement instead of instant teleport
        Actor.MoveAlongPath(path, onComplete);
    }
}