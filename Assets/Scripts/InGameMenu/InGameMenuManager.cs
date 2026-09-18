using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InGameMenu.UI
{
    /// <summary>
    /// Root controller for the in-game menu (Squad / Inventory / Map / Codex / System).
    /// Attach to the top-level "MenuRoot" GameObject.
    ///
    /// v2: now pulls the background, panel border frame and top-bar emblem from
    /// InGameMenuVisualLibrary instead of using flat colors, so it reflects the
    /// actual PNGs you generated (background-image.png, panel_frame_border.png,
    /// emblem_badge.png).
    /// </summary>
    public class InGameMenuManager : MonoBehaviour
    {
        [System.Serializable]
        public class MenuTab
        {
            public string tabName;          // "SQUAD", "INVENTORY", "MAP", "CODEX", "SYSTEM"
            public Button tabButton;
            public GameObject panelRoot;    // the content panel this tab shows
            public Image tabHighlight;      // thin underline/glow shown when active
        }

        [Header("Visual Library (assign InGameVisualLibrary asset)")]
        public InGameVisualLibrary visuals;

        [Header("Chrome References")]
        public Image backgroundImage;         // full-screen Image, gets background-image.png
        public Image topBarEmblem;            // circular sigil top-left, gets emblem_badge.png
        public List<Image> panelBorderFrames; // Just one for ,
                                               // gets panel_frame_border.png as simple

        [Header("Tabs")]
        public List<MenuTab> tabs = new List<MenuTab>();
        public int defaultTabIndex = 1; // Inventory selected by default, matches reference art

        [Header("Top Bar Resources (gold / essence / keys etc.)")]
        public Text goldText;
        public Text essenceText;
        public Text keysText;

        [Header("Colors")]
        public Color activeTabColor = new Color(0.9f, 0.79f, 0.53f);   // gold-bright
        public Color inactiveTabColor = new Color(0.54f, 0.51f, 0.45f, 0.85f);

        private int _currentIndex = -1;

        private void Start()
        {
            ApplyChrome();

            for (int i = 0; i < tabs.Count; i++)
            {
                int index = i; // capture for closure
                if (tabs[i].tabButton != null)
                    tabs[i].tabButton.onClick.AddListener(() => SelectTab(index));
            }
            SelectTab(defaultTabIndex);
        }

        /// <summary>Wires the 5 provided PNGs onto the actual UI Images. Safe to call
        /// again at runtime if you swap the library (e.g. after generating new art).</summary>
        public void ApplyChrome()
        {
            if (visuals == null)
            {
                Debug.LogWarning("InGameMenuManager: no InGameVisualLibrary assigned — using flat placeholder colors.");
                return;
            }

            if (backgroundImage != null && visuals.background != null)
            {
                backgroundImage.sprite = visuals.background;
                backgroundImage.type = Image.Type.Simple;
                backgroundImage.color = Color.white;
            }

            visuals.ApplyEmblem(topBarEmblem);

            if (panelBorderFrames != null)
                foreach (var frame in panelBorderFrames)
                    visuals.ApplyPanelFrame(frame);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= tabs.Count || index == _currentIndex) return;
            _currentIndex = index;

            for (int i = 0; i < tabs.Count; i++)
            {
                bool active = i == index;
                if (tabs[i].panelRoot != null)
                    tabs[i].panelRoot.SetActive(active);

                if (tabs[i].tabHighlight != null)
                    tabs[i].tabHighlight.color = active ? activeTabColor : new Color(0, 0, 0, 0);

                var label = tabs[i].tabButton != null ? tabs[i].tabButton.GetComponentInChildren<Text>() : null;
                if (label != null)
                    label.color = active ? activeTabColor : inactiveTabColor;
            }
        }

        public void SetResources(int gold, int essence, int keys)
        {
            if (goldText != null) goldText.text = gold.ToString("N0");
            if (essenceText != null) essenceText.text = essence.ToString("N0");
            if (keysText != null) keysText.text = keys.ToString();
        }
    }
}