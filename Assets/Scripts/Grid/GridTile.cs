using UnityEngine;

public class GridTile : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Vector3 WorldPosition => transform.position;


    private Renderer tileRenderer;
    private Color defaultColor;

    private Unit occupant;

    public Unit Occupant => occupant;


    // Highlight states
    private bool isSelected;
    private bool isMovementRange;
    private bool isAttackRange;
    private bool isHealingRange;
    private bool isBlocked;


    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();

        if (tileRenderer != null)
        {
            defaultColor = tileRenderer.material.color;
        }
    }


    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;

        gameObject.name = $"Tile ({X}, {Y})";
    }



    // ==========================
    // Highlight System
    // ==========================

    public void SetSelected(bool value)
    {
        isSelected = value;
        UpdateColor();
    }


    public void SetMovementRange(bool value)
    {
        isMovementRange = value;
        UpdateColor();
    }


    public void SetAttackRange(bool value)
    {
        isAttackRange = value;
        UpdateColor();
    }


    public void SetHealingRange(bool value)
    {
        isHealingRange = value;
        UpdateColor();
    }


    public void SetBlocked(bool value)
    {
        isBlocked = value;
        UpdateColor();
    }


    public void ClearHighlights()
    {
        isSelected = false;
        isMovementRange = false;
        isAttackRange = false;
        isHealingRange = false;
        isBlocked = false;

        UpdateColor();
    }


    private void UpdateColor()
    {
        if (tileRenderer == null)
            return;


        // Priority order

        if (isBlocked)
        {
            tileRenderer.material.color = Color.gray;
            return;
        }


        if (isAttackRange)
        {
            tileRenderer.material.color = Color.red;
            return;
        }


        if (isHealingRange)
        {
            tileRenderer.material.color = Color.green;
            return;
        }


        if (isMovementRange)
        {
            tileRenderer.material.color = Color.blue;
            return;
        }


        if (isSelected)
        {
            tileRenderer.material.color = Color.yellow;
            return;
        }


        tileRenderer.material.color = defaultColor;
    }



    // ==========================
    // Unit Occupancy
    // ==========================

    public void SetOccupant(Unit unit)
    {
        occupant = unit;
    }


    public void ClearOccupant()
    {
        occupant = null;
    }



    // ==========================
    // Cursor Position
    // ==========================

    public Vector3 GetCursorPosition(float tileOffset, float unitOffset)
    {
        if (occupant == null)
        {
            return WorldPosition + Vector3.up * tileOffset;
        }


        Renderer renderer =
            occupant.GetComponentInChildren<Renderer>();


        if (renderer != null)
        {
            return new Vector3(
                WorldPosition.x,
                renderer.bounds.max.y + unitOffset,
                WorldPosition.z
            );
        }


        return WorldPosition + Vector3.up * tileOffset;
    }
}