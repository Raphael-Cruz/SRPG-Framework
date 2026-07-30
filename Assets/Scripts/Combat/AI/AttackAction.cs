// AttackAction.cs
public class AttackAction : IAIAction
{
    public Unit Actor { get; }

    public Unit Target { get; }

    private readonly CombatSimulator simulator;

    public AttackAction(
        Unit actor,
        Unit target,
        CombatSimulator simulator)
    {
        Actor = actor;
        Target = target;
        this.simulator = simulator;
    }

    public AIActionOutcome Predict()
    {
        CombatContext context =
            new CombatContext(
                Actor,
                Target,
                Actor.Data.Attack,
                Target.Data.Defense,
                Actor.Data.Accuracy,
                Target.Data.Avoid
            );

        CombatPrediction prediction =
            simulator.Simulate(context);

        AIActionOutcome outcome =
            new AIActionOutcome(prediction);

        bool willKill =
            prediction.DefenderGauge.CurrentHP - prediction.Damage <= 0;

        outcome.Set("WillKill", willKill);

        return outcome;
    }

    public void Execute()
    {
        CombatSystem.Instance.Attack(Actor, Target);
    }
}