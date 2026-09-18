using System.Collections.Generic;
using UnityEngine;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// One node in a character's class tree. A tree is built by nesting these
    /// through childClasses: Base -> Path -> Evolution (or any depth).
    /// </summary>
    [CreateAssetMenu(menuName = "Stigmata/Class Evolution/Class Node", fileName = "ClassNode_")]
    public class ClassNodeData : ScriptableObject
    {
        [Header("Identity")]
        public string classId;
        public string displayName;
        [TextArea] public string flavorText;

        [Header("Visuals")]
        public Sprite portrait;
        [Tooltip("Only affects the node's accent/border color. No text label is ever shown for this.")]
        public ClassAlignment alignment = ClassAlignment.None;

        [Header("Unlock")]
        public List<ClassRequirement> requirements = new List<ClassRequirement>();

        [Header("Footer preview content")]
        public List<StatGain> statGains = new List<StatGain>();
        public List<AbilityPreview> abilitiesPreview = new List<AbilityPreview>();

        [Header("Tree structure")]
        public List<ClassNodeData> childClasses = new List<ClassNodeData>();

        public bool IsBaseClass => childClasses.Count > 0 && requirements.Count == 0;
        public bool IsLeaf => childClasses.Count == 0;
    }
}
