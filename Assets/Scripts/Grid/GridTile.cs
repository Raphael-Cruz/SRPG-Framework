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

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        defaultColor = tileRenderer.material.color;
    }

    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;

        gameObject.name = $"Tile ({X}, {Y})";


    }

    public void Highlight(Color color)
    {
        tileRenderer.material.color = color;
    }

    public void ResetColor()
    {
        tileRenderer.material.color = defaultColor;
    }



public void SetOccupant(Unit unit)
{
    occupant = unit;
}



public void ClearOccupant()
{
    occupant = null;
}
    
public Vector3 GetCursorPosition(float tileOffset, float unitOffset)
{
if (occupant == null)
{
    return WorldPosition + Vector3.up * tileOffset;
}

Renderer renderer = occupant.GetComponentInChildren<Renderer>();
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