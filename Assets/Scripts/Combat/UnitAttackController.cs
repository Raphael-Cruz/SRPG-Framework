using UnityEngine;
using System;

public class UnitAttackController : MonoBehaviour
{
    [SerializeField]
    private AttackRangeCalculator attackRange;


    private Unit attackingUnit;


    public event Action<Unit> OnAttackConfirmed;



    



    public void BeginAttack(Unit unit)
    {
        attackingUnit = unit;

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


        OnAttackConfirmed?.Invoke(attackingUnit);


        attackingUnit = null;
    }



public void TryAttack(GridTile tile)
{
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
public void CancelAttack()
{
    attackRange.ClearAttackRange();
}
}