using System.Collections.Generic;
using UnityEngine;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Entry point for the Class Evolution screen. Wires the reusable party
    /// tab bar to the class tree, and the class tree to the footer.
    /// </summary>
    public class ClassEvolutionScreen : MonoBehaviour
    {
        [SerializeField] private PartyTabsUI partyTabs;
        [SerializeField] private ClassTreeUI classTree;
        [SerializeField] private ClassFooterUI footer;

        private PartyMemberRuntimeState _activeMember;

        private void OnEnable()
        {
            partyTabs.OnCharacterSelected += HandleCharacterSelected;
            classTree.OnNodeSelected += HandleNodeSelected;
            footer.OnEvolveConfirmed += HandleEvolveConfirmed;
        }

        private void OnDisable()
        {
            partyTabs.OnCharacterSelected -= HandleCharacterSelected;
            classTree.OnNodeSelected -= HandleNodeSelected;
            footer.OnEvolveConfirmed -= HandleEvolveConfirmed;
        }

        /// <summary>Call when opening the screen with the current party.</summary>
        public void Open(IEnumerable<PartyMemberRuntimeState> party)
        {
            partyTabs.Initialize(party);
        }

        private void HandleCharacterSelected(PartyMemberRuntimeState member)
        {
            _activeMember = member;
            classTree.BuildTree(member);

            // Default the footer to the character's current class.
            if (member.definition.rootClass != null)
                footer.Show(FindNodeById(member.definition.rootClass, member.currentClassId), member);
        }

        private void HandleNodeSelected(ClassNodeData data)
        {
            if (_activeMember == null) return;
            footer.Show(data, _activeMember);
        }

        private void HandleEvolveConfirmed(ClassNodeData data)
        {
            if (_activeMember == null || !RequirementEvaluator.AllMet(data, _activeMember)) return;

            _activeMember.unlockedClassIds.Add(data.classId);
            _activeMember.currentClassId = data.classId;

            // Refresh the tree (locked states may have changed further down)
            // and the footer (Evolve button becomes "Current").
            classTree.BuildTree(_activeMember);
            footer.Show(data, _activeMember);
        }

        private ClassNodeData FindNodeById(ClassNodeData node, string id)
        {
            if (node.classId == id) return node;
            foreach (var child in node.childClasses)
            {
                var found = FindNodeById(child, id);
                if (found != null) return found;
            }
            return node; // fallback: root
        }
    }
}
