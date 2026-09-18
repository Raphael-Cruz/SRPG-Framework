using System;
using UnityEngine;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Recursively instantiates ClassNodeUI prefabs for a character's class
    /// tree (Base -> Path -> Evolution, or any depth childClasses defines).
    /// Wire this to PartyTabsUI.OnCharacterSelected.
    /// </summary>
    public class ClassTreeUI : MonoBehaviour
    {
        [SerializeField] private ClassNodeUI nodePrefab;

        [Tooltip("Root container the base-class node is spawned into. Each spawned node must expose a 'childrenContainer' RectTransform (see ClassNodeUI prefab) for its own children to be nested under.")]
        [SerializeField] private Transform treeRoot;

        [SerializeField] private ClassNodeUI.ChildrenContainerProvider childrenContainerProvider;

        public event Action<ClassNodeData> OnNodeSelected;

        private PartyMemberRuntimeState _state;

        public void BuildTree(PartyMemberRuntimeState state)
        {
            _state = state;
            foreach (Transform child in treeRoot) Destroy(child.gameObject);

            if (state.definition.rootClass == null) return;
            SpawnNode(state.definition.rootClass, treeRoot);
        }

        private void SpawnNode(ClassNodeData data, Transform parentContainer)
        {
            var node = Instantiate(nodePrefab, parentContainer);
            node.Setup(data, _state, _state.definition, HandleNodeClicked);

            var childrenContainer = node.GetComponent<ClassNodeUI.ChildrenContainerProvider>()?.ChildrenContainer;
            if (childrenContainer == null) return;

            foreach (var child in data.childClasses)
                SpawnNode(child, childrenContainer);
        }

        private void HandleNodeClicked(ClassNodeData data) => OnNodeSelected?.Invoke(data);
    }
}
