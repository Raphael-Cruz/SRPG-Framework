using UnityEngine;
using UnityEngine.UI;
using InGameMenu.Data;

namespace InGameMenu.UI
{
    /// <summary>
    /// Center "CHARACTER" panel: big portrait, EXP bar, equipment paperdoll slots
    /// (Weapon / Off Hand / Accessory / Head / Armor / Gloves / Boots) and the
    /// bottom stat grid (HP, MOV, INIT, EVA, CRIT / STR, MAG, DEX, DEF, RES).
    ///
    /// v2: each EquipmentSlotUI now gets portrait_frame.png (9-slice, reused as a
    /// generic ornate square socket) via the shared IronwardVisualLibrary. Full-body
    /// character art is still a PLACEHOLDER silhouette until you generate prompt 4.1.
    /// </summary>
    public class CharacterPanelUI : MonoBehaviour
    {
        [Header("Visual Library")]
        public InGameVisualLibrary visuals;

        [Header("Header")]
        public Text nameText;
        public Text classText;
        public Text expText;
        public Image expFill;
        public Image fullPortrait; // PLACEHOLDER art until full-body renders exist (prompt 4.1)

        [Header("Equipment Slots (drag targets)")]
        public EquipmentSlotUI weaponSlot;
        public EquipmentSlotUI offHandSlot;
        public EquipmentSlotUI accessorySlot;
        public EquipmentSlotUI headSlot;
        public EquipmentSlotUI armorSlot;
        public EquipmentSlotUI glovesSlot;
        public EquipmentSlotUI bootsSlot;

        [Header("Stat Grid - Left Column")]
        public Text hpValue;
        public Text movValue;
        public Text initValue;
        public Text evaValue;
        public Text critValue;

        [Header("Stat Grid - Right Column")]
        public Text strValue;
        public Text magValue;
        public Text dexValue;
        public Text defValue;
        public Text resValue;

        private void Awake()
        {
            foreach (var slot in new[] { weaponSlot, offHandSlot, accessorySlot, headSlot, armorSlot, glovesSlot, bootsSlot })
                slot?.Init(visuals);
        }

        public void Display(CharacterData c)
        {
            if (c == null) return;

            if (nameText != null) nameText.text = c.characterName;
            if (classText != null) classText.text = c.className;
            if (expText != null) expText.text = $"EXP  {c.exp:N0}/{c.expToNextLevel:N0}";

            if (expFill != null)
            {
                if (visuals != null && visuals.expBarFillOverride != null)
                {
                    expFill.sprite = visuals.expBarFillOverride;
                    expFill.color = Color.white;
                }
                else
                {
                    expFill.color = visuals != null ? visuals.expFillColor : new Color(0.62f, 0.51f, 0.19f);
                }
                expFill.fillAmount = c.expToNextLevel > 0 ? (float)c.exp / c.expToNextLevel : 0f;
            }

            if (fullPortrait != null) fullPortrait.sprite = c.portrait;

            weaponSlot?.SetItem(c.equipment.weapon);
            offHandSlot?.SetItem(c.equipment.offHand);
            accessorySlot?.SetItem(c.equipment.accessory);
            headSlot?.SetItem(c.equipment.head);
            armorSlot?.SetItem(c.equipment.armor);
            glovesSlot?.SetItem(c.equipment.gloves);
            bootsSlot?.SetItem(c.equipment.boots);

            var s = c.stats;
            if (hpValue != null) hpValue.text = $"{s.hpCurrent}/{s.hpMax}";
            if (movValue != null) movValue.text = s.mov.ToString();
            if (initValue != null) initValue.text = s.init.ToString();
            if (evaValue != null) evaValue.text = $"{s.eva}%";
            if (critValue != null) critValue.text = $"{s.crit}%";

            if (strValue != null) strValue.text = s.str.ToString();
            if (magValue != null) magValue.text = s.mag.ToString();
            if (dexValue != null) dexValue.text = s.dex.ToString();
            if (defValue != null) defValue.text = s.def.ToString();
            if (resValue != null) resValue.text = s.res.ToString();
        }
    }

    /// <summary>
    /// A single equipment paperdoll slot (Weapon, Head, Armor, etc). Frame comes
    /// from portrait_frame.png (square 9-slice) via IronwardVisualLibrary; the
    /// item icon itself falls back to nothing until item icon PNGs exist (prompt 5.2).
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour
    {
        public Image frame;   // gets portrait_frame.png
        public Image icon;
        public GameObject emptyStateOverlay; // faint icon shown when slot is empty

        public void Init(InGameVisualLibrary visuals)
        {
            visuals?.ApplySquareFrame(frame);
        }

        public void SetItem(ItemData item)
        {
            bool hasItem = item != null;
            if (icon != null)
            {
                icon.enabled = hasItem;
                if (hasItem) icon.sprite = item.icon; // PLACEHOLDER until item icons exist
            }
            if (emptyStateOverlay != null) emptyStateOverlay.SetActive(!hasItem);
        }
    }
}
