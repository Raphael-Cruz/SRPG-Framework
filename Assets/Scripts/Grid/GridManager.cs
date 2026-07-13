using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private float cellSize = 1f;
    public static GridManager Instance { get; private set; }


    private GridTile[,] grid;

public int Width => width;
public int Height => height;
public float CellSize => cellSize;

 private void Awake()
{
    if(Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;

    GenerateGrid();
}


    private void GenerateGrid()
    {
        grid = new GridTile[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(
                    x * cellSize,
                    0,
                    y * cellSize
                );


                GameObject tileObject =
                    Instantiate(
                        tilePrefab,
                        position,
                        Quaternion.identity,
                        gridParent
                    );


                GridTile tile = tileObject.GetComponent<GridTile>();

                tile.Initialize(x, y);


                grid[x, y] = tile;
            }
        }


        Debug.Log($"Generated {width}x{height} grid.");
    }




    public bool IsInsideGrid(int x, int y)
{
    return x >= 0 &&
           x < width &&
           y >= 0 &&
           y < height;
}

public GridTile GetTile(int x, int y)
{
    if (x < 0 || x >= width ||
        y < 0 || y >= height)
    {
        return null;
    }

    return grid[x, y];
}

public GridTile GetTileFromWorldPosition(Vector3 worldPosition)
{
    int x = Mathf.FloorToInt(worldPosition.x / cellSize);
    int y = Mathf.FloorToInt(worldPosition.z / cellSize);

    return GetTile(x, y);
}

public List<GridTile> GetNeighbors(GridTile tile)
{
    List<GridTile> neighbors = new List<GridTile>();

    int x = tile.X;
    int y = tile.Y;


    // Up
    AddNeighbor(neighbors, x, y + 1);

    // Down
    AddNeighbor(neighbors, x, y - 1);

    // Right
    AddNeighbor(neighbors, x + 1, y);

    // Left
    AddNeighbor(neighbors, x - 1, y);




    return neighbors;
}


private void AddNeighbor(
    List<GridTile> neighbors,
    int x,
    int y)
{
    GridTile tile = GetTile(x, y);

    if(tile != null)
    {
        neighbors.Add(tile);
    }
}

}