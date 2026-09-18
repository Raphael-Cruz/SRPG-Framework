using UnityEngine;

public enum GameCameraMode
{
    Exploration,
    Battle
}

// Owns nothing about movement, combat, or camera behaviour itself - just
// switches which camera rig (and its input-driven scripts) is active.
// Listens to BattleManager.OnBattleStateChanged to enter Battle mode
// automatically, but does NOT auto-return to Exploration on Victory/Defeat:
// per BattleManager's own contract, what happens after those states
// (reward screens, returning to the dungeon) is owned by an external flow
// system, not BattleManager or this controller. That system calls
// ReturnToExploration() once it's actually done.
public class CameraModeController : MonoBehaviour
{
    public static CameraModeController Instance { get; private set; }

    [SerializeField] private GameObject explorationRig;   // player + ThirdPersonOrbitCamBasic
    [SerializeField] private GameObject battleCameraRig;  // CameraController's camera
    [SerializeField] private BattleManager battleManager;

    private GameCameraMode currentMode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (battleManager != null)
            battleManager.OnBattleStateChanged += HandleBattleStateChanged;

        SetMode(GameCameraMode.Exploration);
    }

    private void OnDisable()
    {
        if (battleManager != null)
            battleManager.OnBattleStateChanged -= HandleBattleStateChanged;
    }

    private void HandleBattleStateChanged(BattleState state)
    {
        // Preparing/Fighting put us in battle view. Victory/Defeat stay on
        // the battle camera - the post-battle flow system calls
        // ReturnToExploration() once it's actually finished.
        if (state == BattleState.Preparing || state == BattleState.Fighting)
        {
            SetMode(GameCameraMode.Battle);
        }
    }

    // Called by the post-battle flow system (reward screen closed,
    // returning to the dungeon, etc.) - not by BattleManager itself.
    public void ReturnToExploration()
    {
        SetMode(GameCameraMode.Exploration);
    }

    private void SetMode(GameCameraMode mode)
    {
        currentMode = mode;

        bool exploring = mode == GameCameraMode.Exploration;
        explorationRig.SetActive(exploring);
        battleCameraRig.SetActive(!exploring);
    }
}