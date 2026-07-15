using UnityEngine;

// Handles a unit's on-model visual feedback: selection highlight and the
// "already spent this turn" dimmed/desaturated look.
//
// NOTE: this file wasn't part of the shared project files, so this is a
// best-effort reconstruction based on how Unit.cs and SelectionManager.cs
// already call Visual.Select() / Visual.Deselect(). If a UnitVisual.cs
// already exists in the actual project, just add the SetExhausted(bool)
// method (and the fields/UpdateVisual pattern it relies on) to that file
// instead of using this one.
public class UnitVisual : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Exhausted Look")]
    [Tooltip("0 = no change, 1 = fully grayscale and darkened.")]
    [Range(0f, 1f)]
    [SerializeField] private float exhaustedStrength = 0.6f;

    [Tooltip("Multiplies brightness when exhausted (1 = no darkening).")]
    [Range(0f, 1f)]
    [SerializeField] private float exhaustedBrightness = 0.6f;

    [Header("Selected Look")]
    [SerializeField] private Color selectionColor = Color.yellow;

    [Range(0f, 1f)]
    [SerializeField] private float selectionStrength = 0.5f;

    [Header("Active Turn Look")]
    [Tooltip("Subtle tint so the player can always tell whose turn it is, even before they select the unit.")]
    [SerializeField] private Color activeTurnColor = Color.cyan;

    [Range(0f, 1f)]
    [SerializeField] private float activeTurnStrength = 0.3f;

    private Color baseColor;
    private bool isSelected;
    private bool isExhausted;
    private bool isActiveTurn;


    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null)
        {
            baseColor = targetRenderer.material.color;
        }
    }


    public void Select()
    {
        isSelected = true;
        UpdateVisual();
    }


    public void Deselect()
    {
        isSelected = false;
        UpdateVisual();
    }


    // Called once when the unit spends its movement/action this turn, and
    // again with false when the turn system resets it for its next turn.
    public void SetExhausted(bool value)
    {
        isExhausted = value;
        UpdateVisual();
    }


    // Called by TurnManager (via Unit.SetActiveTurn) when this unit becomes
    // or stops being the current unit in the turn order. Independent of
    // Select()/Deselect() - a unit is "whose turn it is" the moment its
    // turn starts, even before the player has clicked it.
    public void SetActiveTurn(bool value)
    {
        isActiveTurn = value;
        UpdateVisual();
    }


    // Builds the final color by layering each active state on top of the
    // base color, rather than one state overriding another. This is what
    // lets exhausted + selected (and future states like hovered/poisoned/
    // buffed) combine instead of clobbering each other as more states get
    // added later.
    private void UpdateVisual()
    {
        if (targetRenderer == null)
            return;

        Color color = baseColor;

        if (isExhausted)
        {
            color = GetExhaustedColor(color);
        }

        if (isActiveTurn)
        {
            color = Color.Lerp(color, activeTurnColor, activeTurnStrength);
        }

        if (isSelected)
        {
            color = Color.Lerp(color, selectionColor, selectionStrength);
        }

        targetRenderer.material.color = color;
    }


    private Color GetExhaustedColor(Color color)
    {
        float gray = color.grayscale;
        Color desaturated = new Color(gray, gray, gray, color.a);

        Color blended = Color.Lerp(color, desaturated, exhaustedStrength);
        blended *= exhaustedBrightness;
        blended.a = color.a;

        return blended;
    }
}
