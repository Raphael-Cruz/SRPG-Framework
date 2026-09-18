using UnityEngine;
using UnityEngine.UI;
using InGameMenu.Data;

namespace InGameMenu.UI
{
    /// <summary>
    /// One row in the left "SQUAD" list (portrait, name, class, level, HP bar).
    /// Prefab: Image(portrait) + Image(portraitFrame, child, overlaps portrait) +
    /// Text(name) + Text(class) + Text(level) + Image(hpFill) + Button(select).
    ///
    /// v2: portraitFrame now gets portrait_frame.png (9-slice) from the shared
    /// IronwardVisualLibrary instead of a plain border color. Character portrait
    /// art itself (the actual painted bust) is still a placeholder gradient —
    /// see PLACEHOLDER comment below — until you generate section 3.2 from the
    /// asset prompt list (one portrait per squad member).
    /// </summary>
    public class SquadListItemUI : MonoBehaviour
    {
        [Header("Visual Library")]
        public InGameVisualLibrary visuals;

        [Header("References")]
        public Image portrait;          // PLACEHOLDER art until portraits are generated
        public Image portraitFrame;     // gets portrait_frame.png
        public Text nameText;
        public Text classText;
        public Text levelText;
        public Text hpText;
        public Image hpFill;          // Image with Filled type, horizontal
        public Image selectionBadge;  // gets emblem_badge.png, shown only when selected
        public Button selectButton;

        [Header("Fallback (used only while hpBarFillOverride is unset)")]
        public Color hpHighColor = new Color(0.55f, 0.13f, 0.14f);

        private CharacterData _data;
        private System.Action<CharacterData> _onSelected;

        private void Awake()
        {
            if (visuals != null)
                visuals.ApplySquareFrame(portraitFrame);
        }

        public void Bind(CharacterData data, System.Action<CharacterData> onSelected)
        {
            _data = data;
            _onSelected = onSelected;

            // PLACEHOLDER: data.portrait should eventually be the real painted bust
            // (prompt 3.2). Until then this may be a gradient/silhouette sprite.
            if (portrait != null) portrait.sprite = data.portrait;

            if (nameText != null) nameText.text = data.characterName;
            if (classText != null) classText.text = data.className;
            if (levelText != null) levelText.text = "Lv. " + data.level;
            if (hpText != null) hpText.text = $"HP {data.stats.hpCurrent}/{data.stats.hpMax}";

            if (hpFill != null)
            {
                if (visuals != null && visuals.hpBarFillOverride != null)
                {
                    hpFill.sprite = visuals.hpBarFillOverride;
                    hpFill.color = Color.white;
                }
                else
                {
                    hpFill.color = visuals != null ? visuals.hpFillColor : hpHighColor;
                }
                hpFill.fillAmount = data.stats.hpMax > 0
                    ? (float)data.stats.hpCurrent / data.stats.hpMax
                    : 0f;
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => _onSelected?.Invoke(_data));
            }

            SetSelected(false);
        }

        public void SetSelected(bool selected)
        {
            if (visuals != null) visuals.ApplyEmblem(selectionBadge);
            if (selectionBadge != null) selectionBadge.gameObject.SetActive(selected);
        }
    }
}
