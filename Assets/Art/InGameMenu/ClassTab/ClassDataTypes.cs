using System;
using UnityEngine;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Used only to pick the node's border/accent color in the tree and footer.
    /// Never rendered as text in the UI.
    /// </summary>
    public enum ClassAlignment
    {
        None,
        Good,
        Neutral,
        Evil
    }

    public enum RequirementType
    {
        Level,
        Gold,
        Reputation,
        PriorClassSeal // e.g. "Justicar Seal" item/token gated behind another class
    }

    [Serializable]
    public struct StatGain
    {
        [Tooltip("Short label shown in the footer, e.g. HP, DEF, STR, RES, MAG, DEX")]
        public string statAbbreviation;
        public int value; // sign is applied at display time (+25, +5, ...)
    }

    [Serializable]
    public struct AbilityPreview
    {
        public Sprite icon;
        public string abilityName;
        [TextArea] public string description;
    }

    [Serializable]
    public struct ClassRequirement
    {
        public RequirementType type;

        [Tooltip("Display label, e.g. 'Character Level', 'Reputation: Inquisition', 'Gold'")]
        public string label;

        [Tooltip("Numeric amount required. Used for Level and Gold.")]
        public int requiredAmount;

        [Tooltip("Faction id for Reputation, or seal/item id for PriorClassSeal.")]
        public string requiredTag;

        [Tooltip("Reputation tier needed, e.g. 'Exalted'. Ignored for other types.")]
        public string requiredReputationTier;
    }
}
