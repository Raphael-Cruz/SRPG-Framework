using UnityEngine;

public class TacticalCursor : MonoBehaviour
{
    [SerializeField] private MouseSelector mouseSelector;

    [SerializeField] private TileMarkerController tileMarker;
    [SerializeField] private ArrowController arrow;

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
        if (tile == null)
            return;

        tileMarker.SetTarget(tile);
        arrow.SetTarget(tile);
    }
}