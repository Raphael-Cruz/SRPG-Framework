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


    public void ShowMovementRange(Unit unit)
    {
        ClearMovementRange();

        CalculateRange(unit);

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
    }


    public bool IsReachable(Unit unit, GridTile target)
    {
        if (unit == null || target == null)
            return false;

        // currentCost is the source of truth now; currentRange only exists
        // for driving the tile highlight visuals.
        return currentCost.ContainsKey(target);
    }


    // ==========================
    // Weighted range calculation (Dijkstra)
    // ==========================
    //
    // Movement is no longer uniform-cost, so a plain BFS (which assumes every
    // edge costs the same) can no longer guarantee the cheapest distance to
    // each tile. Dijkstra always expands the *cheapest known* frontier tile
    // next, so once a tile is finalized we know that's the true minimum cost
    // to reach it - even with mixed orthogonal/diagonal (and, later, terrain) costs.
    private void CalculateRange(Unit unit)
    {
        int movement = unit.Data.MovementRange;

        GridTile start = unit.CurrentTile;

        Dictionary<GridTile, int> distance = new Dictionary<GridTile, int>();
        HashSet<GridTile> visited = new HashSet<GridTile>();

        // Simple min-priority queue over (tile, cost). Grids in this game are
        // small, so a List + linear scan for the minimum is fast enough and
        // avoids pulling in an external priority queue implementation.
        // (Fine up to ~15x15 maps; swap for a binary heap if maps grow much
        // larger than that.)
        List<(GridTile tile, int cost)> frontier = new List<(GridTile, int)>();

        distance[start] = 0;
        frontier.Add((start, 0));

        while (frontier.Count > 0)
        {
            int bestIndex = GetLowestCostIndex(frontier);
            (GridTile current, int currentDistance) = frontier[bestIndex];
            frontier.RemoveAt(bestIndex);

            // A tile can be enqueued more than once with different costs;
            // once it's been visited with its true minimum, skip stale entries.
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

                // Impassable tiles (e.g. future "Water") report a cost <= 0.
                if (moveCost <= 0)
                    continue;

                int newDistance = currentDistance + moveCost;

                if (newDistance > movement)
                    continue;

                if (!distance.TryGetValue(neighbor, out int knownDistance) ||
                    newDistance < knownDistance)
                {
                    distance[neighbor] = newDistance;
                    frontier.Add((neighbor, newDistance));
                }
            }
        }

        // Reuse the existing dictionary/list rather than replacing them,
        // to avoid handing Unity's GC more work than it already has.
        currentCost.Clear();
        currentRange.Clear();

        foreach (KeyValuePair<GridTile, int> entry in distance)
        {
            if (entry.Key == start)
                continue;

            currentCost[entry.Key] = entry.Value;
            currentRange.Add(entry.Key);
        }
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