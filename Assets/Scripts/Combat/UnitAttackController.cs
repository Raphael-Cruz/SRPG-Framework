using UnityEngine;
using System;

public class UnitAttackController : MonoBehaviour
{
    [SerializeField]
    private AttackRangeCalculator attackRange;


    private Unit attackingUnit;
    private CombatPrediction currentPrediction;
    private Unit targetUnit;
    public AttackState State { get; private set; } = AttackState.None;


    public event Action<Unit> OnAttackConfirmed;



    public Unit CurrentTarget => targetUnit;



    public void BeginAttack(Unit unit)
    {
        attackingUnit = unit;

        State = AttackState.SelectingTarget;

        attackRange.ShowAttackRange(unit);


        Debug.Log(
            $"{unit.name} choosing attack target"
        );
    }





private void ExecuteAttack(Unit target)
{
    attackRange.ClearAttackRange();


    if(CombatPreviewController.Instance != null)
    {
        currentPrediction =
            CombatPreviewController.Instance.PreviewAttack(
                attackingUnit,
                target
            );
    }


    CombatSystem.Instance.Attack(
        attackingUnit,
        target
    );
    ClearTargetPreview();


    Unit attacker = attackingUnit;


    attackingUnit = null;
    State = AttackState.None;


    OnAttackConfirmed?.Invoke(attacker);
}



public void TryAttack(GridTile tile)
{
    if(State != AttackState.SelectingTarget)
        return;

    if(attackingUnit == null)
        return;

    if(tile == null)
        return;

    Unit target = tile.Occupant;

    if(target == null)
    {
        Debug.Log("No target.");
        return;
    }

    if(target.Team == attackingUnit.Team)
    {
        Debug.Log("Cannot attack ally.");
        return;
    }

    if(!attackRange.IsInAttackRange(tile))
    {
        Debug.Log("Target out of range.");
        return;
    }

    targetUnit = target;
    Debug.Log($"Target selected: {target.name}");
}


// Returns true if an in-progress target selection was actually
// cancelled, false if there was nothing to cancel (already idle) - lets
// UnitActionController.CancelAction() tell "something was cancelled"
// apart from "Escape was pressed while nothing was happening".
public bool CancelAttack()
{
    if(State != AttackState.SelectingTarget)
        return false;

    ClearTargetPreview();

    attackRange.ClearAttackRange();

    attackingUnit = null;
    State = AttackState.None;

    Debug.Log("Attack cancelled");

    return true;
}

public void HoverTarget(GridTile tile)
{
    if(State != AttackState.SelectingTarget)
        return;


    ClearTargetPreview();


    if(tile == null)
        return;


    Unit target = tile.Occupant;


    if(target == null)
        return;


    if(target.Team == attackingUnit.Team)
        return;


    if(!attackRange.IsInAttackRange(tile))
        return;


    targetUnit = target;


    targetUnit.Visual.SetHoveredTarget(true);


    if(CombatPreviewController.Instance != null)
    {
        currentPrediction =
            CombatPreviewController.Instance.PreviewAttack(
                attackingUnit,
                targetUnit
            );
    }
}
public void ConfirmCurrentTarget()
{
    if(State != AttackState.SelectingTarget)
        return;

    if(targetUnit == null)
        return;

    ExecuteAttack(targetUnit);
}
private void ClearTargetPreview()
{
    if(targetUnit != null)
    {
        targetUnit.Visual.SetHoveredTarget(false);
    }

    targetUnit = null;
    currentPrediction = null;

    CombatPreviewController.Instance?.ClearPreview();
}

}
