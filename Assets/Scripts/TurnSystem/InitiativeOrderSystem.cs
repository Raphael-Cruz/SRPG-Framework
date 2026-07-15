using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ==========================
// Initiative sorting strategy
// ==========================
//
// Swappable so speed, status effects, equipment, and buffs/debuffs can each
// become their own strategy (or fold into one composite strategy) later
// without TurnManager or InitiativeOrderSystem ever changing.
public interface IInitiativeSortStrategy
{
    List<Unit> Sort(List<Unit> units);
}


// MVP fallback: whatever order units were registered in. Useful for
// scripted battles/tests where you want a fixed, predictable order.
public class ManualInitiativeStrategy : IInitiativeSortStrategy
{
    public List<Unit> Sort(List<Unit> units)
    {
        return new List<Unit>(units);
    }
}


// Higher Speed acts first. OrderByDescending is a stable sort, so units
// with equal Speed keep their relative registration order - deterministic
// results instead of ties being resolved randomly.
public class SpeedInitiativeStrategy : IInitiativeSortStrategy
{
    public List<Unit> Sort(List<Unit> units)
    {
        return units
            .OrderByDescending(GetEffectiveSpeed)
            .ToList();
    }

    // Isolated on purpose: status effects (haste/slow), equipment bonuses,
    // and buffs/debuffs all plug in here later as additional terms without
    // touching the sort itself.
    private int GetEffectiveSpeed(Unit unit)
    {
        return unit.Data.Speed;
    }
}


public class InitiativeOrderSystem : MonoBehaviour
{
    public static InitiativeOrderSystem Instance { get; private set; }

    [SerializeField] private List<Unit> registeredUnits = new List<Unit>();

    // Defaults to speed-based since UnitData already carries a Speed stat;
    // swap to ManualInitiativeStrategy (or any custom one) via SetStrategy
    // if you want a fixed order for a specific battle.
    private IInitiativeSortStrategy strategy = new SpeedInitiativeStrategy();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void SetStrategy(IInitiativeSortStrategy newStrategy)
    {
        strategy = newStrategy ?? new SpeedInitiativeStrategy();
    }


    public void Register(Unit unit)
    {
        if (unit != null && !registeredUnits.Contains(unit))
        {
            registeredUnits.Add(unit);
        }
    }


    public void Unregister(Unit unit)
    {
        registeredUnits.Remove(unit);
    }


    // Unsorted, includes dead units - GenerateOrder() filters/sorts for
    // turn-taking purposes, but BattleManager needs to see everyone
    // (including the dead) to check win/loss conditions.
    public IReadOnlyList<Unit> AllUnits => registeredUnits;


    // Regenerated on demand rather than cached, so a new round naturally
    // picks up anything that changed a unit's effective speed since the
    // last round (status effects wearing off, equipment swapped, etc.),
    // and dead units simply drop out.
    public List<Unit> GenerateOrder()
    {
        List<Unit> aliveUnits = registeredUnits
            .Where(u => u != null && u.IsAlive)
            .ToList();

        return strategy.Sort(aliveUnits);
    }
}