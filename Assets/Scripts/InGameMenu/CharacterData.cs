using System.Collections.Generic;
using UnityEngine;

namespace InGameMenu.Data
{
    [System.Serializable]
    public class EquipmentSlots
    {
        public ItemData weapon;
        public ItemData offHand;
        public ItemData accessory;
        public ItemData head;
        public ItemData armor;
        public ItemData gloves;
        public ItemData boots;
    }

    [System.Serializable]
    public class CharacterStats
    {
        public int hpCurrent;
        public int hpMax;
        public int mov;
        public int init;
        [Range(0, 100)] public int eva;
        [Range(0, 100)] public int crit;
        public int str;
        public int mag;
        public int dex;
        public int def;
        public int res;
    }

    [CreateAssetMenu(fileName = "New Character", menuName = "InGameMenu/Character")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        public string characterName;
        public string className; // Myrmidon, Knight, Rogue, Ranger, Healer...
        public int level;
        public int exp;
        public int expToNextLevel;
        public Sprite portrait;

        [Header("Stats")]
        public CharacterStats stats;

        [Header("Equipment")]
        public EquipmentSlots equipment;

        [Header("Inventory (this unit's carried items)")]
        public List<InventoryStack> personalInventory = new List<InventoryStack>();
    }

    [System.Serializable]
    public class InventoryStack
    {
        public ItemData item;
        public int quantity = 1;
    }
}
