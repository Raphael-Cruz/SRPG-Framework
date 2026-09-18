using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private float heightOffset = 0.05f; // Flat on the ground
    
    [Header("Visuals")]
    [Tooltip("Assign a GameObject (Prefab or Scene Object) with an Arrow here. A copy will be made automatically.")]
    [SerializeField] private GameObject arrowHeadTemplate;
    
    private GameObject arrowHeadInstance;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        
        if (arrowHeadTemplate != null)
        {
            // Create a dedicated copy for the path so we don't steal the cursor's arrow!
            arrowHeadInstance = Instantiate(arrowHeadTemplate, transform);
            arrowHeadInstance.SetActive(false);
            
            // If the user passed the scene cursor as a template, it stays intact.
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

        // Setup Arrow Head
        if (arrowHeadInstance != null)
        {
            arrowHeadInstance.SetActive(true);
            
            // Put it at the last tile
            Vector3 finalPos = path[path.Count - 1].WorldPosition;
            finalPos.y += heightOffset + 0.01f; // slightly above the line to prevent z-fighting
            arrowHeadInstance.transform.position = finalPos;

            // Rotate it to face the direction of the last step
            Vector3 prevPos = path[path.Count - 2].WorldPosition;
            Vector3 dir = (finalPos - prevPos).normalized;
            
            if (dir != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(dir);
                arrowHeadInstance.transform.rotation = rotation;
            }
        }
    }

    public void ClearPath()
    {
        lineRenderer.positionCount = 0;
        if (arrowHeadInstance != null)
        {
            arrowHeadInstance.SetActive(false);
        }
    }
}
