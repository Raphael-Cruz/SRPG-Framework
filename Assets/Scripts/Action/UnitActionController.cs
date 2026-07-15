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

    InputManager.Instance.CancelPressed += HandleCancelPressed;
}

private void HandleCancelPressed()
{
    if(selectedUnit == null)
        return;

    CancelAction();
}

public void BeginUnitTurn(Unit unit)
{
    Debug.Log("UnitActionController -> BeginUnitTurn");

    selectedUnit = unit;

    State = UnitActionState.Selected;
    Debug.Log(
        $"Action selection started for {unit.name}"
    );

    
    // The menu now opens only after:
    //  clicking the selected unit again, or
    //  confirming movement.
}


public void OpenActionSelection()
{
    if(selectedUnit == null)
        return;


    movementController.CancelMove();


    State = UnitActionState.SelectingAction;


    actionMenu.Show(selectedUnit);


    Debug.Log(
        $"{selectedUnit.name} opened action menu."
    );
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


    // Hide the menu while the player is choosing a destination tile;
    // HandleMovementConfirmed() re-shows it once the move is committed.
    actionMenu.Hide();


    movementController.BeginMovement(selectedUnit);


    Debug.Log(
        $"{selectedUnit.name} is choosing movement"
    );
}


public void CancelAction()
{
    // Cancel attack targeting
    if(State == UnitActionState.SelectingAttackTarget)
    {
        bool cancelled = false;

        if(attackController != null)
        {
            cancelled |= attackController.CancelAttack();
        }


        if(!cancelled)
            return;


        State = UnitActionState.SelectingAction;


        if(actionMenu != null)
        {
            actionMenu.Show(selectedUnit);
        }


        Debug.Log("Attack cancelled. Returning to action menu.");

        return;
    }



    bool actionCancelled = false;


    if(movementController != null)
    {
        actionCancelled |= movementController.CancelMove();
    }


    if(attackController != null)
    {
        actionCancelled |= attackController.CancelAttack();
    }


    // Action menu cancellation
    if(State == UnitActionState.SelectingAction)
    {
        actionCancelled = true;
    }


    if(!actionCancelled)
        return;



    if(actionMenu != null)
    {
        actionMenu.Hide();
    }


    State = UnitActionState.Moving;


    movementController.ResumeMovement();


    Debug.Log("Action cancelled");
}

   public void FinishTurn()
{
    if (selectedUnit == null)
        return;

    State = UnitActionState.Finished;


    actionMenu.Hide();


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

    if(InputManager.Instance != null)
    {
        InputManager.Instance.CancelPressed -= HandleCancelPressed;
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


    // Hide the menu while the player is picking a target;
    // HandleAttackConfirmed() -> CheckRemainingActions() re-shows it if
    // the unit can still do something this turn.
    actionMenu.Hide();


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
    Debug.Log("UnitActionController -> HandleMovementConfirmed");


    if (!unit.CanMove && !unit.CanAct)
    {
        FinishTurn();
        return;
    }


    State = UnitActionState.SelectingAction;

    actionMenu.Show(unit);


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
    bool canMove = unit.CanMove;
    bool canAct = unit.CanAct;


    Debug.Log(
        $"Remaining options - Move: {canMove}, Act: {canAct}"
    );


    // Nothing left to do
    if (!canMove && !canAct)
    {
        FinishTurn();
        return;
    }


    // Something remains (movement or action)
    State = UnitActionState.SelectingAction;

    actionMenu.Show(unit);
}

}
