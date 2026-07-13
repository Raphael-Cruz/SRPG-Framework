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

 
    public bool TurnFinished => hasMoved && hasActed;

    public bool CanBeSelected => !TurnFinished;





    private void Awake()
    {
        currentHP = data.MaxHP;
         visual = GetComponent<UnitVisual>();
    }


    public void ResetTurn()
    {
        hasMoved = false;
        hasActed = false;

        RefreshVisualState();
    }


    public void MoveUsed()
    {
        hasMoved = true;

        RefreshVisualState();
    }


    public void ActionUsed()
    {
        hasActed = true;

        RefreshVisualState();
    }


    // Single place responsible for keeping the visual in sync with
    // gameplay state. As more ways to spend a turn are added (attack,
    // wait, heal, cast, item, rescue, trade...), each one just needs to
    // update its own flag and call this - no need to touch the visual
    // system from every action method individually.
    private void RefreshVisualState()
    {
        visual?.SetExhausted(TurnFinished);
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

public void MoveTo(GridTile targetTile)
{
    if (!CanMove)
    {
        Debug.Log($"{name} already moved this turn.");
        return;
    }

    if (targetTile.Occupant != null)
    {
        Debug.Log("Tile occupied.");
        return;
    }


    if (currentTile != null)
    {
        currentTile.ClearOccupant();
    }


    transform.position = targetTile.WorldPosition;


    currentTile = targetTile;
    targetTile.SetOccupant(this);



    // No attack/action step exists yet, so for now moving spends the
    // whole turn. When a real action system is added, remove this line
    // and have that system call ActionUsed() (or not, for units that
    // choose to wait) instead - TurnFinished/CanBeSelected/RefreshVisualState
    // don't need to change at all.
  
    FinishTurn();

    Debug.Log(
        $"{name} moved to ({targetTile.X},{targetTile.Y})"
    );
}

public void FinishTurn()
{
    
  MoveUsed();
    ActionUsed();

   
}

}