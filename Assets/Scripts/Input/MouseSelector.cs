using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectionManager selectionManager;

    private GridTile currentTile;

    public event Action<GridTile> HoveredTileChanged;

    public GridTile CurrentTile => currentTile;


    private void Update()
    {
        DetectTile();

        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }


    private void HandleClick()
    {
        // Ignore gameplay clicks when interacting with UI
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }


        if (currentTile == null)
            return;



        Unit unit = currentTile.Occupant;



        // ==============================
        // Attack targeting has priority
        // ==============================
      if(UnitActionController.Instance.State == UnitActionState.SelectingAttackTarget)
{
    UnitActionController.Instance.ConfirmAttackTarget();
    return;
}



        // ==============================
        // Clicking a unit has priority
        // over movement routing.
        //
        // This allows:
        // Select unit
        // -> movement mode
        // -> click same unit
        // -> open action menu
        // ==============================
        if (unit != null)
        {
            selectionManager.SelectUnit(unit);
            return;
        }



        // ==============================
        // Tile actions
        // ==============================
        if (UnitActionController.Instance != null)
        {
            switch (UnitActionController.Instance.State)
            {
                case UnitActionState.Moving:

                    UnitActionController.Instance.TryMove(currentTile);
                    return;


                case UnitActionState.SelectingAction:
                case UnitActionState.None:
                case UnitActionState.Finished:
                    break;
            }
        }


        Debug.Log("No valid action on this tile.");
    }



    private void DetectTile()
    {
        GridTile tile = GetTileUnderMouse();

        if (tile == currentTile)
            return;

        currentTile = tile;

        HoveredTileChanged?.Invoke(currentTile);
       
        if(UnitActionController.Instance != null &&
            UnitActionController.Instance.State ==
            UnitActionState.SelectingAttackTarget)
            {
            UnitActionController.Instance.ChangeAttackTarget(currentTile);
            }
    }



    private GridTile GetTileUnderMouse()
    {
        if (mainCamera == null)
            return null;


        Ray ray = mainCamera.ScreenPointToRay(
            InputManager.Instance.MousePosition
        );


        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return GridManager.Instance.GetTileFromWorldPosition(hit.point);
        }


        return null;
    }
}