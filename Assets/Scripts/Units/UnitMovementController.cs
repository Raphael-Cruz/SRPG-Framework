using UnityEngine;
using System;
public class UnitMovementController : MonoBehaviour
{
    [SerializeField] private MovementRangeCalculator movementRange;

    private Unit movingUnit;

    private GridTile originalTile;
    private GridTile previewTile;


    public MovementState State { get; private set; }
        = MovementState.None;

public event Action<Unit> OnMovementConfirmed;


// Fired when a tile is previewed (Confirm button should appear) and when
// that preview ends, whether confirmed or cancelled (Confirm button
// should disappear). Kept separate from OnMovementConfirmed since a UI
// button caring "is there a preview to confirm right now" needs to know
// about cancellation too, not just confirmation.
public event Action<Unit> OnPreviewStarted;
public event Action OnPreviewEnded;


private void Start()
{
    InputManager.Instance.ConfirmPressed += HandleConfirmPressed;
}


private void OnDisable()
{
    if (InputManager.Instance != null)
    {
        InputManager.Instance.ConfirmPressed -= HandleConfirmPressed;
    }
}


private void HandleConfirmPressed()
{
    if (State == MovementState.Previewing)
    {
        ConfirmMove();
    }
}
    public void BeginMovement(Unit unit)
    {
        if(unit == null)
            return;


        if(!unit.CanMove)
        {
            Debug.Log("Unit already moved.");
            return;
        }


        movingUnit = unit;

        originalTile = unit.CurrentTile;
        previewTile = null;


        State = MovementState.SelectingDestination;


        movementRange.ShowMovementRange(unit);


        Debug.Log($"{unit.name} choosing movement");
    }




private void PreviewMove(GridTile tile)
{
    previewTile = tile;


    movingUnit.SetPreviewTile(tile);


    movingUnit.transform.position =
        tile.WorldPosition;


    movementRange.ClearMovementRange();


    State = MovementState.Previewing;
      Debug.Log("Invoking Preview Started");


    OnPreviewStarted?.Invoke(movingUnit);


    Debug.Log(
        $"Preview move to ({tile.X},{tile.Y})"
    );
}




public void ConfirmMove()
{
    if(State != MovementState.Previewing)
        return;


    movingUnit.MoveTo(previewTile);

    movingUnit.ClearPreviewTile();


    movementRange.ClearMovementRange();


    Debug.Log(
        $"Movement confirmed to ({previewTile.X},{previewTile.Y})"
    );

Debug.Log("UnitMovementController -> ConfirmMove");
    OnMovementConfirmed?.Invoke(movingUnit);

    OnPreviewEnded?.Invoke();


    // Without this, State stays MovementCommitted forever and future code
    // has to remember that "MovementCommitted" actually means "idle".
    movingUnit = null;
    previewTile = null;
    originalTile = null;

    State = MovementState.None;
}



public bool CancelMove()
{
    if(State != MovementState.Previewing)
        return false;


    movingUnit.transform.position =
        originalTile.WorldPosition;


    movingUnit.ClearPreviewTile();

    previewTile = null;


    State = MovementState.SelectingDestination;


    movementRange.ShowMovementRange(movingUnit);


    Debug.Log("Movement cancelled");

    OnPreviewEnded?.Invoke();

    return true;
}

public void HandleTileClick(GridTile clickedTile)
{
    if (State != MovementState.SelectingDestination)
        return;

    if (clickedTile == null)
        return;

    if (!movementRange.IsReachable(movingUnit, clickedTile))
    {
        Debug.Log("Invalid movement.");
        return;
    }

    PreviewMove(clickedTile);
}

}