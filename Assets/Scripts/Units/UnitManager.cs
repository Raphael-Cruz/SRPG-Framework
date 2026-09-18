using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> units = new();

    private Dictionary<GridTile, Unit> occupiedTiles = new();

    public IReadOnlyList<Unit> Units => units;

    public event Action<Unit> OnUnitMoved;
    public event Action<Unit> OnUnitDied;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Unit SpawnUnit(UnitData data, GridTile tile)
    {
        if (data == null)
        {
            Debug.LogError("Cannot spawn unit. UnitData is null.");
            return null;
        }

        if (tile == null)
        {
            Debug.LogError(
                $"Cannot spawn {data.UnitName}. GridTile is null."
            );

            return null;
        }

        if (data.Prefab == null)
        {
            Debug.LogError(
                $"Cannot spawn {data.UnitName}. " +
                "UnitData.Prefab is not assigned."
            );

            return null;
        }

        if (occupiedTiles.ContainsKey(tile))
        {
            Debug.LogError(
                $"Cannot spawn {data.UnitName}. " +
                $"Tile ({tile.X},{tile.Y}) is already occupied."
            );

            return null;
        }

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

        unit.Initialize(data);
        unit.SetTile(tile);

        RegisterUnit(unit, tile);

        return unit;
    }

    private void RegisterUnit(Unit unit, GridTile tile)
    {
        if (unit == null || tile == null)
            return;

        if (units.Contains(unit))
            return;

        units.Add(unit);

        occupiedTiles.Add(tile, unit);

        tile.SetOccupant(unit);

        unit.OnMoved += HandleUnitMoved;
        unit.OnDied += HandleUnitDied;
    }

    public Unit GetUnitAt(GridTile tile)
    {
        if (tile == null)
            return null;

        if (occupiedTiles.TryGetValue(tile, out Unit unit))
        {
            return unit;
        }

        return null;
    }

    public bool IsTileOccupied(GridTile tile)
    {
        if (tile == null)
            return false;

        return occupiedTiles.ContainsKey(tile);
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit == null)
            return;

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
        RemoveUnit(unit);

        OnUnitDied?.Invoke(unit);
    }
}