using UnityEngine;
using UnityEngine.UI;
using InGameMenu.Data;

namespace InGameMenu.UI
{
    /// <summary>
    /// Small floating card (parchment/dark panel) that follows near the cursor
    /// showing item name (colored by rarity), category and description.
    /// Parent this under the top-level Canvas so it draws above everything.
    /// </summary>
    public class ItemTooltipUI : MonoBehaviour
    {
        public RectTransform panel;
        public Text nameText;
        public Text categoryText;
        public Text descriptionText;
        public Image accentBar; // thin colored strip along the tooltip's top edge

        private void Awake() => Hide();

        public void Show(ItemData item, RectTransform anchorSource)
        {
            if (item == null) return;
            gameObject.SetActive(true);

            nameText.text = item.displayName;
            nameText.color = item.GetRarityColor();
            categoryText.text = item.category.ToString().ToUpper();
            descriptionText.text = item.description;
            if (accentBar != null) accentBar.color = item.GetRarityColor();

            // Position just above/right of the hovered slot.
            if (panel != null && anchorSource != null)
                panel.position = anchorSource.position + new Vector3(20f, 20f, 0f);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
