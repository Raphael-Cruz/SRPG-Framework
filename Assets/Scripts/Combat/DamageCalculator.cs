using UnityEngine;


public class DamageCalculator
{
    public int Calculate(Unit attacker, Unit defender)
    {
        int damage =
            attacker.Data.Attack -
            defender.Data.Defense;


        return Mathf.Max(damage, 1);
    }
}