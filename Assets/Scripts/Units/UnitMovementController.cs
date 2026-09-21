using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnitMovementController : MonoBehaviour
{
    public static UnitMovementController Instance { get; private set; }

    [SerializeField] private MovementRangeCalculator movementRange;
    [SerializeField] private PathRenderer pathRenderer;

    [Tooltip("Tiles per second the unit walks.")]
    [SerializeField] private float moveSpeed = 6f;

    private Unit movingUnit;
    private GridTile originalTile;
    private GridTile selectedTile;
    private List<GridTile> cachedPath;

    public MovementState State { get; private set; } = MovementState.None;

    public event Action<Unit> OnMovementConfirmed;
    public event Action<Unit> OnPreviewStarted;
    public event Action OnPreviewEnded;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        InputManager.Instance.ConfirmPressed += HandleConfirmPressed;

        MouseSelector mouseSelector = FindObjectOfType<MouseSelector>();
        if (mouseSelector != null)
            mouseSelector.HoveredTileChanged += HandleHoveredTileChanged;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.ConfirmPressed -= HandleConfirmPressed;

        MouseSelector mouseSelector = FindObjectOfType<MouseSelector>();
        if (mouseSelector != null)
            mouseSelector.HoveredTileChanged -= HandleHoveredTileChanged;
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void BeginMovement(Unit unit)
    {
        Debug.Log("UnitMovementController.BeginMovement");
        if (unit == null || !unit.CanMove) return;

        movingUnit   = unit;
        originalTile = unit.CurrentTile;
        selectedTile = null;
        cachedPath   = null;

        State = MovementState.SelectingDestination;
        movementRange.ShowMovementRange(unit);
    }

    public void HandleTileClick(GridTile clickedTile)
    {
        if (State != MovementState.SelectingDestination) return;
        if (clickedTile == null) return;
        if (!movementRange.IsReachable(movingUnit, clickedTile))
        {
            Debug.Log("Invalid movement tile.");
            return;
        }

        SelectDestination(clickedTile);
    }

    public void ConfirmMove()
    {
        Debug.Log($"ConfirmMove called. State={State}, cachedPath={(cachedPath == null ? "NULL" : cachedPath.Count.ToString())}");
        if (State != MovementState.Previewing) return;

        Unit           unit        = movingUnit;
        GridTile       destination = selectedTile;
        List<GridTile> path        = cachedPath;

        if (path == null)
        {
            Debug.LogError("ConfirmMove: cachedPath is NULL! Aborting.");
            return;
        }

        // Clear state BEFORE the coroutine
        movingUnit   = null;
        selectedTile = null;
        cachedPath   = null;
        State        = MovementState.None;

        OnPreviewEnded?.Invoke();

        Debug.Log($"Starting WalkRoutine for {unit.name} with {path.Count} steps.");
        StartCoroutine(WalkRoutine(unit, destination, path));
    }

    public bool CancelMove()
    {
        if (State == MovementState.None) return false;

        if (pathRenderer != null) pathRenderer.ClearPath();

        if (State == MovementState.Previewing)
        {
            movingUnit.ClearPreviewTile();
            selectedTile = null;
            cachedPath   = null;

            movementRange.ShowMovementRange(movingUnit);
            State = MovementState.SelectingDestination;
            OnPreviewEnded?.Invoke();
            return true;
        }

        if (State == MovementState.SelectingDestination)
        {
            movementRange.ClearMovementRange();
            movingUnit   = null;
            selectedTile = null;
            originalTile = null;
            cachedPath   = null;
            State        = MovementState.None;
            return true;
        }

        return false;
    }

    public void ResumeMovement()
    {
        if (movingUnit == null)
            movingUnit = UnitActionController.Instance.SelectedUnit;
        if (movingUnit == null || !movingUnit.CanMove) return;

        originalTile = movingUnit.CurrentTile;
        selectedTile = null;
        cachedPath   = null;

        movementRange.ShowMovementRange(movingUnit);
        State = MovementState.SelectingDestination;

        Debug.Log($"{movingUnit.name} returned to movement selection.");
    }

    // -------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------

    private void HandleHoveredTileChanged(GridTile tile)
    {
        if (State != MovementState.SelectingDestination) return;

        if (tile == null || !movementRange.IsReachable(movingUnit, tile))
        {
            if (pathRenderer != null) pathRenderer.ClearPath();
            return;
        }

        if (pathRenderer != null)
            pathRenderer.DrawPath(BuildDisplayPath(tile));
    }

    private void HandleConfirmPressed()
    {
        if (State == MovementState.Previewing) ConfirmMove();
    }

    private void SelectDestination(GridTile tile)
    {
        if (pathRenderer != null) pathRenderer.ClearPath();

        // Build and cache path NOW — before ClearMovementRange wipes cameFrom data!
        List<GridTile> raw = movementRange.GetPath(tile);
        // GetPath includes the origin tile. Remove it — unit is already there.
        if (raw.Count > 0 && raw[0] == originalTile)
            raw.RemoveAt(0);
        cachedPath   = raw;
        selectedTile = tile;

        movingUnit.SetPreviewTile(tile);
        movementRange.ClearMovementRange();
        State = MovementState.Previewing;

        Debug.Log($"Destination selected ({tile.X},{tile.Y}), walk steps = {cachedPath.Count}");
        OnPreviewStarted?.Invoke(movingUnit);
    }

    /// Path including origin for the path-renderer display (shows full line on hover).
    private List<GridTile> BuildDisplayPath(GridTile target)
    {
        List<GridTile> path = movementRange.GetPath(target);
        if (path.Count > 0 && path[0] != originalTile)
            path.Insert(0, originalTile);
        return path;
    }

    private IEnumerator WalkRoutine(Unit unit, GridTile destination, List<GridTile> path)
    {
        Debug.Log($"WalkRoutine start: {unit.name}, steps = {path.Count}");

        if (path.Count > 0)
        {
            unit.Visual?.SetWalking(true);

            GridTile currentStepTile = unit.CurrentTile;

            foreach (GridTile nextTile in path)
            {
                if (nextTile == currentStepTile) continue;

                // --- AOO CHECK ---
                List<Unit> threateningEnemies = unit.GetThreateningEnemies(currentStepTile);
                if (threateningEnemies.Count > 0)
                {
                    Debug.Log($"[AoO] {unit.name} saindo de {currentStepTile.X},{currentStepTile.Y} ameaçado por {threateningEnemies.Count} inimigos.");
                }

                foreach (Unit enemy in threateningEnemies)
                {
                    bool nextTileThreatened = unit.Threatens(enemy, nextTile);
                    if (!nextTileThreatened)
                    {
                        Debug.Log($"[AoO] TRIGGER! {unit.name} tomou Ataque de Oportunidade de {enemy.name}!");
                        unit.Visual?.SetWalking(false);
                        yield return CombatSystem.Instance.ExecuteAoORoutine(enemy, unit);
                        if (!unit.IsAlive) break;
                        unit.Visual?.SetWalking(true);
                    }
                }

                if (!unit.IsAlive) break;

                Vector3 targetPos = nextTile.WorldPosition;

                // Face the direction of movement instantly
                Vector3 dir = targetPos - unit.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    unit.transform.rotation = Quaternion.LookRotation(dir.normalized);

                // Slide to tile
                while (Vector3.Distance(unit.transform.position, targetPos) > 0.01f)
                {
                    unit.transform.position = Vector3.MoveTowards(
                        unit.transform.position, targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                unit.transform.position = targetPos;
                currentStepTile = nextTile;
            }

            unit.Visual?.SetWalking(false);
        }

        if (unit.IsAlive)
        {
            // Post-movement facing
            unit.FaceClosestEnemy();

            // Commit tile occupancy & state (does NOT teleport — position is already correct)
            unit.CommitMove(destination);

            ApplyTerrainModifier(unit, destination);

            Debug.Log($"WalkRoutine done. Firing OnMovementConfirmed.");
            OnMovementConfirmed?.Invoke(unit);
        }
        else
        {
            // Unit died during movement
            OnMovementConfirmed?.Invoke(unit);
        }
    }

    private void ApplyTerrainModifier(Unit unit, GridTile tile)
    {
        CombatModifier terrainMod = null;
        if (tile != null && tile.gameObject.name.Contains("Grass"))
            terrainMod = new GrassTerrainModifier(10f);
        unit.SetTerrainModifier(terrainMod);
    }
}
