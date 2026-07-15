using UnityEngine;

public class UnitActionController : MonoBehaviour
{
    public static UnitActionController Instance { get; private set; }


    public UnitActionState State { get; private set; }
        = UnitActionState.None;


    private Unit selectedUnit;
public Unit SelectedUnit => selectedUnit;

[SerializeField]
private UnitMovementController movementController;


[SerializeField]
private UnitAttackController attackController;
[SerializeField]
private ActionMenuController actionMenu;



    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

private void Start()
{
    movementController.OnMovementConfirmed += HandleMovementConfirmed;
    attackController.OnAttackConfirmed += HandleAttackConfirmed;
}

private void Update()
{
    if(selectedUnit == null)
        return;


    if(Input.GetKeyDown(KeyCode.Escape))
    {
        CancelAction();
    }
}

public void BeginUnitTurn(Unit unit)
{
    selectedUnit = unit;

    State = UnitActionState.SelectingAction;


    Debug.Log(
        $"Action selection started for {unit.name}"
    );


    actionMenu.Show(unit);
}



public void StartMove()
{
    if(selectedUnit == null)
        return;


    if(!selectedUnit.CanMove)
    {
        Debug.Log("Unit already moved.");
        return;
    }


    State = UnitActionState.Moving;


    movementController.BeginMovement(selectedUnit);


    Debug.Log(
        $"{selectedUnit.name} is choosing movement"
    );
}



  public void CancelAction()
{
    if(movementController != null)
    {
        movementController.CancelMove();
            
    }

    if(attackController != null)
    {
        attackController.CancelAttack();
    }

    State = UnitActionState.SelectingAction;

    Debug.Log("Action cancelled");
}



   public void FinishTurn()
{
    if (selectedUnit == null)
        return;

    State = UnitActionState.Finished;

    TurnManager.Instance.EndTurn(selectedUnit);
}

private void OnDestroy()
{
    if(movementController != null)
    {
        movementController.OnMovementConfirmed -= HandleMovementConfirmed;
    }

    if(attackController != null)
    {
        attackController.OnAttackConfirmed -= HandleAttackConfirmed;
    }
}


public void StartAttack()
{
    if(selectedUnit == null)
        return;


    if(movementController.State == MovementState.Previewing)
    {
        Debug.Log("Confirm movement first.");
        return;
    }


    if(!selectedUnit.CanAct)
    {
        Debug.Log("Unit already acted.");
        return;
    }


    State = UnitActionState.SelectingAttackTarget;


    attackController.BeginAttack(selectedUnit);
}


public void TryAttack(GridTile tile)
{
    attackController.TryAttack(tile);
}
public void TryMove(GridTile tile)
{
    movementController.HandleTileClick(tile);
}


private void HandleMovementConfirmed(Unit unit)
{
    State = UnitActionState.SelectingAction;


    ActionMenuController.Instance.Show(unit);


    Debug.Log(
        $"{unit.name} finished moving. Choose next action."
    );
}

private void HandleAttackConfirmed(Unit unit)
{
    CheckRemainingActions(unit);
}

private void CheckRemainingActions(Unit unit)
{
    if(unit.CanAct)
    {
        State = UnitActionState.SelectingAction;

        ActionMenuController.Instance.Show(unit);

        Debug.Log(
            $"{unit.name} still has actions."
        );
    }
    else
    {
        FinishTurn();
    }
}

}