using UnityEngine;

namespace InGameMenu.Data
{
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum ItemCategory
    {
        Weapon,
        Armor,
        Accessory,
        Consumable,
        QuestItem,
        Material
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "InGameMenu/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemId;
        public string displayName;
        [TextArea(2, 5)] public string description;
        public Sprite icon;

        [Header("Classification")]
        public ItemCategory category;
        public ItemRarity rarity = ItemRarity.Common;

        [Header("Stacking")]
        public bool isStackable = true;
        public int maxStack = 99;

        [Header("Stats (optional, weapons/armor)")]
        public int strBonus;
        public int magBonus;
        public int dexBonus;
        public int defBonus;
        public int resBonus;

        // Returns the border/glow color associated with rarity.
        // Matches the Ironward Company palette (amber/gold accents on a near-black UI).
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return new Color(0.55f, 0.53f, 0.48f);      // worn steel grey
                case ItemRarity.Uncommon: return new Color(0.36f, 0.62f, 0.42f);    // verdigris green
                case ItemRarity.Rare: return new Color(0.31f, 0.52f, 0.78f);        // cold blue
                case ItemRarity.Epic: return new Color(0.62f, 0.35f, 0.78f);        // violet
                case ItemRarity.Legendary: return new Color(0.85f, 0.65f, 0.24f);   // brass gold
                default: return Color.white;
            }
        }
    }
}
