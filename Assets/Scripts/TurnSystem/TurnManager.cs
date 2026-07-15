using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private InitiativeOrderSystem initiativeOrder;
    [SerializeField] private SelectionManager selectionManager;

    private List<Unit> turnOrder = new();
    private int currentIndex = -1;
    private int roundNumber;

    public Unit CurrentUnit =>
        (currentIndex >= 0 && currentIndex < turnOrder.Count)
            ? turnOrder[currentIndex]
            : null;

    public IReadOnlyList<Unit> TurnOrder => turnOrder;

    public int RoundNumber => roundNumber;

    public event Action<int> OnRoundStarted;
    public event Action<Unit> OnUnitTurnStarted;
    public event Action<Unit> OnUnitTurnEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartBattle()
    {
        roundNumber = 0;
        currentIndex = -1;

        StartNewRound();
    }

    private void StartNewRound()
    {
        turnOrder = initiativeOrder.GenerateOrder();

        if (turnOrder.Count == 0)
        {
            Debug.LogWarning(
                "TurnManager: no units available to build a turn order from."
            );
            return;
        }

        roundNumber++;
        currentIndex = -1;

        OnRoundStarted?.Invoke(roundNumber);

        AdvanceTurn();
    }

    public void AdvanceTurn()
    {
        currentIndex++;

        while (currentIndex < turnOrder.Count &&
               !turnOrder[currentIndex].IsAlive)
        {
            currentIndex++;
        }

        if (currentIndex >= turnOrder.Count)
        {
            StartNewRound();
            return;
        }

        StartUnitTurn(CurrentUnit);
    }

    private void StartUnitTurn(Unit unit)
    {
        unit.ResetTurn();
        unit.SetActiveTurn(true);

        OnUnitTurnStarted?.Invoke(unit);

        if (unit.IsPlayerControlled)
        {
            selectionManager.SelectUnit(unit);
        }
        else
        {
            RunAITurn(unit);
        }
    }

    private void RunAITurn(Unit unit)
    {
        EndTurn(unit);
    }

    public void EndTurn(Unit unit)
    {
        if (unit == null || unit != CurrentUnit)
            return;

        if (selectionManager.SelectedUnit == unit)
        {
            selectionManager.DeselectCurrentUnit();
        }

        unit.FinishTurn();

        unit.SetActiveTurn(false);

        OnUnitTurnEnded?.Invoke(unit);

        AdvanceTurn();
    }

    public void Halt()
    {
        if (CurrentUnit != null)
        {
            CurrentUnit.SetActiveTurn(false);
        }
    }
}