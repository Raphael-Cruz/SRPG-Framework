using UnityEngine;
using UnityEngine.UI;

namespace Stigmata.ClassEvolution
{
    /// <summary>Single "HP +25" style row in the Stats Gain grid.</summary>
    public class StatGainRowUI : MonoBehaviour
    {
        [SerializeField] private Text abbreviationLabel;
        [SerializeField] private Text valueLabel;
        [SerializeField] private Color positiveColor = new Color(0.55f, 0.75f, 0.45f);
        [SerializeField] private Color negativeColor = new Color(0.75f, 0.35f, 0.3f);

        public void Setup(StatGain stat)
        {
            abbreviationLabel.text = stat.statAbbreviation;
            valueLabel.text = (stat.value >= 0 ? "+" : "") + stat.value;
            valueLabel.color = stat.value >= 0 ? positiveColor : negativeColor;
        }
    }

    /// <summary>Icon + name + description row in the Abilities Preview list.</summary>
    public class AbilityRowUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text descriptionLabel;

        public void Setup(AbilityPreview ability)
        {
            icon.sprite = ability.icon;
            icon.enabled = ability.icon != null;
            nameLabel.text = ability.abilityName;
            descriptionLabel.text = ability.description;
        }
    }

    /// <summary>Checkmark + label + "current / required" row in Requirements.</summary>
    public class RequirementRowUI : MonoBehaviour
    {
        [SerializeField] private Image checkIcon;
        [SerializeField] private Text label;
        [SerializeField] private Text valueLabel;
        [SerializeField] private Sprite metIconSprite;
        [SerializeField] private Sprite unmetIconSprite;
        [SerializeField] private Color metColor = new Color(0.55f, 0.75f, 0.45f);
        [SerializeField] private Color unmetColor = new Color(0.6f, 0.55f, 0.48f);

        public void Setup(string reqLabel, string current, string required, bool met)
        {
            label.text = reqLabel;
            valueLabel.text = $"{current} / {required}";
            valueLabel.color = met ? metColor : unmetColor;
            checkIcon.sprite = met ? metIconSprite : unmetIconSprite;
        }
    }
}
