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


private void Update()
{
    if(State == MovementState.Previewing)
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            ConfirmMove();
        }
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


    Debug.Log(
        $"Preview move to ({tile.X},{tile.Y})"
    );
}




public void ConfirmMove()
{
    if(State != MovementState.Previewing)
        return;


    movingUnit.MoveTo(previewTile);


    State = MovementState.MovementCommitted;


    movementRange.ClearMovementRange();


    OnMovementConfirmed?.Invoke(movingUnit);


    Debug.Log(
        $"Movement confirmed to ({previewTile.X},{previewTile.Y})"
    );
}



public void CancelMove()
{
    if(State != MovementState.Previewing)
        return;


    movingUnit.transform.position =
        originalTile.WorldPosition;


    previewTile = null;


    State = MovementState.SelectingDestination;


    movementRange.ShowMovementRange(movingUnit);


    Debug.Log("Movement cancelled");
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