using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatPreviewUI : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject previewPanel;
    [SerializeField] private GameObject grayPanel;
    [SerializeField] private GameObject portraitPanel;
    [SerializeField] private GameObject bottomPanel;

    [Header("Front Page")]
    [SerializeField] private GameObject leftFrontPanel;
    [SerializeField] private GameObject rightFrontPanel;

    

    [Header("Back Page")]
    [SerializeField] private GameObject modifierPage;
       
    [SerializeField] private GameObject leftBackPanel;
    [SerializeField] private GameObject rightBackPanel;

    [Header("Portraits")]
    [SerializeField] private Image attackerPortrait;
    [SerializeField] private Image targetPortrait;

    [Header("Texts")]
    [SerializeField] private TMP_Text attackerNameText;
     [SerializeField] private TMP_Text attackerNameTextbackPanel;
    [SerializeField] private TMP_Text targetNameText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text hitChanceText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text avoidChanceText;

    [Header("HP Gauges")]
    [SerializeField] private HPGaugeView attackerGauge;
    [SerializeField] private HPGaugeView defenderGauge;

    [Header("Modifiers")]
    [SerializeField] private ModifierListView attackerModifierList;

    private void Awake()
    {
        Hide();
    }

  public void Show(Unit attacker, Unit target, CombatPrediction prediction)
{
    Debug.Log("CombatPreviewUI.Show()");

    if (attacker == null)
    {
        Debug.Log("Attacker is NULL");
        Hide();
        return;
    }

    if (target == null)
    {
        Debug.Log("Target is NULL");
        Hide();
        return;
    }

    if (prediction == null)
    {
        Debug.Log("Prediction is NULL");
        Hide();
        return;
    }

    Debug.Log($"Modifiers: {prediction.Modifiers.Count}");

        previewPanel.SetActive(true);
        grayPanel.SetActive(true);
        portraitPanel.SetActive(true);
        bottomPanel.SetActive(true);

        ResetPages();

        // Portraits
        attackerPortrait.sprite = attacker.Data.Portrait;
        attackerPortrait.enabled = attacker.Data.Portrait != null;

        targetPortrait.sprite = target.Data.Portrait;
        targetPortrait.enabled = target.Data.Portrait != null;

        // Names
        attackerNameText.text = attacker.Data.UnitName;
         attackerNameTextbackPanel.text = attacker.Data.UnitName;
        targetNameText.text = target.Data.UnitName;

        // Stats
        damageText.text = prediction.Damage.ToString();
        hitChanceText.text = $"{prediction.HitChance:0}%";
        defenseText.text = prediction.Defense.ToString();
        avoidChanceText.text = $"{prediction.Avoid:0}%";

        // HP
        attackerGauge.SetGauge(prediction.AttackerGauge);
        defenderGauge.SetGauge(prediction.DefenderGauge);

        // Modifiers
        attackerModifierList.Show(prediction.Modifiers);

        leftBackPanel.SetActive(false);
          rightBackPanel.SetActive(false);
    }

    public void Hide()
    {
        previewPanel.SetActive(false);
        grayPanel.SetActive(false);
        portraitPanel.SetActive(false);
        bottomPanel.SetActive(false);

        leftFrontPanel.SetActive(false);
         rightFrontPanel.SetActive(false);
          leftBackPanel.SetActive(false);
           rightBackPanel.SetActive(false);

        ResetPages();

        attackerPortrait.sprite = null;
        attackerPortrait.enabled = false;

        targetPortrait.sprite = null;
        targetPortrait.enabled = false;

        attackerNameText.text = "";
        
        attackerNameTextbackPanel.text = "";
        targetNameText.text = "";
        damageText.text = "";
        defenseText.text = "";
        hitChanceText.text = "";
        avoidChanceText.text = "";

        attackerModifierList.Clear();
    }

    public void ResetPages()
    {
        leftFrontPanel.SetActive(true);
        rightFrontPanel.SetActive(true);

        if (modifierPage != null)
            modifierPage.SetActive(false);
    }

    public void HideOnFlipLeft()
    {
        leftFrontPanel.SetActive(false);
        Debug.Log("hide on");
    }

    public void HideOnFlipRight()
    {
        rightFrontPanel.SetActive(false);
    }

    public void ShowModifierPage()
    {
        if (modifierPage != null)
            modifierPage.SetActive(true);

            rightBackPanel.SetActive(true);
            leftBackPanel.SetActive(true);
         

            
    }

    public void HideModifierPage()
    {
        if (modifierPage != null)
            modifierPage.SetActive(false);
    }

    public void ShowFrontLeft()
    {
        leftFrontPanel.SetActive(true);
      
    }

    public void ShowFrontRight()
    {
        rightFrontPanel.SetActive(true);
      
    }
    public void ShowFrontPage()
{
    leftFrontPanel.SetActive(true);
    rightBackPanel.SetActive(true);
     rightBackPanel.SetActive(false);
      leftBackPanel.SetActive(false);
}
}