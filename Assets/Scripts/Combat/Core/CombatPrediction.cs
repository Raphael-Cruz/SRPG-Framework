using System.Collections.Generic;


public class CombatPrediction
{
    public bool CanExecute { get; private set; }


    public int Damage { get; private set; }


    public float HitChance { get; private set; }


    public IReadOnlyList<CombatModifier> Modifiers
        => modifiers;


    private readonly List<CombatModifier> modifiers
        = new List<CombatModifier>();



    public CombatPrediction(
        bool canExecute,
        int damage,
        float hitChance)
    {
        CanExecute = canExecute;
        Damage = damage;
        HitChance = hitChance;
    }



    public void AddModifier(
        CombatModifier modifier)
    {
        modifiers.Add(modifier);
    }
}