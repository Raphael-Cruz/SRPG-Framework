using UnityEngine;

public class UnitSpawnerTest : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Unit knightPrefab;
   
     [SerializeField] private Unit enemyPrefab;

  private void Start()
{
    SpawnUnit(knightPrefab, 0, 0);
    SpawnUnit(knightPrefab, 0, 4);
    SpawnUnit(enemyPrefab, 0, 1);
      SpawnUnit(enemyPrefab, 1, 0);


    BattleManager.Instance.StartBattle();
}

    private Unit SpawnUnit(Unit prefab, int x, int y)
    {
        GridTile tile = GridManager.Instance.GetTile(x, y);

        if (tile == null)
        {
            Debug.LogError($"Cannot spawn. Tile ({x},{y}) does not exist.");
            return null;
        }

        Unit unit = Instantiate(
            prefab,
            tile.WorldPosition,
            Quaternion.identity
        );

        unit.SetTile(tile);

        InitiativeOrderSystem.Instance.Register(unit);
 

        Debug.Log($"Spawned {unit.name} at Tile ({tile.X},{tile.Y})");

        return unit;
    }
}