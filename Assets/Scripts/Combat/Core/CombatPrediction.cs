using System.Collections.Generic;

public class CombatPrediction
{
    public bool CanExecute { get; }

    public int Attack { get; }
    public int Defense { get; }
    public int Damage { get; }

    public float Accuracy { get; }
    public float Avoid { get; }
    public float HitChance { get; }

    public IReadOnlyList<CombatModifier> Modifiers => modifiers;

    private readonly List<CombatModifier> modifiers =
        new List<CombatModifier>();

    public CombatPrediction(
        bool canExecute,
        int attack,
        int defense,
        int damage,
        float accuracy,
        float avoid,
        float hitChance)
    {
        CanExecute = canExecute;

        Attack = attack;
        Defense = defense;
        Damage = damage;

        Accuracy = accuracy;
        Avoid = avoid;
        HitChance = hitChance;
    }

    public void AddModifier(CombatModifier modifier)
    {
        modifiers.Add(modifier);
    }
}