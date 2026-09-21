
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private float heightOffset = 0.05f;

    [Header("Glow Effect")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;

    private Material lineMaterial;
    private Color baseLineColor;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        if (lineRenderer != null)
        {
            lineMaterial = lineRenderer.material;

            if (lineMaterial != null)
            {
                baseLineColor = lineMaterial.color;
            }
        }
    }

    private void Update()
    {
        if (enableGlow &&
            lineRenderer.positionCount > 0 &&
            lineMaterial != null)
        {
            float t = Mathf.PingPong(Time.time * glowSpeed, 1f);
            float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            Color newColor = baseLineColor;
            newColor.a = currentAlpha;

            lineMaterial.color = newColor;
        }
    }

    public void DrawPath(List<GridTile> path)
    {
        if (path == null || path.Count < 2)
        {
            ClearPath();
            return;
        }

        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = path[i].WorldPosition;
            pos.y += heightOffset;

            lineRenderer.SetPosition(i, pos);
        }
    }

    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
    }
}
