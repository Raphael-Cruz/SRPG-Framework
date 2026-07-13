using UnityEngine;
using System;

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

        // Units that have already spent their turn (moved and/or acted)
        // can't be selected at all - no indicator, no movement range,
        // no deselect-by-reclicking. Clicking one simply does nothing here;
        // a small "already acted" feedback can be hooked in later.
        if (!unit.CanBeSelected)
            return;


        // If clicking the same unit 
      if (selectedUnit == unit)
{
    DeselectCurrentUnit();
    return;
}


        // Deselect previous unit
        if (selectedUnit != null)
        {
            DeselectCurrentUnit();
        }



        selectedUnit = unit;

        selectedUnit.Visual.Select();
        indicator.Show(
    selectedUnit.transform.position
);

        OnUnitSelected?.Invoke(selectedUnit);
        movementRange.ShowMovementRange(selectedUnit);

        Debug.Log(
            $"Selected Unit: {selectedUnit.name}"
);


   
    }


    public void DeselectCurrentUnit()
    {
        if (selectedUnit == null)
            return;


        Debug.Log(
            $"Deselected Unit: {selectedUnit.name}"
        );


        selectedUnit.Visual.Deselect();
        movementRange.ClearMovementRange();
        selectedUnit = null;
        
        indicator.Hide();
        OnUnitDeselected?.Invoke();

        selectedUnit = null;
    }
}