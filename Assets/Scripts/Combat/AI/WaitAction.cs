public class WaitAction : IAIAction
{
    public Unit Actor { get; }

    private readonly int threatAtCurrentTile;

    public WaitAction(Unit actor, int threatAtCurrentTile)
    {
        Actor = actor;
        this.threatAtCurrentTile = threatAtCurrentTile;
    }

    public AIActionOutcome Predict()
    {
        AIActionOutcome outcome = new AIActionOutcome();

        outcome.Set("IsWait", true);
        outcome.Set("ThreatAtCurrentTile", threatAtCurrentTile);

        return outcome;
    }

    public void Execute()
    {
        // Deliberately does nothing. Doing nothing needs to be a
        // candidate the scorer can choose - not just what happens
        // by accident when every other action scored worse.
    }
}