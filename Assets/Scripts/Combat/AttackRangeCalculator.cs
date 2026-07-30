using System.Collections.Generic;
using UnityEngine;

public class AttackRangeCalculator : MonoBehaviour
{
    private readonly List<GridTile> currentRange = new List<GridTile>();

    public IReadOnlyList<GridTile> CurrentRange => currentRange;


    public void ShowAttackRange(Unit unit)
    {
        ClearAttackRange();

        List<GridTile> tiles = CalculateRange(unit);
        currentRange.AddRange(tiles);

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


    // Pure calculation, no visual side effects.
    // Safe to call for AI evaluation without touching tile state.
    public List<GridTile> CalculateRange(Unit unit)
    {
        List<GridTile> tiles = new List<GridTile>();

        GridTile start = unit.EffectiveTile;

        int range = unit.Data.AttackRange;

        // Right
        CheckDirection(start, 1, 0, range, tiles);

        // Left
        CheckDirection(start, -1, 0, range, tiles);

        // Up
        CheckDirection(start, 0, 1, range, tiles);

        // Down
        CheckDirection(start, 0, -1, range, tiles);

        return tiles;
    }


    private void CheckDirection(
        GridTile start,
        int xDirection,
        int yDirection,
        int range,
        List<GridTile> tiles)
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

            tiles.Add(tile);

            // Future:
            // Stop here if line-of-sight is blocked.
        }
    }
}