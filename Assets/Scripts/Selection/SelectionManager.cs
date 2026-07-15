using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Unit selectedUnit;


    public Unit SelectedUnit => selectedUnit;


    public event Action<Unit> OnUnitSelected;
    public event Action OnUnitDeselected;


    [SerializeField] private SelectionIndicator indicator;
    [SerializeField] private MovementRangeCalculator movementRange;



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



        // Clicking the same unit deselects it
        if (selectedUnit == unit)
        {
            DeselectCurrentUnit();
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



        OnUnitSelected?.Invoke(selectedUnit);
        UnitActionController.Instance?.BeginUnitTurn(selectedUnit);



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