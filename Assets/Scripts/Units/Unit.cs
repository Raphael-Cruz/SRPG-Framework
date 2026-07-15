using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private UnitData data;


    [Header("Runtime State")]
    [SerializeField] private int currentHP;

    private UnitVisual visual;

    public UnitVisual Visual => visual;


    private GridTile currentTile;


    private bool hasMoved;
    private bool hasActed;


    [Header("Team")]
    [SerializeField]
    private UnitTeam team;

    public UnitTeam Team => team;


    public UnitData Data => data;

    public int CurrentHP => currentHP;

    public GridTile CurrentTile => currentTile;


    public bool HasMoved => hasMoved;

    public bool HasActed => hasActed;


    public bool CanMove => !hasMoved;

    public bool CanAct => !hasActed;


    public bool IsAlive => currentHP > 0;


    public bool TurnFinished => hasMoved && hasActed;


    public bool CanBeSelected => !TurnFinished;


    public bool IsPlayerControlled =>
        team == UnitTeam.Player;



    public event Action<Unit> OnTurnStateChanged;


private GridTile previewTile;

public GridTile EffectiveTile =>
    previewTile != null ? previewTile : currentTile;

    private void Awake()
    {
        currentHP = data.MaxHP;

        visual = GetComponent<UnitVisual>();

        InitiativeOrderSystem.Instance?.Register(this);
    }

public void SetCurrentTile(GridTile tile)
{
    currentTile = tile;
}

    private void OnDestroy()
    {
        InitiativeOrderSystem.Instance?.Unregister(this);
    }



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




    public void SetTile(GridTile tile)
    {
        currentTile = tile;

        tile.SetOccupant(this);
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
        if (!CanMove)
        {
            Debug.Log($"{name} already moved this turn.");
            return;
        }


        if (targetTile.Occupant != null)
        {
            Debug.Log("Tile occupied.");
            return;
        }



        if (currentTile != null)
        {
            currentTile.ClearOccupant();
        }



        transform.position = targetTile.WorldPosition;


        currentTile = targetTile;

        targetTile.SetOccupant(this);



        MoveUsed();



        Debug.Log(
            $"{name} moved to ({targetTile.X},{targetTile.Y})"
        );
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



        Debug.Log(
            $"{name} HP: {currentHP}/{data.MaxHP}"
        );



        if(currentHP <= 0)
        {
            Die();
        }
    }




    private void Die()
    {
        Debug.Log($"{name} died");


        if(currentTile != null)
        {
            currentTile.ClearOccupant();
        }


        gameObject.SetActive(false);


        BattleManager.Instance?.CheckBattleEnd();
    }
}



public enum UnitTeam
{
    Player,
    Enemy
}