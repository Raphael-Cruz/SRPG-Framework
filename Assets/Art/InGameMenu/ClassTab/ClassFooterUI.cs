using System;
using UnityEngine;
using UnityEngine.UI;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Populates the bottom footer for whichever class node is currently
    /// selected — mirrors the reference layout: preview portrait, stats
    /// gain, abilities preview, requirements list, Evolve button.
    /// Shown for every class in the tree (base, path and evolution nodes).
    /// </summary>
    public class ClassFooterUI : MonoBehaviour
    {
        [Header("Preview")]
        [SerializeField] private Image previewImage;
        [SerializeField] private Text classNameLabel;
        [SerializeField] private Text flavorTextLabel;

        [Header("Stats Gain")]
        [SerializeField] private Transform statsGainContainer;
        [SerializeField] private StatGainRowUI statGainRowPrefab;

        [Header("Abilities Preview")]
        [SerializeField] private Transform abilitiesContainer;
        [SerializeField] private AbilityRowUI abilityRowPrefab;

        [Header("Requirements")]
        [SerializeField] private Transform requirementsContainer;
        [SerializeField] private RequirementRowUI requirementRowPrefab;

        [Header("Evolve")]
        [SerializeField] private Button evolveButton;
        [SerializeField] private Text evolveButtonLabel;

        public event Action<ClassNodeData> OnEvolveConfirmed;

        private ClassNodeData _current;
        private PartyMemberRuntimeState _state;

        public void Show(ClassNodeData data, PartyMemberRuntimeState state)
        {
            _current = data;
            _state = state;

            bool unlocked = RequirementEvaluator.AllMet(data, state);

            classNameLabel.text = data.displayName;
            flavorTextLabel.text = data.flavorText;
            previewImage.sprite = unlocked && data.portrait != null ? data.portrait : state.definition.defaultSilhouette;
            previewImage.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            PopulateStatsGain(data);
            PopulateAbilities(data);
            PopulateRequirements(data, state);

            bool alreadyUnlockedAsCurrent = state.unlockedClassIds.Contains(data.classId);
            evolveButton.interactable = unlocked && !alreadyUnlockedAsCurrent;
            evolveButtonLabel.text = alreadyUnlockedAsCurrent ? "Current" : "Evolve";

            evolveButton.onClick.RemoveAllListeners();
            evolveButton.onClick.AddListener(() => OnEvolveConfirmed?.Invoke(_current));
        }

        private void PopulateStatsGain(ClassNodeData data)
        {
            foreach (Transform child in statsGainContainer) Destroy(child.gameObject);
            foreach (var stat in data.statGains)
            {
                var row = Instantiate(statGainRowPrefab, statsGainContainer);
                row.Setup(stat);
            }
        }

        private void PopulateAbilities(ClassNodeData data)
        {
            foreach (Transform child in abilitiesContainer) Destroy(child.gameObject);
            foreach (var ability in data.abilitiesPreview)
            {
                var row = Instantiate(abilityRowPrefab, abilitiesContainer);
                row.Setup(ability);
            }
        }

        private void PopulateRequirements(ClassNodeData data, PartyMemberRuntimeState state)
        {
            foreach (Transform child in requirementsContainer) Destroy(child.gameObject);
            foreach (var req in data.requirements)
            {
                var result = RequirementEvaluator.Evaluate(req, state);
                var row = Instantiate(requirementRowPrefab, requirementsContainer);
                row.Setup(req.label, result.currentDisplay, result.requiredDisplay, result.met);
            }
        }
    }
}
