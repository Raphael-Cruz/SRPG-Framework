using System.Collections.Generic;
using UnityEngine;

public class AttackTargetSelector : MonoBehaviour
{
    private readonly List<Unit> validTargets = new();

    private Unit currentTarget;

    public IReadOnlyList<Unit> ValidTargets => validTargets;

    public Unit CurrentTarget => currentTarget;


    public void BuildTargetList(
        Unit attacker,
        IReadOnlyList<GridTile> attackTiles)
    {
        validTargets.Clear();

        currentTarget = null;


        foreach(GridTile tile in attackTiles)
        {
            if(tile == null)
                continue;


            Unit target = tile.Occupant;


            if(target == null)
                continue;


            if(target.Team == attacker.Team)
                continue;


            validTargets.Add(target);
        }


        if(validTargets.Count > 0)
        {
            currentTarget = validTargets[0];
        }
    }


    public void SetTarget(Unit target)
    {
        if(!validTargets.Contains(target))
            return;


        currentTarget = target;
    }


    public bool IsValidTarget(Unit target)
    {
        return validTargets.Contains(target);
    }


    public void Clear()
    {
        validTargets.Clear();
        currentTarget = null;
    }

}