
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatPresentationController : MonoBehaviour
{
    [Header("Setups")]
    [SerializeField] private GameObject tacticalSetup;
    [SerializeField] private GameObject combatSetup;

    [Header("Cameras")]
    [SerializeField] private Camera tacticalCamera;
    [SerializeField] private Camera combatCamera;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.25f;

    private bool isInCombatView;

    [Header("Combat Stage Points")]
    [SerializeField] private Transform attackerSpawn;
    [SerializeField] private Transform defenderSpawn;

    // Real data references for your future UI
    public Unit ActiveAttacker { get; private set; }
    public Unit ActiveDefender { get; private set; }

    // Cloned visual references to play animations
    private UnitVisual clonedAttackerVisual;
    private UnitVisual clonedDefenderVisual;

    private void Awake()
    {
        Debug.Log("[CombatPresentationController] Awake -> Iniciando SetTacticalViewImmediate");
        SetTacticalViewImmediate();
    }

    public IEnumerator EnterCombatViewRoutine(Unit attacker, Unit defender)
    {
        Debug.Log("[CombatPresentationController] EnterCombatViewRoutine chamado!");
        if (isInCombatView)
            yield break;

        ActiveAttacker = attacker;
        ActiveDefender = defender;

        isInCombatView = true;

        // 1. Fade to black
        Debug.Log("[CombatPresentationController] Iniciando Fade Out (para preto)...");
        yield return Fade(1f);
        Debug.Log("[CombatPresentationController] Fade Out concluído!");

        // 2. Setup the Clones in the dark
        Debug.Log("[CombatPresentationController] Spawnando Clones...");
        try { SpawnClones(); } catch (System.Exception e) { Debug.LogError("Erro no SpawnClones: " + e); }

        // 3. Switch setups and cameras
        Debug.Log("[CombatPresentationController] Trocando Setups e Câmeras...");
        if (tacticalSetup != null) tacticalSetup.SetActive(false);
        if (combatSetup != null) combatSetup.SetActive(true);
        if (tacticalCamera != null) tacticalCamera.gameObject.SetActive(false);
        if (combatCamera != null) combatCamera.gameObject.SetActive(true);

        // 4. Fade back in
        Debug.Log("[CombatPresentationController] Iniciando Fade In (voltando para transparente)...");
        yield return Fade(0f);
        Debug.Log("[CombatPresentationController] Fade In concluído! Palco revelado.");
    }

    public IEnumerator ExitCombatViewRoutine()
    {
        Debug.Log("[CombatPresentationController] ExitCombatViewRoutine chamado!");
        if (!isInCombatView)
            yield break;

        // 1. Fade to black
        yield return Fade(1f);

        // 2. Switch setups and cameras back
        if (combatSetup != null) combatSetup.SetActive(false);
        if (tacticalSetup != null) tacticalSetup.SetActive(true);
        if (combatCamera != null) combatCamera.gameObject.SetActive(false);
        if (tacticalCamera != null) tacticalCamera.gameObject.SetActive(true);

        // 3. Destroy clones and clear data
        DestroyClones();
        ActiveAttacker = null;
        ActiveDefender = null;

        // 4. Fade back in
        yield return Fade(0f);

        isInCombatView = false;
    }

    // --- Helpers to trigger animations from CombatSystem ---
    public void TriggerAttackerAnimation()
    {
        if (clonedAttackerVisual != null)
            clonedAttackerVisual.TriggerAttack();
    }

    public void TriggerDefenderDeathAnimation()
    {
        if (clonedDefenderVisual != null)
            clonedDefenderVisual.TriggerDeath();
    }

    private void SpawnClones()
    {
        if (ActiveAttacker != null && ActiveAttacker.Visual != null && attackerSpawn != null)
        {
            clonedAttackerVisual = Instantiate(ActiveAttacker.Visual, attackerSpawn.position, attackerSpawn.rotation, combatSetup.transform);
        }

        if (ActiveDefender != null && ActiveDefender.Visual != null && defenderSpawn != null)
        {
            clonedDefenderVisual = Instantiate(ActiveDefender.Visual, defenderSpawn.position, defenderSpawn.rotation, combatSetup.transform);
        }
    }

    private void DestroyClones()
    {
        if (clonedAttackerVisual != null) Destroy(clonedAttackerVisual.gameObject);
        if (clonedDefenderVisual != null) Destroy(clonedDefenderVisual.gameObject);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null)
            yield break;

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / fadeDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }

    private void SetTacticalViewImmediate()
    {
        Debug.Log("[CombatPresentationController] SetTacticalViewImmediate: Ligando Tático, desligando Combat, Fade alpha 0");
        isInCombatView = false;

        if (tacticalSetup != null)
            tacticalSetup.SetActive(true);

        if (combatSetup != null)
            combatSetup.SetActive(false);

        if (tacticalCamera != null)
            tacticalCamera.gameObject.SetActive(true);

        if (combatCamera != null)
            combatCamera.gameObject.SetActive(false);

        if (fadeImage != null)
        {
            fadeImage.raycastTarget = false; // EVITA QUE A IMAGEM INVISÍVEL ROUBE OS CLIQUES DO MOUSE!
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }
}
