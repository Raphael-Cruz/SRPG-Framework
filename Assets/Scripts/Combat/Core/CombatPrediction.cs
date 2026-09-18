using System.Collections.Generic;

public class CombatPrediction
{
    public bool CanExecute { get; }

    public int Attack { get; }
    public int Defense { get; }

    public int MinDamage { get; }
    public int MaxDamage { get; }

    public int Accuracy { get; }
    public int Avoid { get; }

    public float HitChance { get; }
    public float CritChance { get; }

    public HPGaugeState AttackerGauge { get; }
    public HPGaugeState DefenderGauge { get; }

    public SPGaugeState AttackerSPGauge { get; }
    public SPGaugeState DefenderSPGauge { get; }

    public IReadOnlyList<CombatModifier> Modifiers => modifiers;

    private readonly List<CombatModifier> modifiers = new();

    public CombatPrediction(
        bool canExecute,
        int attack,
        int defense,
        int minDamage,
        int maxDamage,
        int accuracy,
        int avoid,
        float hitChance,
        float critChance,
        HPGaugeState attackerGauge,
        HPGaugeState defenderGauge,
        SPGaugeState attackerSPGauge,
        SPGaugeState defenderSPGauge)
    {
        CanExecute = canExecute;

        Attack = attack;
        Defense = defense;

        MinDamage = minDamage;
        MaxDamage = maxDamage;

        Accuracy = accuracy;
        Avoid = avoid;

        HitChance = hitChance;
        CritChance = critChance;

        AttackerGauge = attackerGauge;
        DefenderGauge = defenderGauge;

        AttackerSPGauge = attackerSPGauge;
        DefenderSPGauge = defenderSPGauge;
    }

    public void AddModifier(CombatModifier modifier)
    {
        modifiers.Add(modifier);
    }
}