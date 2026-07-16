public class CombatModifier
{
    public CombatModifierType Type { get; private set; }

    public float Value { get; private set; }

    public string Source { get; private set; }



    public CombatModifier(
        CombatModifierType type,
        float value,
        string source)
    {
        Type = type;
        Value = value;
        Source = source;
    }
}