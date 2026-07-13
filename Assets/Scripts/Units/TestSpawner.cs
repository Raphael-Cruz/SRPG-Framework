using UnityEngine;

public class UnitSpawnerTest : MonoBehaviour
{
    [SerializeField] private Unit unitPrefab;

    [Header("Spawn Position")]
    [SerializeField] private int spawnX = 3;
    [SerializeField] private int spawnY = 4;


    private void Start()
    {
        SpawnUnit();
    }


    private void SpawnUnit()
    {
        GridTile tile =
            GridManager.Instance.GetTile(spawnX, spawnY);


        if(tile == null)
        {
            Debug.LogError(
                $"Cannot spawn. Tile ({spawnX},{spawnY}) does not exist."
            );

            return;
        }


        Unit unit = Instantiate(
            unitPrefab,
            tile.WorldPosition,
            Quaternion.identity
        );


        unit.SetTile(tile);


        Debug.Log(
            $"Spawned {unit.name} at Tile ({tile.X},{tile.Y})"
        );
    }
}