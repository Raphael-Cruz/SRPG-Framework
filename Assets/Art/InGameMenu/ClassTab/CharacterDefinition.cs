using UnityEngine;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Authored data for one party member. Add a new asset whenever a new
    /// character can join the party — PartyTabsUI reads a list of these
    /// (or their runtime states) and spawns one tab per entry, so no UI
    /// code needs to change to support a new character.
    /// </summary>
    [CreateAssetMenu(menuName = "Stigmata/Class Evolution/Character Definition", fileName = "Character_")]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string characterId;
        public string displayName;

        [Header("Tab")]
        public Sprite tabIcon;

        [Header("Class Tree")]
        [Tooltip("The root/base class node for this character's tree.")]
        public ClassNodeData rootClass;

        [Header("Locked State")]
        [Tooltip("Shown as the footer/tree portrait for any class node this character hasn't unlocked yet.")]
        public Sprite defaultSilhouette;
    }
}
