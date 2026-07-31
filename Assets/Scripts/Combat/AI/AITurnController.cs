using System.Collections.Generic;
using UnityEngine;

public class AITurnController : MonoBehaviour
{
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private AIPersonalityProfile defaultProfile;

    private AIPerception perception;
    private AttackRangeCalculator rangeCalculator;
    private AttackTargetSelector targetSelector;
    private MovementRangeCalculator movementCalculator;
    private ThreatMapCalculator threatMapCalculator;
    private CombatSimulator simulator;

    private void Awake()
    {
        perception = new AIPerception(unitManager);

        rangeCalculator = gameObject.AddComponent<AttackRangeCalculator>();
        targetSelector = gameObject.AddComponent<AttackTargetSelector>();
        movementCalculator = gameObject.AddComponent<MovementRangeCalculator>();

        threatMapCalculator =
            new ThreatMapCalculator(rangeCalculator, movementCalculator);

        simulator = CombatSimulatorFactory.CreateStandard();
    }

    private void Start()
{
    if (unitManager != null)
    {
        unitManager.OnUnitMoved += HandleThreatChangingEvent;
        unitManager.OnUnitDied += HandleThreatChangingEvent;
    }
}

private void OnDisable()
{
    if (unitManager != null)
    {
        unitManager.OnUnitMoved -= HandleThreatChangingEvent;
        unitManager.OnUnitDied -= HandleThreatChangingEvent;
    }
}

private void HandleThreatChangingEvent(Unit unit)
{
    threatMapCalculator.Invalidate();
}

    public void RunTurn(Unit unit)
    {
        BattlefieldSnapshot snapshot = perception.Observe(unit);

        if (snapshot.Enemies.Count == 0)
        {
            Debug.Log($"{unit.name}: no enemies observed, ending turn.");
            TurnManager.Instance.EndTurn(unit);
            return;
        }

        AIActionGenerator generator =
            new AIActionGenerator(
                rangeCalculator,
                targetSelector,
                movementCalculator,
                simulator,
                threatMapCalculator
            );

        IAIActionScorer scorer = new UtilityActionScorer(defaultProfile);

        // Pass 1: choose the best action available - attack, a move
        // that sets one up or plays safer, or explicitly waiting.
        List<IAIAction> initialActions =
            generator.GenerateActions(unit, snapshot.Enemies);
        ExecuteBest(initialActions, scorer, unit);

        // Pass 2: if the unit moved but can still act, re-evaluate
        // attacks from its new position - including declining to
        // attack if nothing found is worth it.
        if (unit.CanAct)
        {
            List<IAIAction> followUpActions =
                generator.GenerateAttackActionsWithWait(unit, snapshot.Enemies);
            ExecuteBest(followUpActions, scorer, unit);
        }

        TurnManager.Instance.EndTurn(unit);
    }

    private void ExecuteBest(
        List<IAIAction> actions,
        IAIActionScorer scorer,
        Unit unit)
    {
        IAIAction bestAction = null;
        float bestScore = float.NegativeInfinity;

        foreach (IAIAction action in actions)
        {
            AIActionOutcome outcome = action.Predict();
            float score = scorer.Score(action, outcome);

            if (score > bestScore)
            {
                bestScore = score;
                bestAction = action;
            }
        }

        if (bestAction != null)
        {
            Debug.Log($"{unit.name} chose an action, score {bestScore}");
            bestAction.Execute();
        }
    }
}