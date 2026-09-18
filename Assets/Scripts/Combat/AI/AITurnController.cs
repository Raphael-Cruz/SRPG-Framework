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
        StartCoroutine(RunTurnRoutine(unit));
    }

    private System.Collections.IEnumerator RunTurnRoutine(Unit unit)
    {
        // Wait 1 second before doing anything (allows Idle1 to play)
        yield return new WaitForSeconds(1.0f);

        BattlefieldSnapshot snapshot = perception.Observe(unit);

        if (snapshot.Enemies.Count == 0)
        {
            Debug.Log($"{unit.name}: no enemies observed, ending turn.");
            TurnManager.Instance.EndTurn(unit);
            yield break;
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

        // Pass 1: choose the best action available
        List<IAIAction> initialActions =
            generator.GenerateActions(unit, snapshot.Enemies);
        yield return StartCoroutine(ExecuteBest(initialActions, scorer, unit));

        // Pass 2: follow-up actions if still can act
        if (unit.CanAct)
        {
            List<IAIAction> followUpActions =
                generator.GenerateAttackActionsWithWait(unit, snapshot.Enemies);
            yield return StartCoroutine(ExecuteBest(followUpActions, scorer, unit));
        }

        // Add a tiny delay before ending turn so animations can start
        yield return new WaitForSeconds(0.1f);
        TurnManager.Instance.EndTurn(unit);
    }

    private System.Collections.IEnumerator ExecuteBest(
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
            bool actionComplete = false;
            bestAction.Execute(() => actionComplete = true);
            
            // Wait until the action's callback is invoked
            yield return new WaitUntil(() => actionComplete);
        }
    }
}