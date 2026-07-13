using UnityEngine;
using System;

public class SelectionManager : MonoBehaviour
{
    private Unit selectedUnit;


    public Unit SelectedUnit => selectedUnit;
    public event Action<Unit> OnUnitSelected;
    public event Action OnUnitDeselected;
    [SerializeField] private SelectionIndicator indicator;


    public void SelectUnit(Unit unit)
    {
        if (unit == null)
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
        selectedUnit = null;
        indicator.Hide();
        OnUnitDeselected?.Invoke();

        selectedUnit = null;
    }
}