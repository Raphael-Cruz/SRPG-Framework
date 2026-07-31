using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private List<Unit> units = new();

    private Dictionary<GridTile, Unit> occupiedTiles = new();

    public IReadOnlyList<Unit> Units => units;

    // Aggregated from every registered unit's own events, so a listener
    // only needs one subscription to know "something on the battlefield
    // moved or died" instead of managing per-unit subscriptions itself.
    public event Action<Unit> OnUnitMoved;
    public event Action<Unit> OnUnitDied;

    public Unit SpawnUnit(UnitData data, GridTile tile)
    {
        GameObject obj = Instantiate(
            data.Prefab,
            tile.WorldPosition,
            Quaternion.identity
        );

        Unit unit = obj.GetComponent<Unit>();

        if (unit == null)
        {
            Debug.LogError(
                "Spawned object does not contain a Unit component."
            );

            Destroy(obj);
            return null;
        }

        unit.SetTile(tile);

        RegisterUnit(unit, tile);

        return unit;
    }

    private void RegisterUnit(Unit unit, GridTile tile)
    {
        units.Add(unit);

        occupiedTiles.Add(tile, unit);

        tile.SetOccupant(unit);

        unit.OnMoved += HandleUnitMoved;
        unit.OnDied += HandleUnitDied;
    }

    public Unit GetUnitAt(GridTile tile)
    {
        if (occupiedTiles.TryGetValue(tile, out Unit unit))
        {
            return unit;
        }

        return null;
    }

    public bool IsTileOccupied(GridTile tile)
    {
        return occupiedTiles.ContainsKey(tile);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);

        if (unit.CurrentTile != null)
        {
            occupiedTiles.Remove(unit.CurrentTile);
        }

        unit.OnMoved -= HandleUnitMoved;
        unit.OnDied -= HandleUnitDied;
    }

    private void HandleUnitMoved(Unit unit)
    {
        OnUnitMoved?.Invoke(unit);
    }

    private void HandleUnitDied(Unit unit)
    {
        OnUnitDied?.Invoke(unit);
    }
}