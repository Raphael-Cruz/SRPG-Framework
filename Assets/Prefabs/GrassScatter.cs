using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GrassScatter : MonoBehaviour
{
    [Header("Grass")]
    public Mesh grassMesh;
    public Material grassMaterial;

    [Header("Area")]
    public float areaWidth = 50f;
    public float areaLength = 50f;

    [Header("Density")]
    [Min(1)]
    public int instanceCount = 5000;

    [Header("Scale")]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);

    [Header("Rotation")]
    public bool randomRotation = true;

    [Header("Terrain")]
    public bool followTerrain = true;
    public LayerMask terrainLayer = ~0;
    public float raycastHeight = 100f;
    public float terrainOffset = 0.02f;

    [Header("Random")]
    public int seed = 12345;

    [Header("Rendering")]
    public float renderDistance = 100f;
    public ShadowCastingMode shadowCastingMode =
        ShadowCastingMode.Off;

    public bool receiveShadows = false;

    private const int MaxInstancesPerBatch = 1023;

    private readonly List<Matrix4x4[]> batches =
        new List<Matrix4x4[]>();

#if UNITY_EDITOR
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        GenerateGrass();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
#else
    private void OnEnable()
    {
        GenerateGrass();
    }
#endif

    private void OnValidate()
    {
        GenerateGrass();

#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
    }

    // =========================================================
    // GENERATE
    // =========================================================

    [ContextMenu("Generate Grass")]
    public void GenerateGrass()
    {
        batches.Clear();

        if (grassMesh == null)
        {
            Debug.LogWarning(
                "GrassScatter: Grass Mesh is missing.",
                this
            );

            return;
        }

        if (grassMaterial == null)
        {
            Debug.LogWarning(
                "GrassScatter: Grass Material is missing.",
                this
            );

            return;
        }

        grassMaterial.enableInstancing = true;

        Random.InitState(seed);

        List<Matrix4x4> currentBatch =
            new List<Matrix4x4>(
                MaxInstancesPerBatch
            );

        int generated = 0;

        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 position =
                GetRandomPosition();

            if (followTerrain)
            {
                if (!TryGetTerrainPosition(
                    ref position))
                {
                    continue;
                }
            }

            float scale =
                Random.Range(
                    scaleRange.x,
                    scaleRange.y
                );

            Quaternion rotation =
                randomRotation
                ? Quaternion.Euler(
                    0f,
                    Random.Range(0f, 360f),
                    0f
                )
                : Quaternion.identity;

            Matrix4x4 matrix =
                Matrix4x4.TRS(
                    position,
                    rotation,
                    Vector3.one * scale
                );

            currentBatch.Add(matrix);

            generated++;

            if (currentBatch.Count >=
                MaxInstancesPerBatch)
            {
                batches.Add(
                    currentBatch.ToArray()
                );

                currentBatch.Clear();
            }
        }

        if (currentBatch.Count > 0)
        {
            batches.Add(
                currentBatch.ToArray()
            );
        }

        Debug.Log(
            $"GrassScatter: Generated {generated} grass instances in {batches.Count} batches.",
            this
        );

#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
    }

    // =========================================================
    // POSITION
    // =========================================================

    private Vector3 GetRandomPosition()
    {
        float x =
            Random.Range(
                -areaWidth * 0.5f,
                areaWidth * 0.5f
            );

        float z =
            Random.Range(
                -areaLength * 0.5f,
                areaLength * 0.5f
            );

        return transform.position +
               new Vector3(
                   x,
                   raycastHeight,
                   z
               );
    }

    private bool TryGetTerrainPosition(
        ref Vector3 position)
    {
        Ray ray =
            new Ray(
                position,
                Vector3.down
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            raycastHeight * 2f,
            terrainLayer,
            QueryTriggerInteraction.Ignore))
        {
            position =
                hit.point +
                hit.normal * terrainOffset;

            return true;
        }

        return false;
    }

    // =========================================================
    // PLAY MODE
    // =========================================================

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        DrawGrass(Camera.main);
    }

    // =========================================================
    // SCENE VIEW
    // =========================================================

#if UNITY_EDITOR

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying)
            return;

        if (sceneView == null)
            return;

        DrawGrass(sceneView.camera);
    }

#endif

    // =========================================================
    // DRAW
    // =========================================================

    private void DrawGrass(Camera camera)
    {
        if (camera == null)
            return;

        if (grassMesh == null)
            return;

        if (grassMaterial == null)
            return;

        if (batches.Count == 0)
            return;

        float distanceSqr =
            (
                transform.position -
                camera.transform.position
            ).sqrMagnitude;

        if (distanceSqr >
            renderDistance * renderDistance)
        {
            return;
        }

        for (int i = 0;
             i < batches.Count;
             i++)
        {
            Matrix4x4[] batch =
                batches[i];

            if (batch == null ||
                batch.Length == 0)
                continue;

            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                batch,
                batch.Length,
                null,
                shadowCastingMode,
                receiveShadows,
                gameObject.layer,
                camera
            );
        }
    }

    // =========================================================
    // CLEAR
    // =========================================================

    [ContextMenu("Clear Grass")]
    public void ClearGrass()
    {
        batches.Clear();

#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
    }

    // =========================================================
    // GIZMO
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                areaWidth,
                0.1f,
                areaLength
            )
        );
    }
}