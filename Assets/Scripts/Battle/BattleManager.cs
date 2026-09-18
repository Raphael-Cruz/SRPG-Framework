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

    [Header("Conditions")]
    [Tooltip("Lista de condições para o jogador ganhar. Se QUALQUER UMA for atingida, é Vitória.")]
    public System.Collections.Generic.List<BattleCondition> winConditions;
    
    [Tooltip("Lista de condições para o jogador perder. Se QUALQUER UMA for atingida, é Derrota.")]
    public System.Collections.Generic.List<BattleCondition> lossConditions;

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
        {
            Debug.LogWarning($"CheckBattleEnd chamado mas o estado atual é {State}");
            return;
        }

        Debug.Log("Verificando fim de batalha...");

        // Verifica primeiro as condições de Derrota
        if (lossConditions != null && lossConditions.Count > 0)
        {
            foreach (var condition in lossConditions)
            {
                if (condition != null && condition.IsConditionMet(this))
                {
                    Debug.Log($"Condição de derrota atingida: {condition.name}");
                    Defeat();
                    return; // Interrompe pois a batalha acabou
                }
            }
        }
        else
        {
            Debug.Log("Nenhuma condição de derrota configurada na lista Loss Conditions.");
        }

        // Se não perdeu, verifica as condições de Vitória
        if (winConditions != null && winConditions.Count > 0)
        {
            foreach (var condition in winConditions)
            {
                if (condition != null)
                {
                    bool met = condition.IsConditionMet(this);
                    Debug.Log($"Checando condição de vitória '{condition.name}': {met}");
                    if (met)
                    {
                        Victory();
                        return; // Interrompe pois a batalha acabou
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("A lista Win Conditions está vazia ou nula no BattleManager!");
        }
    }


    private void Victory()
    {
        SetState(BattleState.Victory);
        EndBattle();

        Debug.Log("Battle Won");
        OnVictory?.Invoke();

        // Retorna para a cena de exploração usando o GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToExploration();
        }
    }


    private void Defeat()
    {
        SetState(BattleState.Defeat);
        EndBattle();

        Debug.Log("Battle Lost");
        OnDefeat?.Invoke();

        // O ideal seria chamar GameManager.Instance.GoToGameOverScene()
        // Mas como ainda não foi criada, vamos apenas voltar para a cena de exploração
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToExploration();
        }
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