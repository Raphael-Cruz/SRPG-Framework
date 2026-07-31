using System.Collections.Generic;

// Answers "how many enemies could attack this tile next turn?" for every
// tile any enemy threatens. This is expensive (a full movement + attack
// scan per enemy), so the result is cached and only rebuilt when the
// threat landscape could actually have changed - which AITurnController
// signals by calling Invalidate() whenever any unit finishes a turn.
public class ThreatMapCalculator
{
    private readonly AttackRangeCalculator rangeCalculator;
    private readonly MovementRangeCalculator movementCalculator;

    private Dictionary<GridTile, int> cachedThreatMap;
    private bool isDirty = true;

    public ThreatMapCalculator(
        AttackRangeCalculator rangeCalculator,
        MovementRangeCalculator movementCalculator)
    {
        this.rangeCalculator = rangeCalculator;
        this.movementCalculator = movementCalculator;
    }

    public void Invalidate()
    {
        isDirty = true;
    }

    public Dictionary<GridTile, int> GetThreatMap(IReadOnlyList<Unit> enemies)
    {
        if (isDirty || cachedThreatMap == null)
        {
            cachedThreatMap = BuildThreatMap(enemies);
            isDirty = false;
        }

        return cachedThreatMap;
    }

    private Dictionary<GridTile, int> BuildThreatMap(IReadOnlyList<Unit> enemies)
    {
        Dictionary<GridTile, int> threatCounts = new Dictionary<GridTile, int>();

        foreach (Unit enemy in enemies)
        {
            HashSet<GridTile> tilesThisEnemyThreatens =
                CollectThreatenedTiles(enemy);

            foreach (GridTile tile in tilesThisEnemyThreatens)
            {
                threatCounts.TryGetValue(tile, out int count);
                threatCounts[tile] = count + 1;
            }
        }

        return threatCounts;
    }

    private HashSet<GridTile> CollectThreatenedTiles(Unit enemy)
    {
        HashSet<GridTile> threatened = new HashSet<GridTile>();

        foreach (GridTile tile in rangeCalculator.CalculateRange(enemy))
        {
            threatened.Add(tile);
        }

        Dictionary<GridTile, int> reachable =
            movementCalculator.CalculateRange(enemy);

        foreach (GridTile destination in reachable.Keys)
        {
            enemy.SetPreviewTile(destination);

            foreach (GridTile tile in rangeCalculator.CalculateRange(enemy))
            {
                threatened.Add(tile);
            }

            enemy.ClearPreviewTile();
        }

        return threatened;
    }
}