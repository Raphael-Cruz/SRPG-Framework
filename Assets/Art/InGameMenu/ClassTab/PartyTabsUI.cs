using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stigmata.ClassEvolution
{
    /// <summary>
    /// Renders one tab per party member (replaces the old "Holy Order / Iron
    /// Rebellion" tabs). Call AddCharacterTab whenever a character joins the
    /// party — no other UI code needs to change to support a new character.
    /// </summary>
    public class PartyTabsUI : MonoBehaviour
    {
        [SerializeField] private Transform tabsContainer;
        [SerializeField] private PartyTabButtonUI tabButtonPrefab;

        public event Action<PartyMemberRuntimeState> OnCharacterSelected;

        private readonly List<PartyTabButtonUI> _spawnedTabs = new List<PartyTabButtonUI>();
        private PartyMemberRuntimeState _selected;

        /// <summary>Populate from the current party at screen open.</summary>
        public void Initialize(IEnumerable<PartyMemberRuntimeState> party)
        {
            foreach (var tab in _spawnedTabs) Destroy(tab.gameObject);
            _spawnedTabs.Clear();

            foreach (var member in party)
                AddCharacterTab(member);

            if (_spawnedTabs.Count > 0)
                SelectTab(_spawnedTabs[0]);
        }

        /// <summary>Call this when a new character joins the party.</summary>
        public void AddCharacterTab(PartyMemberRuntimeState member)
        {
            var tab = Instantiate(tabButtonPrefab, tabsContainer);
            tab.Setup(member, SelectTab);
            _spawnedTabs.Add(tab);
        }

        public void RemoveCharacterTab(string characterId)
        {
            var tab = _spawnedTabs.Find(t => t.Member.definition.characterId == characterId);
            if (tab == null) return;
            _spawnedTabs.Remove(tab);
            Destroy(tab.gameObject);
        }

        private void SelectTab(PartyTabButtonUI tab)
        {
            _selected = tab.Member;
            foreach (var t in _spawnedTabs)
                t.SetActiveState(t == tab);
            OnCharacterSelected?.Invoke(_selected);
        }
    }

    /// <summary>Single tab button — icon + character name, active/inactive styling.</summary>
    public class PartyTabButtonUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text label; // swap for TMP_Text if the project uses TextMeshPro
        [SerializeField] private Button button;
        [SerializeField] private Image activeUnderline;

        public PartyMemberRuntimeState Member { get; private set; }

        public void Setup(PartyMemberRuntimeState member, Action<PartyTabButtonUI> onClick)
        {
            Member = member;
            icon.sprite = member.definition.tabIcon;
            label.text = member.definition.displayName;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick(this));
            SetActiveState(false);
        }

        public void SetActiveState(bool active)
        {
            if (activeUnderline != null) activeUnderline.enabled = active;
            label.color = active ? Color.white : new Color(0.66f, 0.6f, 0.5f); // parch-dim vs parch
        }
    }
}
