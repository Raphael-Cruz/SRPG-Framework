using UnityEngine;

namespace InGameMenu.UI
{
    /// <summary>
    /// Single source of truth for the shared UI chrome assets (frames, dividers,
    /// emblem, background). Create one asset via
    /// Create → Ironward → Visual Library, assign the 5 sprites you already have,
    /// and every panel/slot in the menu pulls from here instead of being wired
    /// by hand in a dozen prefabs.
    ///
    /// STATUS OF EACH FIELD (as of the assets you've sent so far):
    ///   ✅ background            -> background-image.png
    ///   ✅ panelFrameBorder      -> panel_frame_border.png   (9-slice, rectangular)
    ///   ✅ squareFrame           -> portrait_frame.png       (9-slice, square — reused
    ///                               for squad portraits AND equipment paperdoll slots)
    ///   ✅ dividerLine           -> divider_line.png
    ///   ✅ emblemBadge           -> emblem_badge.png         (top-left sigil)
    ///   ⬜ inventorySlotFrame    -> NOT PROVIDED YET. Falls back to a flat dark
    ///                               square (see InventorySlotUI). Generate with
    ///                               prompt "5.1 Frame de slot vazio da grade".
    ///   ⬜ itemIconFallback      -> NOT PROVIDED YET. Falls back to a plain colored
    ///                               dot per category. Generate prompts 5.2 (lotes A-E).
    ///   ⬜ hpBarFillTexture      -> NOT PROVIDED YET. Falls back to a flat color fill.
    ///   ⬜ expBarFillTexture     -> NOT PROVIDED YET. Falls back to a flat color fill.
    ///   ⬜ smallIconsAtlas       -> NOT PROVIDED YET (close X, gear, lock, eye, coin,
    ///                               essence, key, sort arrow, quick-action icons).
    ///                               Falls back to TextMeshPro glyph placeholders.
    /// </summary>
    [CreateAssetMenu(fileName = "InGameVisualLibrary", menuName = "InGame/Visual Library")]
    public class InGameVisualLibrary : ScriptableObject
    {
        [Header("✅ Provided")]
        public Sprite background;
        public Sprite panelFrameBorder;   // rectangular 9-slice, used on the 3 main panels
        public Sprite squareFrame;        // square 9-slice, used on portraits + equipment slots
        public Sprite dividerLine;        // ornamental line under each panel header
        public Sprite emblemBadge;        // circular compass sigil (top bar + selection badge)

        [Header("⬜ Placeholder — not provided yet")]
        public Sprite inventorySlotFrame; // leave null to use flat-color fallback
        public Sprite hpBarFillOverride;  // leave null to use flat-color fallback
        public Sprite expBarFillOverride; // leave null to use flat-color fallback

        [Header("Fallback colors (used only while art above is missing)")]
        public Color fallbackPanelDark = new Color(0.055f, 0.05f, 0.043f, 0.9f);
        public Color fallbackSlotDark = new Color(0f, 0f, 0f, 0.45f);
        public Color hpFillColor = new Color(0.42f, 0.11f, 0.12f);
        public Color expFillColor = new Color(0.62f, 0.51f, 0.19f);

        /// <summary>Applies the rectangular 9-slice frame to a panel's border Image.</summary>
        public void ApplyPanelFrame(UnityEngine.UI.Image target)
        {
            if (target == null) return;
            if (panelFrameBorder != null)
            {
                target.sprite = panelFrameBorder;
                target.type = UnityEngine.UI.Image.Type.Sliced;
                target.color = Color.white;
            }
            else
            {
                target.sprite = null;
                target.type = UnityEngine.UI.Image.Type.Simple;
                target.color = fallbackPanelDark;
            }
        }

        /// <summary>Applies the square 9-slice frame (portraits, equipment slots).</summary>
        public void ApplySquareFrame(UnityEngine.UI.Image target)
        {
            if (target == null) return;
            if (squareFrame != null)
            {
                target.sprite = squareFrame;
                target.type = UnityEngine.UI.Image.Type.Sliced;
                target.color = Color.white;
            }
        }

        public void ApplyDivider(UnityEngine.UI.Image target)
        {
            if (target == null) return;
            if (dividerLine != null)
            {
                target.sprite = dividerLine;
                target.type = UnityEngine.UI.Image.Type.Simple;
                target.preserveAspect = true;
                target.color = Color.white;
            }
            else
            {
                target.gameObject.SetActive(false);
            }
        }

        public void ApplyEmblem(UnityEngine.UI.Image target)
        {
            if (target == null) return;
            if (emblemBadge != null)
            {
                target.sprite = emblemBadge;
                target.type = UnityEngine.UI.Image.Type.Simple;
                target.preserveAspect = true;
                target.color = Color.white;
            }
        }

        /// <summary>Inventory cell frame: uses the dedicated slot frame if present,
        /// otherwise falls back to a flat dark square so the grid still reads fine.</summary>
        public void ApplyInventorySlotFrame(UnityEngine.UI.Image target)
        {
            if (target == null) return;
            if (inventorySlotFrame != null)
            {
                target.sprite = inventorySlotFrame;
                target.type = UnityEngine.UI.Image.Type.Sliced;
                target.color = Color.white;
            }
            else
            {
                target.sprite = null;
                target.type = UnityEngine.UI.Image.Type.Simple;
                target.color = fallbackSlotDark;
            }
        }
    }
}