using UnityEngine;

[ExecuteAlways]
public class gridpreview : MonoBehaviour
{
    [Header("Grid Size")]
    [Tooltip("Se ativo, calcula o tamanho do grid automaticamente baseado no Terrain da cena.")]
    public bool autoSizeToTerrain = true;

    [Min(1)]
    public int gridWidth = 35;

    [Min(1)]
    public int gridHeight = 35;

    [Min(0.01f)]
    public float tileSize = 1f;

    [Header("Grid Position")]
    public Vector3 gridOffset = Vector3.zero;

    [Header("Appearance")]
    public Color gridColor = new Color(1f, 1f, 1f, 0.35f);

    [Min(0.001f)]
    public float lineWidth = 0.015f;

    [Header("Terrain Following")]
    public bool followTerrain = true;

    [Min(0.01f)]
    public float raycastHeight = 20f;

    [Min(0.01f)]
    public float terrainOffset = 0.03f;

    [Header("Layer")]
    public LayerMask terrainLayer = ~0;

    [Header("Editor")]
    public bool showGrid = true;

    private Transform gridContainer;

    private void OnEnable()
    {
        GenerateGrid();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    GenerateGrid();
                }
            };
#endif
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        // Mantém o preview atualizado no Editor.
        if (transform.hasChanged)
        {
            GenerateGrid();
            transform.hasChanged = false;
        }
    }

    public void GenerateGrid()
    {
        ClearGrid();

        if (!showGrid)
            return;

        // Se estiver configurado para auto-size, adapta ao terreno ativo
        if (autoSizeToTerrain && Terrain.activeTerrain != null)
        {
            Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
            gridWidth = Mathf.FloorToInt(terrainSize.x / tileSize);
            gridHeight = Mathf.FloorToInt(terrainSize.z / tileSize);

            // TRAVA DE SEGURANÇA para o preview não travar a Unity
            if (gridWidth > 150) gridWidth = 150;
            if (gridHeight > 150) gridHeight = 150;

            // Move a base do preview para a origem do terreno
            transform.position = Terrain.activeTerrain.transform.position;
        }

        GameObject container = new GameObject("GridLines");
        container.transform.SetParent(transform);
        container.transform.localPosition = gridOffset;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        gridContainer = container.transform;

        // Linhas verticais
        for (int x = 0; x <= gridWidth; x++)
        {
            Vector3 start = GetGridPosition(x, 0);
            Vector3 end = GetGridPosition(x, gridHeight);

            CreateLine(
                "Vertical_" + x,
                start,
                end
            );
        }

        // Linhas horizontais
        for (int y = 0; y <= gridHeight; y++)
        {
            Vector3 start = GetGridPosition(0, y);
            Vector3 end = GetGridPosition(gridWidth, y);

            CreateLine(
                "Horizontal_" + y,
                start,
                end
            );
        }
    }

    private Vector3 GetGridPosition(int x, int y)
    {
        Vector3 localPosition = new Vector3(
            x * tileSize,
            0f,
            y * tileSize
        );

        Vector3 worldPosition =
            transform.TransformPoint(localPosition + gridOffset);

        if (followTerrain)
        {
            Ray ray = new Ray(
                worldPosition + Vector3.up * raycastHeight,
                Vector3.down
            );

            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                raycastHeight * 2f,
                terrainLayer
            ))
            {
                worldPosition = hit.point +
                                hit.normal * terrainOffset;
            }
        }

        return worldPosition;
    }

    private void CreateLine(
        string lineName,
        Vector3 start,
        Vector3 end
    )
    {
        GameObject lineObject = new GameObject(lineName);

        lineObject.transform.SetParent(
            gridContainer,
            true
        );

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.positionCount = 2;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.useWorldSpace = true;

        line.startColor = gridColor;
        line.endColor = gridColor;

        line.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        line.receiveShadows = false;

        line.alignment = LineAlignment.View;

        // Material simples para o preview
        Material material = new Material(
            Shader.Find("Sprites/Default")
        );

        material.color = gridColor;

        line.material = material;
    }

    private void ClearGrid()
    {
        Transform existingGrid =
            transform.Find("GridLines");

        if (existingGrid != null)
        {
            if (Application.isPlaying)
            {
                Destroy(existingGrid.gameObject);
            }
            else
            {
                DestroyImmediate(existingGrid.gameObject);
            }
        }

        gridContainer = null;
    }

    private void OnDestroy()
    {
        ClearGrid();
    }
}