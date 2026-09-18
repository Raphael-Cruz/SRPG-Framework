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

    public bool IsWalkable => !isBlocked && occupant == null;


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

        // Shrink slightly to create the gap effect between tiles (like Fire Emblem)
        // If your tile is 1x1, scaling it to 0.95 gives a 5% margin around it.
        transform.localScale = new Vector3(0.9f, transform.localScale.y, 0.9f);
    }


    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;

        gameObject.name = $"Tile ({X}, {Y})";

        // Garante que o tile comece invisível, mostrando apenas o terreno
        ClearHighlights();
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

        // Por padrão, esconde a malha para não cobrir o terreno
        tileRenderer.enabled = false;

        // Priority order
        
        if (isSelected)
        {
            tileRenderer.enabled = true;
            tileRenderer.material.color = new Color(1f, 1f, 0f, 0.6f); // Yellow with alpha
            return;
        }

        if (isAttackRange)
        {
            tileRenderer.enabled = true;
            tileRenderer.material.color = new Color(1f, 0f, 0f, 0.5f); // Red with alpha
            return;
        }

        if (isHealingRange)
        {
            tileRenderer.enabled = true;
            tileRenderer.material.color = new Color(0f, 1f, 0f, 0.5f); // Green with alpha
            return;
        }

        if (isMovementRange)
        {
            tileRenderer.enabled = true;
            tileRenderer.material.color = new Color(0f, 0.4f, 1f, 0.5f); // Blue with alpha
            return;
        }

        // isBlocked normalmente não deve ser desenhado permanentemente para não deixar
        // a cena cheia de quadrados cinzas nas árvores. Mas se você quiser ver os bloqueios
        // enquanto depura o jogo, você pode descomentar as linhas abaixo:
        /*
        if (isBlocked)
        {
            tileRenderer.enabled = true;
            tileRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Cinza semitransparente
            return;
        }
        */

        // Se chegou aqui, nenhuma cor especial está ativa. 
        // A malha continuará invisível (enabled = false).
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