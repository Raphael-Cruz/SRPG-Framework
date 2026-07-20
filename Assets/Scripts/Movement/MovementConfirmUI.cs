using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Shows a "Confirm" button whenever UnitMovementController has an active
// movement preview, and hides it the moment that preview ends - whether
// by confirming, cancelling, or Enter/gamepad Confirm (which already
// calls ConfirmMove() directly via InputManager - see
// UnitMovementController.HandleConfirmPressed). This button is just an
// additional, mouse-friendly way to trigger the same ConfirmMove() call;
// it doesn't duplicate or compete with that path.
public class MovementConfirmUI : MonoBehaviour
{
    [Tooltip("The panel that gets shown/hidden as a whole. Defaults to this GameObject if left empty.")]
    [SerializeField] private GameObject panelRoot;

    [SerializeField] private Button confirmButton;
    [SerializeField] private UnitMovementController movementController;
      [SerializeField] private ActionMenuUI actionMenuUI;
      [SerializeField] private GameObject ActionMenuSideBar;


private void Awake()
{
    if(panelRoot == null)
        panelRoot = gameObject;

    panelRoot.SetActive(false);

    confirmButton.onClick.AddListener(HandleConfirmClicked);

  
   
    if (movementController == null)
    {
        Debug.LogError("MovementConfirmUI: MovementController not assigned!");
    }

    if (confirmButton == null)
    {
        Debug.LogError("MovementConfirmUI: Confirm Button not assigned!");
    }

if (actionMenuUI == null)
        {
            return;
        }
}


private void Start()
{
    
    if (movementController == null)
    {
        return;
    }

    movementController.OnPreviewStarted += HandleShow;
    movementController.OnPreviewEnded += HandleHide;
}


    private void OnDisable()
    {
     
        if (movementController != null)
        {
            movementController.OnPreviewStarted -= HandleShow;
            movementController.OnPreviewEnded -= HandleHide;
        }
    }


    private void OnDestroy()
    {
        confirmButton.onClick.RemoveAllListeners();
    }


    private void HandleShow(Unit unit)
    {
            Debug.Log("MovementConfirmUI SHOW");

        panelRoot.SetActive(true);

        actionMenuUI.cogPanelRoot.SetActive(true);
        

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        }
    }


    private void HandleHide()
    {
        panelRoot.SetActive(false);

        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == confirmButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }


    private void HandleConfirmClicked()
    {
        // ConfirmMove() already no-ops if State isn't Previewing, so
        // there's no risk of a stray click doing anything once the
        // preview has already ended.
        movementController.ConfirmMove();
    }
}
