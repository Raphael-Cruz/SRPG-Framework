using System.Collections.Generic;
using UnityEngine;



public class AttackRangeCalculator : MonoBehaviour
{
    private readonly List<GridTile> currentRange = new List<GridTile>();

    public IReadOnlyList<GridTile> CurrentRange => currentRange;


    public void ShowAttackRange(Unit unit)
    {
        
        ClearAttackRange();

        CalculateRange(unit);

        foreach (GridTile tile in currentRange)
        {
            tile.SetAttackRange(true);
        }
    }


    public void ClearAttackRange()
    {
        foreach (GridTile tile in currentRange)
        {
            tile.SetAttackRange(false);
        }

        currentRange.Clear();
    }


    public bool IsInAttackRange(GridTile target)
    {
        return currentRange.Contains(target);
    }


    private void CalculateRange(Unit unit)
    {
   GridTile start = unit.EffectiveTile;

        int range = unit.Data.AttackRange;

        // Right
        CheckDirection(start, 1, 0, range);

        // Left
        CheckDirection(start, -1, 0, range);

        // Up
        CheckDirection(start, 0, 1, range);

        // Down
        CheckDirection(start, 0, -1, range);
    }


    private void CheckDirection(
        GridTile start,
        int xDirection,
        int yDirection,
        int range)
    {
        int x = start.X;
        int y = start.Y;

        for (int i = 1; i <= range; i++)
        {
            x += xDirection;
            y += yDirection;

            GridTile tile = GridManager.Instance.GetTile(x, y);

            if (tile == null)
                break;

            currentRange.Add(tile);

            // Future:
            // Stop here if line-of-sight is blocked.
        }
    }
}