using UnityEngine;

public class UnitSpawnerTest : MonoBehaviour
{
    [Header("Unit Data")]
    [SerializeField] private UnitData knightData;
    [SerializeField] private UnitData enemyData;

    private void Start()
    {
        SpawnUnit(knightData, 0, 0);
        SpawnUnit(knightData, 0, 4);

        SpawnUnit(enemyData, 0, 1);
        SpawnUnit(enemyData, 1, 0);

        BattleManager.Instance.StartBattle();
    }

    private Unit SpawnUnit(UnitData data, int x, int y)
    {
        if (data == null)
        {
            Debug.LogError("Cannot spawn unit. UnitData is null.");
            return null;
        }

        GridTile tile = GridManager.Instance.GetTile(x, y);

        if (tile == null)
        {
            Debug.LogError(
                $"Cannot spawn {data.UnitName}. " +
                $"Tile ({x},{y}) does not exist."
            );

            return null;
        }

        Unit unit = UnitManager.Instance.SpawnUnit(
            data,
            tile
        );

        if (unit == null)
        {
            Debug.LogError(
                $"Failed to spawn unit: {data.UnitName}"
            );

            return null;
        }

        InitiativeOrderSystem.Instance.Register(unit);

        Debug.Log(
            $"Spawned {unit.name} at Tile ({tile.X},{tile.Y})"
        );

        return unit;
    }
}