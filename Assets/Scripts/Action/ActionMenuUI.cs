using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Owns the actual Move/Attack/Wait/confirm buttons. ActionMenuController stays the
// single entry point everything else already talks to (Show/Hide/
// SelectMove/SelectAttack/SelectWait) - this class only knows about UI:
// button interactable state and keyboard/controller focus. Nothing outside
// ActionMenuController should reference this directly.
public class ActionMenuUI : MonoBehaviour
{
    [Tooltip("The panel that gets shown/hidden as a whole. Defaults to this GameObject if left empty.")]
    [SerializeField] private GameObject panelRoot;
      [SerializeField] public GameObject cogPanelRoot;
   
    [SerializeField] private Button moveButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button waitButton;

    private Unit currentUnit;


    private void Awake()
    {
        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }
         if (cogPanelRoot == null)
        {
            cogPanelRoot = gameObject;
        }

        panelRoot.SetActive(false);
        cogPanelRoot.SetActive(false);

        // Buttons call back into ActionMenuController rather than
        // UnitActionController directly, so ActionMenuController remains
        // the one place other systems integrate with.
        moveButton.onClick.AddListener(() => ActionMenuController.Instance.SelectMove());
        attackButton.onClick.AddListener(() => ActionMenuController.Instance.SelectAttack());
        waitButton.onClick.AddListener(() => ActionMenuController.Instance.SelectWait());
     
    }


    private void OnDestroy()
    {
        moveButton.onClick.RemoveAllListeners();
        attackButton.onClick.RemoveAllListeners();
        waitButton.onClick.RemoveAllListeners();
    }


    public void Show(Unit unit)
    {
        Debug.Log("ActionMenuUI -> Show");
        currentUnit = unit;

        panelRoot.SetActive(true);
        cogPanelRoot.SetActive(true);

        RefreshAvailability();
        FocusFirstAvailableButton();
    }


    public void Hide()
    {
        currentUnit = null;

        panelRoot.SetActive(false);
        cogPanelRoot.SetActive(false);

        ClearFocusIfOwnedByThisMenu();
    }


    // Re-evaluates which buttons should be interactable. Called every time
    // the menu opens - including when ActionMenuController.Show() is
    // called again mid-turn (e.g. after a move is confirmed but the unit
    // can still attack) - so button state never goes stale.
    private void RefreshAvailability()
    {
        if (currentUnit == null)
            return;

        moveButton.interactable = currentUnit.CanMove;
        attackButton.interactable = currentUnit.CanAct;

        // Waiting/ending the turn is always available while the menu is
        // open at all.
        waitButton.interactable = true;
    }


    // Keyboard/controller Navigate input needs something already selected
    // to move focus from. Picks the first interactable button so a
    // disabled Move (already moved this turn) doesn't eat initial focus.
    private void FocusFirstAvailableButton()
    {
        if (EventSystem.current == null)
            return;

        Button target =
            moveButton.interactable ? moveButton :
            attackButton.interactable ? attackButton :
            waitButton;

        EventSystem.current.SetSelectedGameObject(target.gameObject);
    }


    private void ClearFocusIfOwnedByThisMenu()
    {
        if (EventSystem.current == null)
            return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected != null && selected.transform.IsChildOf(transform))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
