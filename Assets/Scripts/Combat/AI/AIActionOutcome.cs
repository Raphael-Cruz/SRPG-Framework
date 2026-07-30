using System.Collections.Generic;

public class AIActionOutcome
{
    public CombatPrediction CombatPrediction { get; }

    private readonly Dictionary<string, object> facts =
        new Dictionary<string, object>();

    public AIActionOutcome(CombatPrediction combatPrediction = null)
    {
        CombatPrediction = combatPrediction;
    }

    public void Set<T>(string key, T value)
    {
        facts[key] = value;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (facts.TryGetValue(key, out object result))
        {
            if (result is T typedValue)
            {
                value = typedValue;
                return true;
            }
        }

        value = default;
        return false;
    }

    public bool Has(string key)
    {
        return facts.ContainsKey(key);
    }
}