using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPreviewUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject previewPanel;
    [SerializeField] private GameObject grayPanel;
    [SerializeField] private GameObject portraitPanel;
    [Header("Portraits")]
    [SerializeField] private Image attackerPortrait;
    [SerializeField] private Image targetPortrait;

    [Header("Texts")]
    [SerializeField] private TMP_Text attackerNameText;
    [SerializeField] private TMP_Text targetNameText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text hitChanceText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text avoidChanceText;

    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    [SerializeField] private HPGaugeView attackerGauge;

[SerializeField] private HPGaugeView defenderGauge;


    private void Awake()
    {
        Hide();
    }

    public void Show(Unit attacker, Unit target, CombatPrediction prediction)
    {
 Debug.Log(
    $"COMBAT PREVIEW SHOW: {attacker.Data.UnitName} -> {target.Data.UnitName} " +
    $"Damage: {prediction.Damage} " +
    $"Target HP: {prediction.DefenderGauge.CurrentHP} " +
    $"Preview Damage: {prediction.DefenderGauge.PredictedDamage}"
);
        if (attacker == null ||
            target == null ||
            prediction == null)
        {
            Hide();
            return;
        }
        

        previewPanel.SetActive(true);
        grayPanel.SetActive(true);
        portraitPanel.SetActive(true);

        leftPanel.SetActive(true);
        rightPanel.SetActive(true);

        // Portraits
        attackerPortrait.sprite = attacker.Data.Portrait;
        attackerPortrait.enabled = attacker.Data.Portrait != null;

        targetPortrait.sprite = target.Data.Portrait;
        targetPortrait.enabled = target.Data.Portrait != null;

        // Names
        attackerNameText.text = attacker.Data.UnitName;
        targetNameText.text = target.Data.UnitName;

        // Stats
        damageText.text = prediction.Damage.ToString();
        hitChanceText.text = $"{prediction.HitChance:0}%";
        defenseText.text = prediction.Defense.ToString();
        avoidChanceText.text = $"{prediction.Avoid:0}%";

        //HP
        attackerGauge.SetGauge(prediction.AttackerGauge);
        defenderGauge.SetGauge(prediction.DefenderGauge);
        
    }

    public void Hide()
    {
        previewPanel.SetActive(false);
        grayPanel.SetActive(false);
        portraitPanel.SetActive(false);

        leftPanel.SetActive(false);
        rightPanel.SetActive(false);

        attackerPortrait.sprite = null;
        attackerPortrait.enabled = false;

        targetPortrait.sprite = null;
        targetPortrait.enabled = false;

        attackerNameText.text = "";
        targetNameText.text = "";
        damageText.text = "";
        defenseText.text = "";
        hitChanceText.text = "";
        avoidChanceText.text = "";
    }
}