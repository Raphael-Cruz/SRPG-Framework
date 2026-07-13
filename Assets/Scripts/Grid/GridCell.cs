using UnityEngine;

/// <summary>
/// Represents a single cell on the tactical grid.
/// This class stores game data only—it does not render anything.
/// </summary>
public class GridCell
{
    public int X { get; }
    public int Y { get; }

    public bool Walkable { get; set; } = true;
    public bool Occupied { get; set; } = false;

    public GridCell(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}