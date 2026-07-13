using UnityEngine;

public class UnitMovementController : MonoBehaviour
{
[SerializeField] private SelectionManager selectionManager;
[SerializeField] private MovementRangeCalculator movementRange;

private void Start()
{
    InputManager.Instance.LeftClick += TryMove;
}


private void OnDisable()
{
    if (InputManager.Instance != null)
    {
        InputManager.Instance.LeftClick -= TryMove;
    }
}


    private void TryMove()
    {
        Unit unit = selectionManager.SelectedUnit;
   


        if (unit == null)
            return;


        GridTile clickedTile = GetClickedTile();


        if (clickedTile == null)
            return;


     if (!movementRange.IsReachable(unit, clickedTile))
{
    Debug.Log(
        $"Cannot move from ({unit.CurrentTile.X},{unit.CurrentTile.Y}) " +
        $"to ({clickedTile.X},{clickedTile.Y})"
    );

    return;
}


       unit.MoveTo(clickedTile);
       selectionManager.DeselectCurrentUnit();

movementRange.ClearMovementRange();
    }



    private GridTile GetClickedTile()
    {
        Ray ray =
            Camera.main.ScreenPointToRay(
                InputManager.Instance.MousePosition
            );


        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<GridTile>();
        }


        return null;
    }



    private bool IsNeighbor(GridTile current, GridTile target)
    {
        if (current == null)
            return false;


        return GridManager.Instance
            .GetNeighbors(current)
            .Contains(target);
    }
}