using UnityEngine;

public class CombatContext
{
    public Unit Attacker { get; }

    public Unit Defender { get; }

    public int Attack { get; }

    public int Defense { get; }

    public int Accuracy { get; }

    public int Avoid { get; }

    public CombatContext(
        Unit attacker,
        Unit defender,
        int attack,
        int defense,
        int accuracy,
        int avoid)
    {
        Attacker = attacker;
        Defender = defender;

        Attack = attack;
        Defense = defense;

        Accuracy = accuracy;
        Avoid = avoid;
    }
}
