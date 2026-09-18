using UnityEngine;

public class CombatContext
{
    public Unit Attacker { get; }

    public Unit Defender { get; }

    public int Attack { get; }

    public int Defense { get; }

    public int Accuracy { get; }
    public int Avoid { get; }
    public int Crit { get; }

    public TimeOfDay CurrentTime { get; set; }

    public CombatContext(
        Unit attacker,
        Unit defender,
        int attack,
        int defense,
        int accuracy,
        int avoid,
        int crit)
    {
        Attacker = attacker;
        Defender = defender;

        Attack = attack;
        Defense = defense;

        Accuracy = accuracy;
        Avoid = avoid;
        Crit = crit;
    }

    
}

public enum TimeOfDay 
{
    Day,
    Night
}

