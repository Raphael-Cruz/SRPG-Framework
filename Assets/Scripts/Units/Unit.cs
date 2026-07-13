using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private UnitData data;

    [Header("Runtime State")]
    [SerializeField] private int currentHP;
      private UnitVisual visual;

    public UnitVisual Visual => visual;


    private GridTile currentTile;

    private bool hasMoved;
    private bool hasActed;


    public UnitData Data => data;

    public int CurrentHP => currentHP;


public GridTile CurrentTile => currentTile;


    public bool HasMoved => hasMoved;

    public bool HasActed => hasActed;

    public bool CanMove => !hasMoved;

    public bool CanAct => !hasActed;

    public bool IsAlive => currentHP > 0;





    private void Awake()
    {
        currentHP = data.MaxHP;
         visual = GetComponent<UnitVisual>();
    }


    public void ResetTurn()
    {
        hasMoved = false;
        hasActed = false;
    }


    public void MoveUsed()
    {
        hasMoved = true;
    }


    public void ActionUsed()
    {
        hasActed = true;
    }


public void SetTile(GridTile tile)
{
    currentTile = tile;
    tile.SetOccupant(this);
}

public void Select()
{
    // visual feedback later
}

public void Deselect()
{
    // remove visual feedback later
}

}