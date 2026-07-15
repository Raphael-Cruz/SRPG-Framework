using UnityEngine;

public class ActionMenuTester : MonoBehaviour
{
    private void Update()
    {
        if(ActionMenuController.Instance == null)
            return;


        if(Input.GetKeyDown(KeyCode.M))
        {
            ActionMenuController.Instance.SelectMove();
        }


        if(Input.GetKeyDown(KeyCode.N))
        {
            ActionMenuController.Instance.SelectAttack();
        }


        if(Input.GetKeyDown(KeyCode.B))
        {
            ActionMenuController.Instance.SelectWait();
        }
    }
}