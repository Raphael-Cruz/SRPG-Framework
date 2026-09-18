using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private UnitData data;

    [Header("Runtime State")]
    [SerializeField] private int currentHP;
    [SerializeField] private int currentSP;

    private UnitVisual visual;
    public UnitVisual Visual => visual;

    private GridTile currentTile;
    private GridTile previewTile;

    private bool hasMoved;
    private bool hasActed;

    [Header("Team")]
    [SerializeField] private UnitTeam team;
    public UnitTeam Team => team;

    // --- TERRENO & MODIFICADORES ---
    public List<CombatModifier> ActiveModifiers = new List<CombatModifier>();
    private CombatModifier currentTerrainModifier;
    // -------------------------------

    public UnitData Data => data;
    public int CurrentHP => currentHP;
    public int CurrentSP => currentSP;
    public GridTile CurrentTile => currentTile;
    public GridTile EffectiveTile => previewTile != null ? previewTile : currentTile;

    public bool HasMoved => hasMoved;
    public bool HasActed => hasActed;
    public bool CanMove => !hasMoved;
    public bool CanAct => !hasActed;
    public bool IsAlive => currentHP > 0;
    public bool TurnFinished => hasMoved && hasActed;
    public bool CanBeSelected => !TurnFinished;
    public bool IsPlayerControlled => team == UnitTeam.Player;
    public bool IsMainCharacter => data != null && data.IsMainCharacter;

    public event Action<Unit> OnTurnStateChanged;
    public event Action<Unit> OnMoved;
    public event Action<Unit> OnDied;

    private void Awake()
    {
        if (data != null)
        {
            currentHP = data.MaxHP;
            currentSP = data.MaxSP;
        }
        
        visual = GetComponent<UnitVisual>();

        InitiativeOrderSystem.Instance?.Register(this);
    }

    public void Initialize(UnitData newData)
    {
        data = newData;
        if (data != null)
        {
            currentHP = data.MaxHP;
            currentSP = data.MaxSP;
        }
    }

    private void OnDestroy()
    {
        InitiativeOrderSystem.Instance?.Unregister(this);
    }

    public void SetCurrentTile(GridTile tile)
    {
        currentTile = tile;
    }

    public void SetTile(GridTile tile)
    {
        currentTile = tile;
        tile.SetOccupant(this);
    }

    // --- GERENCIAMENTO DO TERRENO ---
    public void SetTerrainModifier(CombatModifier newTerrainMod)
    {
        // Remove o modificador do terreno anterior
        if (currentTerrainModifier != null)
        {
            ActiveModifiers.Remove(currentTerrainModifier);
        }

        currentTerrainModifier = newTerrainMod;

        // Adiciona o novo modificador (se não for um terreno nulo)
        if (currentTerrainModifier != null)
        {
            ActiveModifiers.Add(currentTerrainModifier);
        }
    }
    // --------------------------------

    // Called when this unit's turn begins
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

    // Ends turn manually
    public void FinishTurn()
    {
        hasMoved = true;
        hasActed = true;

        RefreshVisualState();
        OnTurnStateChanged?.Invoke(this);
    }

    private void RefreshVisualState()
    {
        visual?.SetExhausted(TurnFinished);
    }

    public void SetActiveTurn(bool value)
    {
        visual?.SetActiveTurn(value);
    }

    public void Select()
    {
        visual?.Select();
    }

    public void Deselect()
    {
        visual?.Deselect();
    }

    public void MoveTo(GridTile targetTile)
    {
        if (!CanMove) return;
        if (targetTile.Occupant != null && targetTile.Occupant != this) return;

        if (currentTile != null) currentTile.ClearOccupant();
        transform.position = targetTile.WorldPosition;
        currentTile = targetTile;
        targetTile.SetOccupant(this);
        MoveUsed();
        OnMoved?.Invoke(this);
    }

    /// <summary>
    /// Called by UnitMovementController after the walk animation finishes.
    /// The unit's transform is ALREADY at the destination — this only updates
    /// the logical tile state (occupancy, hasMoved flag, event).
    /// </summary>
    public void CommitMove(GridTile destination)
    {
        if (currentTile != null)
            currentTile.ClearOccupant();

        currentTile = destination;
        destination.SetOccupant(this);
        ClearPreviewTile();

        MoveUsed();

        Debug.Log($"{name} committed move to ({destination.X},{destination.Y})");
        OnMoved?.Invoke(this);
    }

    public void MoveAlongPath(List<GridTile> path, Action onCompleted)
    {
        if (!CanMove)
        {
            Debug.Log($"{name} already moved this turn.");
            return;
        }

        GridTile targetTile = path[path.Count - 1];

        if (targetTile.Occupant != null && targetTile.Occupant != this)
        {
            Debug.Log("Tile occupied.");
            return;
        }

        if (currentTile != null)
        {
            currentTile.ClearOccupant();
        }

        targetTile.SetOccupant(this);
        MoveUsed();

        StartCoroutine(AnimateMovementRoutine(path, targetTile, onCompleted));
    }

    [Header("Movement (AI/Pathing)")]
    [Tooltip("Tiles per second when this unit moves automatically.")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 10f;

    private IEnumerator AnimateMovementRoutine(List<GridTile> path, GridTile finalTile, Action onCompleted)
    {
        visual?.SetWalking(true);

        foreach (var tile in path)
        {
            Vector3 targetPos = tile.WorldPosition;
            
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                
                Vector3 moveDir = (targetPos - transform.position).normalized;
                if (moveDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
                }

                yield return null;
            }
            transform.position = targetPos;
        }

        visual?.SetWalking(false);
        currentTile = finalTile;

        Debug.Log($"{name} moved to ({finalTile.X},{finalTile.Y})");
        OnMoved?.Invoke(this);
        
        onCompleted?.Invoke();
    }

    public void SetPreviewTile(GridTile tile)
    {
        previewTile = tile;
    }

    public void ClearPreviewTile()
    {
        previewTile = null;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log($"{name} HP: {currentHP}/{data.MaxHP}");

        if(currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} died");

        if (currentTile != null)
        {
            currentTile.ClearOccupant();
        }

        // Fire the event immediately so AI and turn managers know it's dead
        OnDied?.Invoke(this);
        
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // Play death animation
        visual?.TriggerDeath();

        // Wait for animation to finish + 1 second (adjust time as needed for your specific animation length)
        yield return new WaitForSeconds(2.5f);

        BattleManager.Instance?.CheckBattleEnd();
        
        // Use Destroy instead of SetActive(false) to fully remove the unit
        Destroy(gameObject);
    }
}

public enum UnitTeam
{
    Player,
    Enemy
}