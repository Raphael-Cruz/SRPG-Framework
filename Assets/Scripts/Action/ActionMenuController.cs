using UnityEngine;

public class ActionMenuController : MonoBehaviour
{
    public static ActionMenuController Instance { get; private set; }


    [SerializeField] private ActionMenuUI menuUI;

    private Unit currentUnit;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }



    public void Show(Unit unit)
    {
        Debug.Log("ActionMenuController -> Show");
        currentUnit = unit;


        Debug.Log(
            $"Action menu opened for {unit.Data.UnitName}"
        );


        menuUI?.Show(unit);
    }



    public void Hide()
    {
        currentUnit = null;


        Debug.Log("Action menu closed");


        menuUI?.Hide();
    }



    public void SelectMove()
    {
        if(currentUnit == null)
            return;


        UnitActionController.Instance.StartMove();


        
    }



    public void SelectAttack()
    {
        if(currentUnit == null)
            return;


        UnitActionController.Instance.StartAttack();


        
    }



    public void SelectWait()
    {
        if(currentUnit == null)
            return;


        UnitActionController.Instance.FinishTurn();


       
    }
}
