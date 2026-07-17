using UnityEngine;

public class TacticalCursor : MonoBehaviour
{
    [SerializeField] private MouseSelector mouseSelector;

    [SerializeField] private TileMarkerController tileMarker;
    [SerializeField] private ArrowController arrow;
    [SerializeField] private UnitAttackController attackController;

    private void OnEnable()
    {
        mouseSelector.HoveredTileChanged += HandleHoveredTileChanged;
    }

    private void OnDisable()
    {
        if (mouseSelector != null)
            mouseSelector.HoveredTileChanged -= HandleHoveredTileChanged;
    }

private void HandleHoveredTileChanged(GridTile tile)
{
    if(tile == null)
        return;


    if(UnitActionController.Instance.State ==
       UnitActionState.SelectingAttackTarget)
    {
        if(!attackController.CanHoverTarget(tile))
            return;
    }


    tileMarker.SetTarget(tile);
    arrow.SetTarget(tile);
}
}