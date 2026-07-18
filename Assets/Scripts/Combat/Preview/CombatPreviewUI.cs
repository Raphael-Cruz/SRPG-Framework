using TMPro;
using UnityEngine;

public class CombatPreviewUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject previewPanel;
        [SerializeField] private GameObject portrait;
        
        [SerializeField] private GameObject grayPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text attackerNameText;
    [SerializeField] private TMP_Text targetNameText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text hitChanceText;
      [SerializeField] private TMP_Text defenseText;
        [SerializeField] private TMP_Text avoidChanceText;

   [SerializeField] private GameObject leftPanel;
      [SerializeField] private GameObject rightPanel;

    private void Awake()
    {
        Hide();
    }

    public void Show(Unit attacker, Unit target, CombatPrediction prediction)
    {
        if (attacker == null ||
            target == null ||
            prediction == null)
        {
            Hide();
            return;
        }

        previewPanel.SetActive(true);
        portrait.SetActive(true);
        grayPanel.SetActive(true);

        leftPanel.SetActive(true);
        rightPanel.SetActive(true);

        attackerNameText.text = attacker.Data.UnitName;
        targetNameText.text = target.Data.UnitName;

        damageText.text = prediction.Damage.ToString();
        hitChanceText.text = $"{prediction.HitChance:0}%";

        defenseText.text = prediction.Defense.ToString();
        avoidChanceText.text = $"{prediction.Avoid:0}%";


    }

    public void Hide()
    {
        previewPanel.SetActive(false);
           portrait.SetActive(false);
            grayPanel.SetActive(false);

        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        attackerNameText.text = "";
        targetNameText.text = "";
        damageText.text = "";
        defenseText.text = "";
        hitChanceText.text = "";
        avoidChanceText.text = "";
    }
}