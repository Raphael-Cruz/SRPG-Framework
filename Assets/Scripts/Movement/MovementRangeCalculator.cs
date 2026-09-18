using System.Collections.Generic;
using UnityEngine;

// ==========================
// Movement cost abstraction
// ==========================
//
// Pulled out of MovementRangeCalculator so the calculator only has to ask
// "how much does this move cost?" and never has to know *why*. Terrain,
// unit type (flying/cavalry/mage), equipment, and special rules (bridges,
// lava) all become swappable implementations of this interface instead of
// branches inside the pathfinding code.
public interface IMovementCostProvider
{
    // Return <= 0 to mark the destination tile as impassable for this unit.
    int GetCost(Unit unit, GridTile from, GridTile to);
}


// Default rule set: orthogonal moves cost 1, diagonal moves cost 2.
// This is the only place that knows about direction; terrain-aware
// providers can wrap or replace this later without touching the calculator.
public class DefaultMovementCostProvider : IMovementCostProvider
{
    public int GetCost(Unit unit, GridTile from, GridTile to)
    {
        int xDifference = Mathf.Abs(from.X - to.X);
        int yDifference = Mathf.Abs(from.Y - to.Y);

        // Diagonal
        if (xDifference == 1 && yDifference == 1)
        {
            return 2;
        }

        // Cardinal
        return 1;
    }
}


public class MovementRangeCalculator : MonoBehaviour
{
    // Swappable so future terrain/unit-aware rules can be plugged in from
    // the inspector or from code without changing this class.
    private IMovementCostProvider costProvider = new DefaultMovementCostProvider();

    private List<GridTile> currentRange = new List<GridTile>();

    // Source of truth for "can this unit reach this tile, and for how much".
    // Reused across calculations to avoid re-allocating every selection.
    private Dictionary<GridTile, int> currentCost = new Dictionary<GridTile, int>();


    public void SetCostProvider(IMovementCostProvider provider)
    {
        costProvider = provider ?? new DefaultMovementCostProvider();
    }


    private Dictionary<GridTile, GridTile> currentCameFrom = new Dictionary<GridTile, GridTile>();

    public void ShowMovementRange(Unit unit)
    {
        ClearMovementRange();

        var calculation = CalculateRange(unit);
        Dictionary<GridTile, int> costs = calculation.costs;
        Dictionary<GridTile, GridTile> cameFrom = calculation.cameFrom;

        currentCameFrom = cameFrom;

        foreach (KeyValuePair<GridTile, int> entry in costs)
        {
            currentCost[entry.Key] = entry.Value;
            currentRange.Add(entry.Key);
        }

        foreach (GridTile tile in currentRange)
        {
            tile.SetMovementRange(true);
        }
    }

    public void ClearMovementRange()
    {
        foreach (GridTile tile in currentRange)
        {
            tile.SetMovementRange(false);
        }

        currentRange.Clear();
        currentCost.Clear();
        currentCameFrom.Clear();
    }

    public bool IsReachable(Unit unit, GridTile target)
    {
        if (unit == null || target == null)
            return false;

        return currentCost.ContainsKey(target);
    }

    public List<GridTile> GetPath(GridTile target)
    {
        List<GridTile> path = new List<GridTile>();

        if (target == null || (!currentCameFrom.ContainsKey(target) && !currentCost.ContainsKey(target)))
            return path;

        GridTile curr = target;
        while (curr != null)
        {
            path.Add(curr);
            if (currentCameFrom.TryGetValue(curr, out GridTile next))
            {
                curr = next;
            }
            else
            {
                break;
            }
        }

        path.Reverse();
        return path;
    }

    // Pure calculation, no visual side effects, no shared-state mutation.
    public (Dictionary<GridTile, int> costs, Dictionary<GridTile, GridTile> cameFrom) CalculateRange(Unit unit)
    {
        int movement = unit.Data.MovementRange;

        GridTile start = unit.CurrentTile;

        Dictionary<GridTile, int> distance = new Dictionary<GridTile, int>();
        Dictionary<GridTile, GridTile> cameFrom = new Dictionary<GridTile, GridTile>();
        HashSet<GridTile> visited = new HashSet<GridTile>();

        List<(GridTile tile, int cost)> frontier = new List<(GridTile, int)>();

        distance[start] = 0;
        cameFrom[start] = null;
        frontier.Add((start, 0));

        while (frontier.Count > 0)
        {
            int bestIndex = GetLowestCostIndex(frontier);
            (GridTile current, int currentDistance) = frontier[bestIndex];
            frontier.RemoveAt(bestIndex);

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            foreach (GridTile neighbor in GridManager.Instance.GetNeighbors(current))
            {
                if (visited.Contains(neighbor))
                    continue;

                if (neighbor.Occupant != null)
                    continue;

                int moveCost = costProvider.GetCost(unit, current, neighbor);

                if (moveCost <= 0)
                    continue;

                int newDistance = currentDistance + moveCost;

                if (newDistance > movement)
                    continue;

                if (!distance.TryGetValue(neighbor, out int knownDistance) ||
                    newDistance < knownDistance)
                {
                    distance[neighbor] = newDistance;
                    cameFrom[neighbor] = current;
                    frontier.Add((neighbor, newDistance));
                }
            }
        }

        Dictionary<GridTile, int> resultCosts = new Dictionary<GridTile, int>();

        foreach (KeyValuePair<GridTile, int> entry in distance)
        {
            if (entry.Key == start)
                continue;

            resultCosts[entry.Key] = entry.Value;
        }

        return (resultCosts, cameFrom);
    }


    private int GetLowestCostIndex(List<(GridTile tile, int cost)> frontier)
    {
        int bestIndex = 0;

        for (int i = 1; i < frontier.Count; i++)
        {
            if (frontier[i].cost < frontier[bestIndex].cost)
            {
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}