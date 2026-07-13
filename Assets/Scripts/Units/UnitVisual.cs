using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [Header("Rendering")]
    private Renderer[] targetRenderers;

    [Header("Exhausted Look")]
    [Tooltip("0 = no desaturation, 1 = fully grayscale")]
    [Range(0f, 1f)]
    [SerializeField] private float exhaustedStrength = 0.5f;

    [Tooltip("Brightness multiplier when exhausted")]
    [Range(0f, 1f)]
    [SerializeField] private float exhaustedBrightness = 0.5f;

    [Header("Selected Look")]
    [SerializeField] private Color selectionColor = Color.yellow;

    [Range(0f, 1f)]
    [SerializeField] private float selectionStrength = 0.5f;


    private Color[] baseColors;

    private bool isSelected;
    private bool isExhausted;


    private void Awake()
    {
        targetRenderers = GetComponentsInChildren<Renderer>();

        baseColors = new Color[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            baseColors[i] = targetRenderers[i].material.color;
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


    public void SetExhausted(bool value)
    {
        isExhausted = value;
        UpdateVisual();
    }


    private void UpdateVisual()
    {
        if (targetRenderers == null)
            return;


        for (int i = 0; i < targetRenderers.Length; i++)
        {
            Color color = baseColors[i];


            // Apply exhausted state first
            if (isExhausted)
            {
                color = GetExhaustedColor(color);
            }


            // Apply selection on top
            if (isSelected)
            {
                color = Color.Lerp(
                    color,
                    selectionColor,
                    selectionStrength
                );
            }


            targetRenderers[i].material.color = color;
        }
    }


    private Color GetExhaustedColor(Color color)
    {
        float gray = color.grayscale;

        Color desaturated = Color.Lerp(
            color,
            new Color(gray, gray, gray, color.a),
            exhaustedStrength
        );


        return desaturated * exhaustedBrightness;
    }
}