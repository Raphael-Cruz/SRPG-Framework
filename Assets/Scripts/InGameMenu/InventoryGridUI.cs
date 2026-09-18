using System.Collections.Generic;
using UnityEngine;
using InGameMenu.Data;

namespace InGameMenu.UI
{
    /// <summary>
    /// Fills a GridLayoutGroup (recommended: 4-6 columns, cell 76x76, spacing 8)
    /// with InventorySlotUI instances. Put this on the "Content" object of a
    /// ScrollRect so the grid scrolls when the inventory exceeds the visible rows.
    /// </summary>
    public class InventoryGridUI : MonoBehaviour
    {
        public InventorySlotUI slotPrefab;
        public int totalSlots = 100; // matches "32 / 100" capacity counter in reference art
        public UnityEngine.UI.Text capacityText;

        [Header("Tooltip")]
        public ItemTooltipUI tooltip;

        private readonly List<InventorySlotUI> _spawned = new List<InventorySlotUI>();

        public void Populate(List<InventoryStack> stacks)
        {
            foreach (var s in _spawned) Destroy(s.gameObject);
            _spawned.Clear();

            for (int i = 0; i < totalSlots; i++)
            {
                var slot = Instantiate(slotPrefab, transform);
                var data = i < stacks.Count ? stacks[i] : null;
                slot.SetStack(data);

                slot.onHoverEnter = (stack, source) => tooltip?.Show(stack.item, source.transform as RectTransform);
                slot.onHoverExit = () => tooltip?.Hide();

                _spawned.Add(slot);
            }

            if (capacityText != null)
                capacityText.text = $"{stacks.Count} / {totalSlots}";
        }
    }
}
