using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Se ativo, calcula o tamanho do grid automaticamente baseado no Terrain da cena.")]
    [SerializeField] private bool autoSizeToTerrain = true;
    
    [Tooltip("Valores manuais (ignorados se Auto Size To Terrain estiver ativo)")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private float cellSize = 1f;

    [Header("Terrain Settings")]
    [Tooltip("A layer do seu terreno/chão.")]
    [SerializeField] private LayerMask groundLayer = ~0;
    
    [Tooltip("A layer de obstáculos (pedras, paredes). Tiles sob isso ficarão bloqueados.")]
    [SerializeField] private LayerMask obstacleLayer;

    [Tooltip("Altura de onde o raio será disparado para encontrar o chão.")]
    [SerializeField] private float raycastHeight = 50f;

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
        // Se estiver configurado para auto-size, adapta ao terreno ativo
        if (autoSizeToTerrain && Terrain.activeTerrain != null)
        {
            Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
            width = Mathf.FloorToInt(terrainSize.x / cellSize);
            height = Mathf.FloorToInt(terrainSize.z / cellSize);

            // TRAVA DE SEGURANÇA: Previne que terrenos gigantes travem a Unity (ex: 1000x1000 = 1 milhão de instâncias)
            if (width > 150) width = 150;
            if (height > 150) height = 150;

            // Move o GridManager para a origem do terreno para que o grid comece do ponto (0,0) do terreno
            transform.position = Terrain.activeTerrain.transform.position;
        }

        grid = new GridTile[width, height];


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calcula a posição baseada na posição do objeto GridManager
                Vector3 position = new Vector3(
                    transform.position.x + (x * cellSize),
                    transform.position.y,
                    transform.position.z + (y * cellSize)
                );

                // Dispara um raio de cima para baixo para encontrar a altura do terreno
                Vector3 rayStart = position + Vector3.up * raycastHeight;
                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
                {
                    position.y = hit.point.y;
                }


                GameObject tileObject =
                    Instantiate(
                        tilePrefab,
                        position,
                        Quaternion.identity,
                        gridParent
                    );


                GridTile tile = tileObject.GetComponent<GridTile>();
                tile.Initialize(x, y);

                // Se houver um obstáculo em cima desse tile, marca como bloqueado
                if (obstacleLayer != 0 && Physics.CheckSphere(position + Vector3.up * 0.5f, cellSize * 0.45f, obstacleLayer))
                {
                    tile.SetBlocked(true);
                }

                // Eleva levemente o tile para ele não "afundar" ou piscar dentro do Terreno (Z-Fighting)
                position.y += 0.05f;

                grid[x, y] = tile;
                
                // Reposiciona o objeto visual do tile
                tileObject.transform.position = position;
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
    // Subtrai a posição base do grid para encontrar a coordenada local relativa
    Vector3 localPosition = worldPosition - transform.position;

    int x = Mathf.FloorToInt(localPosition.x / cellSize);
    int y = Mathf.FloorToInt(localPosition.z / cellSize);

    return GetTile(x, y);
}

public List<GridTile> GetNeighbors(GridTile tile)
{
    List<GridTile> neighbors = new List<GridTile>();

    int x = tile.X;
    int y = tile.Y;


    // Cardinal directions
    AddNeighbor(neighbors, x, y + 1); // Up
    AddNeighbor(neighbors, x, y - 1); // Down
    AddNeighbor(neighbors, x + 1, y); // Right
    AddNeighbor(neighbors, x - 1, y); // Left


    // Diagonal directions
    AddNeighbor(neighbors, x + 1, y + 1); // Up Right
    AddNeighbor(neighbors, x - 1, y + 1); // Up Left
    AddNeighbor(neighbors, x + 1, y - 1); // Down Right
    AddNeighbor(neighbors, x - 1, y - 1); // Down Left


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

public IEnumerable<GridTile> GetAllTiles()
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            yield return grid[x, y];
        }
    }
}

}