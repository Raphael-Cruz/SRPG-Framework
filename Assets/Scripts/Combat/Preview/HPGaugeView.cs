using UnityEngine;
using UnityEngine.UI;

public class HPGaugeView : MonoBehaviour
{
    [Header("Gauge Layers")]
    [SerializeField] private Image emptyFill;
    [SerializeField] private Image currentFill;
    [SerializeField] private Image predictionFill;


    [Header("Prediction Pulse")]
    [SerializeField] private bool enablePredictionPulse = true;
    [SerializeField] private float pulseSpeed = 2f;

    [SerializeField] private Color predictionLow = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color predictionHigh = new Color(1f, 1f, 0.3f);


    private float pulseTimer;


    private void Awake()
    {
        emptyFill.fillAmount = 1f;
    }


    private void Update()
    {
        if (!enablePredictionPulse)
            return;


        pulseTimer += Time.deltaTime;

        float t = (Mathf.Sin(pulseTimer * pulseSpeed) + 1f) * 0.5f;


        predictionFill.color = Color.Lerp(
            predictionLow,
            predictionHigh,
            t);
    }


public void SetGauge(HPGaugeState state)
{
    emptyFill.fillAmount = 1f;


    // Yellow shows the whole current HP area
    predictionFill.fillAmount = state.CurrentFill;


    // Red covers the HP that remains after damage
    float remainingHP =
        Mathf.Max(
            state.CurrentHP - state.PredictedDamage,
            0
        );


    currentFill.fillAmount =
        state.MaxHP <= 0
            ? 0f
            : (float)remainingHP / state.MaxHP;
}


    public void Hide()
    {
        currentFill.fillAmount = 0f;

        predictionFill.fillAmount = 0f;

        predictionFill.rectTransform.localRotation =
            Quaternion.identity;
    }
}