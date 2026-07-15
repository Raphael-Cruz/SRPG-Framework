using System;
using UnityEngine;

public enum BattleState
{
    Preparing,
    Fighting,
    Victory,
    Defeat
}


// The battle director. Answers "is a battle happening, and who's winning" -
// nothing more. It does not move units, decide AI actions, calculate
// damage, or decide encounter composition; those stay in MovementSystem/
// UnitMovementController, (future) EnemyAI, (future) CombatSystem, and
// (future) EncounterManager respectively. BattleManager only starts/stops
// the systems that do that work and tracks win/loss.
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private TurnManager turnManager;
    [SerializeField] private InitiativeOrderSystem initiativeOrder;

    public BattleState State { get; private set; } = BattleState.Preparing;

    public event Action<BattleState> OnBattleStateChanged;
    public event Action OnVictory;
    public event Action OnDefeat;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // Call once units already exist and are registered with
    // InitiativeOrderSystem - spawning enemies, placing units, and any
    // other "Preparing" work belongs to EncounterManager, upstream of this
    // call, not inside it.
    public void StartBattle()
    {
        SetState(BattleState.Preparing);

        SetState(BattleState.Fighting);

        turnManager.OnUnitTurnEnded += HandleUnitTurnEnded;

        turnManager.StartBattle();
    }


    // A turn ending is a cheap, convenient moment to double-check win/loss
    // as a safety net. The primary trigger is still expected to be
    // whatever combat/death system reduces a unit's HP to 0 calling
    // CheckBattleEnd() directly, right after marking that unit dead.
    private void HandleUnitTurnEnded(Unit unit)
    {
        CheckBattleEnd();
    }


    // Call this after any unit dies. Public so a future CombatSystem (or
    // anything else that can kill a unit) can trigger the check without
    // BattleManager needing to know how or why a unit died.
    public void CheckBattleEnd()
    {
        if (State != BattleState.Fighting)
            return;

        bool anyEnemyAlive = false;
        bool anyPlayerAlive = false;

        foreach (Unit unit in initiativeOrder.AllUnits)
        {
            if (unit == null || !unit.IsAlive)
                continue;

            if (unit.IsPlayerControlled)
            {
                anyPlayerAlive = true;
            }
            else
            {
                anyEnemyAlive = true;
            }
        }

        if (!anyEnemyAlive)
        {
            Victory();
        }
        else if (!anyPlayerAlive)
        {
            Defeat();
        }
    }


    private void Victory()
    {
        SetState(BattleState.Victory);
        EndBattle();

        Debug.Log("Battle Won");
        OnVictory?.Invoke();

        // Animation, rewards, and returning to the dungeon are handled by
        // whatever UI/flow system is listening to OnVictory - not here.
    }


    private void Defeat()
    {
        SetState(BattleState.Defeat);
        EndBattle();

        Debug.Log("Battle Lost");
        OnDefeat?.Invoke();

        // Game over screen, respawn, and checkpoint reload are handled by
        // whatever flow system is listening to OnDefeat - not here.
    }


    private void EndBattle()
    {
        turnManager.OnUnitTurnEnded -= HandleUnitTurnEnded;
        turnManager.Halt();
    }


    private void SetState(BattleState newState)
    {
        State = newState;
        OnBattleStateChanged?.Invoke(newState);
    }
}