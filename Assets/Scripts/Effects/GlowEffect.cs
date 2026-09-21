using TMPro;
using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    [Header("Glow Effect")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!enableGlow || text == null)
            return;

        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f
        );

        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}