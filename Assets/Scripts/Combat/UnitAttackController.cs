using UnityEngine;
using System;

public class UnitAttackController : MonoBehaviour
{
    [SerializeField]
    private AttackRangeCalculator attackRange;
    
[SerializeField]
private AttackTargetSelector targetSelector;


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

        targetSelector.BuildTargetList(
            unit,
            attackRange.CurrentRange
        );


        Debug.Log(
            $"{unit.name} choosing attack target"
        );
    }





private void ExecuteAttack()
{
    if (targetUnit == null)
        return;

    attackRange.ClearAttackRange();

    if (CombatPreviewController.Instance != null)
    {
        currentPrediction =
            CombatPreviewController.Instance.PreviewAttack(
                attackingUnit,
                targetUnit
            );
    }

    CombatSystem.Instance.Attack(
        attackingUnit,
        targetUnit
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


if(!targetSelector.IsValidTarget(target))
{
    Debug.Log("Invalid attack target.");
    return;
}


targetSelector.SetTarget(target);

targetUnit = target;


Debug.Log(
    $"Target selected: {target.name}"
);

ExecuteAttack();
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

public void ChangeTarget(GridTile tile)
{
    if(State != AttackState.SelectingTarget)
        return;


    if(tile == null)
        return;


    Unit target = tile.Occupant;


    if(target == null)
        return;


    if(target.Team == attackingUnit.Team)
        return;


    if(!attackRange.IsInAttackRange(tile))
        return;



    // Same target, don't rebuild preview
    if(targetUnit == target)
        return;



    ClearTargetPreview();


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

public bool IsValidAttackTile(GridTile tile)
{
    if(tile == null)
        return false;


    Unit target = tile.Occupant;


    if(target == null)
        return false;


    if(target.Team == attackingUnit.Team)
        return false;


    return attackRange.IsInAttackRange(tile);
}


public void ConfirmCurrentTarget()
{
    if(State != AttackState.SelectingTarget)
        return;


    if(targetUnit == null)
    {
        Debug.Log("No attack target selected.");
        return;
    }


    ExecuteAttack();
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

public bool CanHoverTarget(GridTile tile)
{
    if(tile == null)
        return false;


    Unit target = tile.Occupant;


    if(target == null)
        return false;


    if(targetUnit == target)
        return true;


    return targetSelector.IsValidTarget(target);
}

}
