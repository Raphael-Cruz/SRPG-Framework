
public class UtilityActionScorer : IAIActionScorer
{
    private readonly AIPersonalityProfile profile;

    public UtilityActionScorer(AIPersonalityProfile profile)
    {
        this.profile = profile;
    }

    public float Score(IAIAction action, AIActionOutcome outcome)
    {
        if (outcome.CombatPrediction != null)
        {
            return ScoreCombat(outcome);
        }

        return 0f;
    }

    private float ScoreCombat(AIActionOutcome outcome)
    {
        CombatPrediction prediction = outcome.CombatPrediction;

        if (!prediction.CanExecute)
        {
            return 0f;
        }

        float expectedDamage =
            prediction.Damage * (prediction.HitChance / 100f);

        float score = expectedDamage * profile.DamageWeight;

        if (outcome.TryGet("WillKill", out bool willKill) && willKill)
        {
            score += profile.KillBonus;
        }

        return score;
    }
}