public class MoveAction : IAIAction
{
    public Unit Actor { get; }

    public GridTile Destination { get; }

    private readonly int movementCost;
    private readonly CombatPrediction bestReachablePrediction;
    private readonly int threatAtDestination;

    public MoveAction(
        Unit actor,
        GridTile destination,
        int movementCost,
        CombatPrediction bestReachablePrediction,
        int threatAtDestination)
    {
        Actor = actor;
        Destination = destination;
        this.movementCost = movementCost;
        this.bestReachablePrediction = bestReachablePrediction;
        this.threatAtDestination = threatAtDestination;
    }

    public AIActionOutcome Predict()
    {
        AIActionOutcome outcome = new AIActionOutcome();

        outcome.Set("Destination", Destination);
        outcome.Set("MovementCost", movementCost);
        outcome.Set("ThreatAtDestination", threatAtDestination);

        bool opensAttack = bestReachablePrediction != null;
        outcome.Set("OpensAttack", opensAttack);

        if (opensAttack)
        {
            outcome.Set("BestReachableDamage", bestReachablePrediction.Damage);
            outcome.Set("BestReachableHitChance", bestReachablePrediction.HitChance);
        }

        return outcome;
    }

    public void Execute()
    {
        Actor.MoveTo(Destination);
    }
}