using UnityEngine;

public class HoverMovementPreview : MonoBehaviour
{
    [SerializeField] private MouseSelector mouseSelector;
    [SerializeField] private MovementRangeCalculator movementRange;
    

    private Unit hoveredUnit;


    private void OnEnable()
    {
        mouseSelector.HoveredTileChanged += HandleHoverChanged;
    }


    private void OnDisable()
    {
        if(mouseSelector != null)
            mouseSelector.HoveredTileChanged -= HandleHoverChanged;
    }


   private void HandleHoverChanged(GridTile tile)
{
    if(SelectionManager.Instance != null &&
       SelectionManager.Instance.SelectedUnit != null)
    {
        return;
    }


    ClearPreview();


        if(tile == null)
            return;


        Unit unit = tile.Occupant;


        if(unit == null)
            return;


        // Only preview the active player's unit
        if(!unit.IsPlayerControlled)
            return;


        // Don't show if the unit already moved
        if(!unit.CanMove)
            return;


     


        hoveredUnit = unit;

        movementRange.ShowMovementRange(unit);
    }


    private void ClearPreview()
    {
        movementRange.ClearMovementRange();
        hoveredUnit = null;
    }
}