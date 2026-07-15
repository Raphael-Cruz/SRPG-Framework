using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
      public static SelectionManager Instance { get; private set; }
    private Unit selectedUnit;


    public Unit SelectedUnit => selectedUnit;


    public event Action<Unit> OnUnitSelected;
    public event Action OnUnitDeselected;


    [SerializeField] private SelectionIndicator indicator;
    [SerializeField] private MovementRangeCalculator movementRange;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void SelectUnit(Unit unit)
    {
  
        if (unit == null)
            return;



        // Only allow selection during active battle
        if (BattleManager.Instance != null &&
            BattleManager.Instance.State != BattleState.Fighting)
        {
            return;
        }



        // Unit has finished all actions
        if (!unit.CanBeSelected)
        {
            Debug.Log($"{unit.name} has no actions left.");
            return;
        }



        // Only current turn unit can be controlled
        if (TurnManager.Instance != null &&
            unit != TurnManager.Instance.CurrentUnit)
        {
            return;
        }



     // Clicking the selected unit again leaves movement selection
    // and returns to action choices.
    if (selectedUnit == unit)
    {
        UnitActionController.Instance.OpenActionSelection();
        return;
    }



        // Remove previous selection
        if (selectedUnit != null)
        {
            DeselectCurrentUnit();
        }



        selectedUnit = unit;



        selectedUnit.Select();



        indicator.Show(
            selectedUnit.transform.position
        );



        RefreshSelectionOptions();

        //select the unit starts the unit movement.
        UnitActionController.Instance.BeginUnitTurn(selectedUnit);
        UnitActionController.Instance.StartMove();



        OnUnitSelected?.Invoke(selectedUnit);



        Debug.Log(
            $"Selected Unit: {selectedUnit.name}"
        );
    }




    private void RefreshSelectionOptions()
    {
        if (selectedUnit == null)
            return;



        movementRange.ClearMovementRange();



        // Only show movement if the unit still has movement available
        if (selectedUnit.CanMove)
        {
            movementRange.ShowMovementRange(selectedUnit);
        }
    }





    public void DeselectCurrentUnit()
    {
        if (selectedUnit == null)
            return;



        Debug.Log(
            $"Deselected Unit: {selectedUnit.name}"
        );



        selectedUnit.Deselect();



        movementRange.ClearMovementRange();



        indicator.Hide();



        OnUnitDeselected?.Invoke();



        selectedUnit = null;
    }
}