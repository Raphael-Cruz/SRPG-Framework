using UnityEngine;
using System;

public class MouseSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectionManager selectionManager;
    

    private GridTile currentTile;

public event Action<GridTile> HoveredTileChanged;

public GridTile CurrentTile => currentTile;


private void Start()
{
    InputManager.Instance.LeftClick += HandleClick;
}

private void OnDisable()
{
    if (InputManager.Instance != null)
        InputManager.Instance.LeftClick -= HandleClick;
}

    private void Update()
    {
        DetectTile();
    }

private void HandleClick()
{
    if (currentTile == null)
        return;


    // MouseSelector routes the click depending on the state.
  if(UnitActionController.Instance != null)
{
    switch(UnitActionController.Instance.State)
    {
        case UnitActionState.SelectingAction:
            break;

        case UnitActionState.Moving:
            UnitActionController.Instance.TryMove(currentTile);
            return;

        case UnitActionState.SelectingAttackTarget:
            UnitActionController.Instance.TryAttack(currentTile);
            return;
    }
}



    Unit unit = currentTile.Occupant;


    if (unit == null)
    {
        Debug.Log("No unit on this tile.");
        return;
    }


    selectionManager.SelectUnit(unit);
}

private void DetectTile()
{
    GridTile tile = GetTileUnderMouse();

    if(tile == null)
        return;

    if(currentTile != tile)
    {
        currentTile = tile;

        HoveredTileChanged?.Invoke(currentTile);
    }
}

    private GridTile GetTileUnderMouse()
{
    Ray ray = mainCamera.ScreenPointToRay(
        InputManager.Instance.MousePosition
    );

    if(Physics.Raycast(ray, out RaycastHit hit))
    {
        return GridManager.Instance
            .GetTileFromWorldPosition(hit.point);
    }

    return null;
}
}