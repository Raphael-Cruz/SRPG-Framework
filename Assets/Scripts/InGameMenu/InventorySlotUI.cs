using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InGameMenu.Data;

namespace InGameMenu.UI
{
    /// <summary>
    /// One cell in the right "INVENTORY" grid.
    ///
    /// v2: no dedicated inventory-slot-frame PNG has been provided yet, so
    /// `slotBackground` falls back to a flat dark square via
    /// IronwardVisualLibrary.ApplyInventorySlotFrame(). Once you generate prompt
    /// "5.1 Frame de slot vazio da grade", just assign it to the library asset's
    /// `inventorySlotFrame` field — no code changes needed, every slot picks it
    /// up automatically.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Visual Library")]
        public InGameVisualLibrary visuals;

        [Header("References")]
        public Image slotBackground;   // gets inventorySlotFrame if provided, else flat dark fallback
        public Image itemIcon;         // PLACEHOLDER until item icon PNGs exist (prompt 5.2)
        public Image rarityBorder;     // thin frame recolored per-rarity (color-only, no art yet)
        public Text quantityText;      // hidden if quantity <= 1
        public GameObject quantityBadge;

        private InventoryStack _stack;
        public System.Action<InventoryStack, InventorySlotUI> onHoverEnter;
        public System.Action onHoverExit;
        public System.Action<InventoryStack> onClick;

        private void Awake()
        {
            visuals?.ApplyInventorySlotFrame(slotBackground);
        }

        public void SetStack(InventoryStack stack)
        {
            _stack = stack;
            bool hasItem = stack != null && stack.item != null;

            if (itemIcon != null)
            {
                itemIcon.enabled = hasItem;
                if (hasItem) itemIcon.sprite = stack.item.icon;
            }

            if (rarityBorder != null)
            {
                var fallback = visuals != null ? visuals.fallbackSlotDark : new Color(0, 0, 0, 0.45f);
                rarityBorder.color = hasItem ? stack.item.GetRarityColor() : fallback;
            }

            bool showQty = hasItem && stack.item.isStackable && stack.quantity > 1;
            if (quantityBadge != null) quantityBadge.SetActive(showQty);
            if (quantityText != null && showQty) quantityText.text = stack.quantity.ToString();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_stack != null && _stack.item != null)
                onHoverEnter?.Invoke(_stack, this);
        }

        public void OnPointerExit(PointerEventData eventData) => onHoverExit?.Invoke();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_stack != null) onClick?.Invoke(_stack);
        }
    }
}
