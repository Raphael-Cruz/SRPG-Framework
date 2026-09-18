using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// One node (base / path / evolution) in the tree. Locked nodes render
    /// dark gray with the character's default silhouette and are not
    /// clickable — only the alignment border color hints at the path,
    /// never a "Good/Neutral/Evil" label.
    /// </summary>
    public class ClassNodeUI : MonoBehaviour
    {
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image borderAccent;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text nameLabel; // swap for TMP_Text if desired
        [SerializeField] private Button button;

        [Header("Alignment colors")]
        [SerializeField] private Color goodColor = new Color(0.36f, 0.56f, 0.78f);
        [SerializeField] private Color neutralColor = new Color(0.54f, 0.45f, 0.28f);
        [SerializeField] private Color evilColor = new Color(0.63f, 0.21f, 0.18f);
        [SerializeField] private Color noneColor = new Color(0.54f, 0.45f, 0.28f, 0.4f);

        [Header("Locked state")]
        [SerializeField] private Color unlockedBackground = new Color(0.10f, 0.09f, 0.07f);
        [SerializeField] private Color lockedBackground = new Color(0.06f, 0.06f, 0.06f);
        [SerializeField] private Color lockedTextColor = new Color(0.35f, 0.33f, 0.30f);
        [SerializeField] private Color unlockedTextColor = new Color(0.91f, 0.87f, 0.79f);

        public ClassNodeData Data { get; private set; }
        public bool Unlocked { get; private set; }

        public void Setup(ClassNodeData data, PartyMemberRuntimeState state, CharacterDefinition definition, Action<ClassNodeData> onClick)
        {
            Data = data;
            Unlocked = RequirementEvaluator.AllMet(data, state);

            nameLabel.text = data.displayName;
            nameLabel.color = Unlocked ? unlockedTextColor : lockedTextColor;
            backgroundImage.color = Unlocked ? unlockedBackground : lockedBackground;
            borderAccent.color = Unlocked ? AlignmentColor(data.alignment) : new Color(0.2f, 0.2f, 0.2f);

            portraitImage.sprite = Unlocked
                ? (data.portrait != null ? data.portrait : definition.defaultSilhouette)
                : definition.defaultSilhouette;
            portraitImage.color = Unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            button.interactable = Unlocked;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick(data));
        }

        private Color AlignmentColor(ClassAlignment alignment)
        {
            switch (alignment)
            {
                case ClassAlignment.Good: return goodColor;
                case ClassAlignment.Neutral: return neutralColor;
                case ClassAlignment.Evil: return evilColor;
                default: return noneColor;
            }
        }

        /// <summary>
        /// Optional companion component on the node prefab that exposes where
        /// this node's children (its evolutions) should be instantiated —
        /// e.g. the "align-row" placeholder under a path node. Add it only on
        /// prefab variants that actually have children slots.
        /// </summary>
        public class ChildrenContainerProvider : MonoBehaviour
        {
            [SerializeField] private RectTransform childrenContainer;
            public RectTransform ChildrenContainer => childrenContainer;
        }
    }
}
