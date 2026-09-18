using UnityEngine;
using UnityEngine.UI;

namespace InGameMenu.UI
{
    /// <summary>
    /// Put this on each panel's header block (Squad / Character / Inventory).
    /// Pulls the divider-line art from the shared IronwardVisualLibrary so all
    /// three panels stay visually identical without re-wiring each one by hand.
    /// </summary>
    public class PanelHeaderUI : MonoBehaviour
    {
        public InGameVisualLibrary visuals;
        public Text titleText;
        public Image dividerBelowTitle;
        public Button closeButton; // optional, Character/Inventory panels have this, Squad doesn't

        private void Awake()
        {
            if (visuals == null)
            {
                Debug.LogWarning($"{name}: no InGameVisualLibrary assigned, header chrome will look flat.");
                return;
            }
            visuals.ApplyDivider(dividerBelowTitle);
        }
    }
}