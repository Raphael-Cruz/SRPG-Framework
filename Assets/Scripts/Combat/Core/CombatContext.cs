using UnityEngine;

public class CombatContext
{
    public int BaseDamage { get; private set; }

public float BaseHitChance { get; private set; }

    public Unit Attacker { get; private set; }
    public Unit Defender { get; private set; }

public CombatContext(
    Unit attacker,
    Unit defender,
    int baseDamage,
    float baseHitChance
)
{
    Attacker = attacker;
    Defender = defender;

    BaseDamage = baseDamage;
    BaseHitChance = baseHitChance;
}
}