using System.Collections.Generic;

public class CombatPrediction
{
    public bool CanExecute { get; }

    public int Attack { get; }

    public int Defense { get; }

    public int Damage { get; }

    public int Accuracy { get; }

    public int Avoid { get; }

    public float HitChance { get; }

    public HPGaugeState AttackerGauge { get; }

    public HPGaugeState DefenderGauge { get; }

    public IReadOnlyList<CombatModifier> Modifiers => modifiers;

    private readonly List<CombatModifier> modifiers =
        new();

    public CombatPrediction(
        bool canExecute,
        int attack,
        int defense,
        int damage,
        int accuracy,
        int avoid,
        float hitChance,
        HPGaugeState attackerGauge,
        HPGaugeState defenderGauge)
    {
        CanExecute = canExecute;

        Attack = attack;
        Defense = defense;

        Damage = damage;

        Accuracy = accuracy;
        Avoid = avoid;

        HitChance = hitChance;

        AttackerGauge = attackerGauge;
        DefenderGauge = defenderGauge;
    }

    public void AddModifier(CombatModifier modifier)
    {
        modifiers.Add(modifier);
    }
}