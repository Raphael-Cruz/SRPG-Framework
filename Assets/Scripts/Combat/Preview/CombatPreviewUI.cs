using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CombatantUIStats
{
    public Image Portrait;
    public TMP_Text NameText;
    public TMP_Text LevelText;
    
    [Header("Gauges")]
    public HPGaugeView HPGauge;
    public TMP_Text HPText;
    
    // We can reuse HPGaugeView for SP, or just a generic slider/bar
    // Assuming HPGaugeView works fine for now, or we can just update a text
    public TMP_Text SPText;
    // public HPGaugeView SPGauge; // Descomente caso tenha criado um SPGaugeView ou vá usar o HPGaugeView para SP

    [Header("Stats")]
    public TMP_Text DamageText;
    public TMP_Text HitOrAvoidText; // HIT% para atacante, AVO% para defensor
    public TMP_Text CritText;

    [Header("Modifiers")]
    public Transform ModifiersContainer; // Layout Group para os ícones
    public TMP_Text CritMultiplierText; // Exibe a tag multiplicadora de dano (ex: x1.25)
}

public class CombatPreviewUI : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject previewPanel;
    
    [Header("Combatants")]
    [SerializeField] private CombatantUIStats attackerStats;
    [SerializeField] private CombatantUIStats defenderStats;

    [Header("Prefabs")]
    [SerializeField] private GameObject modifierIconPrefab; // Prefab contendo apenas uma Image (Image componente)

    private void Awake()
    {
        Hide();
    }

    public void Show(Unit attacker, Unit target, CombatPrediction prediction)
    {
        if (attacker == null || target == null || prediction == null)
        {
            Hide();
            return;
        }

        previewPanel.SetActive(true);

        // Preenche Atacante
        FillCombatant(attackerStats, attacker, prediction.AttackerGauge, prediction.AttackerSPGauge, prediction.MinDamage, prediction.MaxDamage, prediction.HitChance, prediction.CritChance, prediction.Modifiers);
        
        // Preenche Defensor
        FillCombatant(defenderStats, target, prediction.DefenderGauge, prediction.DefenderSPGauge, prediction.MinDamage, prediction.MaxDamage, prediction.Avoid, prediction.CritChance, prediction.Modifiers);
    }

    private void FillCombatant(CombatantUIStats stats, Unit unit, HPGaugeState hpGauge, SPGaugeState spGauge, int minDmg, int maxDmg, float hitOrAvo, float crit, IReadOnlyList<CombatModifier> modifiers)
    {
        if (stats.Portrait != null)
        {
            stats.Portrait.sprite = unit.Data.Portrait;
            stats.Portrait.enabled = unit.Data.Portrait != null;
        }

        if (stats.NameText != null) stats.NameText.text = unit.Data.UnitName;
        if (stats.LevelText != null) stats.LevelText.text = $"LV  {unit.Data.Level}";

        if (stats.HPGauge != null) stats.HPGauge.SetGauge(hpGauge);
        if (stats.HPText != null) stats.HPText.text = $"{hpGauge.CurrentHP} / {hpGauge.MaxHP}";

        if (stats.SPText != null) stats.SPText.text = $"{spGauge.CurrentSP} / {spGauge.MaxSP}";

        if (stats.DamageText != null) stats.DamageText.text = $"{minDmg} - {maxDmg}";
        if (stats.HitOrAvoidText != null) stats.HitOrAvoidText.text = $"{hitOrAvo:0}";
        if (stats.CritText != null) stats.CritText.text = $"{crit:0}";

        // Modifiers
        if (stats.ModifiersContainer != null && modifierIconPrefab != null)
        {
            // Limpa os ícones antigos
            foreach (Transform child in stats.ModifiersContainer)
            {
                Destroy(child.gameObject);
            }

            // Cria um ícone para cada modificador ativo
            foreach (var mod in modifiers)
            {
                // Aqui você instanciará o prefab. No futuro, você usará mod.Icon se adicionar isso no CombatModifier
                GameObject icon = Instantiate(modifierIconPrefab, stats.ModifiersContainer);
                
                // Exemplo:
                // Image img = icon.GetComponent<Image>();
                // img.sprite = mod.Icon;
            }
        }

        // Crit/Damage Multiplier Display
        if (stats.CritMultiplierText != null)
        {
            // Procura o primeiro modificador que possua uma DisplayTag válida
            string tagToDisplay = "";
            foreach (var mod in modifiers)
            {
                if (!string.IsNullOrEmpty(mod.DisplayTag))
                {
                    tagToDisplay = mod.DisplayTag;
                    // Você pode concatenar tags se quiser: tagToDisplay += mod.DisplayTag + " ";
                    // Ou apenas pegar a primeira (mais importante), dependendo do design.
                    break; 
                }
            }

            stats.CritMultiplierText.text = tagToDisplay;
            stats.CritMultiplierText.gameObject.SetActive(!string.IsNullOrEmpty(tagToDisplay));
        }
    }

    public void Hide()
    {
        previewPanel.SetActive(false);
    }
}