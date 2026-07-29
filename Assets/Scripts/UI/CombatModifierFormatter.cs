public static class CombatModifierFormatter
{
    public static string GetCategory(CombatModifier modifier)
    {
        return modifier.Type switch
        {
            CombatModifierType.Attack => "Attack",
            CombatModifierType.Defense => "Defense",
            CombatModifierType.Accuracy => "Accuracy",
            CombatModifierType.Avoid => "Avoid",
            CombatModifierType.Damage => "Damage",
            CombatModifierType.HitChance => "Hit",
            CombatModifierType.CriticalChance => "Critical",
            CombatModifierType.Range => "Range",
            _ => "Other"
        };
    }

    public static string GetEffect(CombatModifier modifier)
    {
        if (modifier.Value == 0)
            return "";

        string sign = modifier.Value > 0 ? "+" : "";

        return modifier.Type switch
        {
            CombatModifierType.Attack =>
                $"{sign}{modifier.Value} Attack",

            CombatModifierType.Defense =>
                $"{sign}{modifier.Value} Defense",

            CombatModifierType.Accuracy =>
                $"{sign}{modifier.Value} Accuracy",

            CombatModifierType.Avoid =>
                $"{sign}{modifier.Value} Avoid",

            CombatModifierType.Damage =>
                $"{sign}{modifier.Value} Damage",

            CombatModifierType.HitChance =>
                $"{sign}{modifier.Value}% Hit",

            CombatModifierType.CriticalChance =>
                $"{sign}{modifier.Value}% Crit",

            CombatModifierType.Range =>
                $"{sign}{modifier.Value} Range",

            _ =>
                $"{sign}{modifier.Value}"
        };
    }
}