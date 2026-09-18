using UnityEngine;

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

    // O método agora é genérico e serve para qualquer tipo de modificador.
    // Por padrão, ele apenas arredonda e retorna o 'Value' original.
    public virtual int GetContextualValue(CombatContext context) 
    {
        return Mathf.RoundToInt(Value);
    }
}