using UnityEngine;
using System.Collections;

public class CombatPreviewPageController : MonoBehaviour
{
    private static readonly int FlipForward =
        Animator.StringToHash("FlipForward");

    private static readonly int FlipBackward =
        Animator.StringToHash("FlipBackward");

    [SerializeField] private CombatPreviewUI combatPreviewUI;
    [SerializeField] private Animator animator;

    [Header("Forward Delays")]
    [SerializeField] private float flipForwardDelayRightPanel = 0.25f;
    [SerializeField] private float flipForwardDelayLeftPanel = 0.45f;
    [SerializeField] private float flipForwardDelayModifierPage = 0.45f;

    [Header("Backward Delays")]
    [SerializeField] private float flipBackwardDelayModifierPage = 0.25f;
    [SerializeField] private float flipBackwardDelayRightPanel = 0.45f;
    [SerializeField] private float flipBackwardDelayLeftPanel = 0.45f;

    [Header("Animator States")]
    [SerializeField] private string idleFrontState = "IdleFront";
    [SerializeField] private string idleBackState = "IdleBack";
    [SerializeField] private string flipForwardState = "FlipForward";
    [SerializeField] private string flipBackwardState = "FlipBackward";
    

    public bool IsTransitioning
    {
        get
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            return state.IsName(flipForwardState) ||
                   state.IsName(flipBackwardState);
        }
    }

    public bool ShowingBackPage
    {
        get
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            return state.IsName(idleBackState);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TogglePage();
        }
    }
#endif

    public void TogglePage()
    {
        if (IsTransitioning)
            return;

        if (ShowingBackPage)
            StartCoroutine(FlipBackwardRoutine());
        else
            StartCoroutine(FlipForwardRoutine());
    }

    private IEnumerator FlipForwardRoutine()
    {
        animator.SetTrigger(FlipForward);

        StartCoroutine(HideRightRoutine());
        StartCoroutine(HideLeftRoutine());
        StartCoroutine(ShowModifierRoutine());

        yield break;
    }

    private IEnumerator FlipBackwardRoutine()
    {
        animator.SetTrigger(FlipBackward);

        StartCoroutine(HideModifierRoutine());
        StartCoroutine(ShowRightRoutine());
        StartCoroutine(ShowLeftRoutine());

        yield break;
    }

    private IEnumerator HideRightRoutine()
    {
        yield return new WaitForSeconds(flipForwardDelayRightPanel);
        combatPreviewUI.HideOnFlipRight();
    }

    private IEnumerator HideLeftRoutine()
    {
        yield return new WaitForSeconds(flipForwardDelayLeftPanel);
        combatPreviewUI.HideOnFlipLeft();
    }

    private IEnumerator ShowModifierRoutine()
    {
        yield return new WaitForSeconds(flipForwardDelayModifierPage);
        combatPreviewUI.ShowModifierPage();
    }

    private IEnumerator HideModifierRoutine()
    {
        yield return new WaitForSeconds(flipBackwardDelayModifierPage);
        combatPreviewUI.HideModifierPage();
    }

    private IEnumerator ShowRightRoutine()
    {
        yield return new WaitForSeconds(flipBackwardDelayRightPanel);
        combatPreviewUI.ShowFrontRight();
    }

    private IEnumerator ShowLeftRoutine()
    {
        yield return new WaitForSeconds(flipBackwardDelayLeftPanel);
        combatPreviewUI.ShowFrontLeft();
    }

    public void ResetToFront()
    {
        if (!ShowingBackPage)
            return;

        TogglePage();
    }
}