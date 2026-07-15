using UnityEngine;
using System;

public class UnitAttackController : MonoBehaviour
{
    [SerializeField]
    private AttackRangeCalculator attackRange;


    private Unit attackingUnit;

    public AttackState State { get; private set; } = AttackState.None;


    public event Action<Unit> OnAttackConfirmed;



    



    public void BeginAttack(Unit unit)
    {
        attackingUnit = unit;

        State = AttackState.SelectingTarget;

        attackRange.ShowAttackRange(unit);


        Debug.Log(
            $"{unit.name} choosing attack target"
        );
    }





    private void ConfirmAttack(Unit target)
    {
        attackRange.ClearAttackRange();


        CombatSystem.Instance.Attack(
            attackingUnit,
            target
        );


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

    ConfirmAttack(target);
}


// Returns true if an in-progress target selection was actually
// cancelled, false if there was nothing to cancel (already idle) - lets
// UnitActionController.CancelAction() tell "something was cancelled"
// apart from "Escape was pressed while nothing was happening".
public bool CancelAttack()
{
    if(State != AttackState.SelectingTarget)
        return false;

    attackRange.ClearAttackRange();

    attackingUnit = null;
    State = AttackState.None;

    Debug.Log("Attack cancelled");

    return true;
}
}
