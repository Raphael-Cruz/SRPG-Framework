public class UtilityActionScorer : IAIActionScorer
{
    private readonly AIPersonalityProfile profile;

    public UtilityActionScorer(AIPersonalityProfile profile)
    {
        this.profile = profile;
    }

    public float Score(IAIAction action, AIActionOutcome outcome)
    {
        if (outcome.Has("IsWait"))
        {
            return ScoreWait(outcome);
        }

        if (outcome.CombatPrediction != null)
        {
            return ScoreCombat(outcome);
        }

        if (outcome.Has("OpensAttack"))
        {
            return ScoreMove(outcome);
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

    private float ScoreMove(AIActionOutcome outcome)
    {
        outcome.TryGet("OpensAttack", out bool opensAttack);
        outcome.TryGet("ThreatAtDestination", out int threat);

        float score = 0f;

        if (opensAttack)
        {
            outcome.TryGet("BestReachableDamage", out int damage);
            outcome.TryGet("BestReachableHitChance", out float hitChance);

            float expectedDamage = damage * (hitChance / 100f);

            score += expectedDamage
                * profile.DamageWeight
                * profile.MoveOpportunityDiscount;
        }

        score -= threat * profile.ThreatWeight;

        return score;
    }

    private float ScoreWait(AIActionOutcome outcome)
    {
        outcome.TryGet("ThreatAtCurrentTile", out int threat);

        return -threat * profile.ThreatWeight;
    }
}