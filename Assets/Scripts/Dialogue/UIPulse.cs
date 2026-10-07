using UnityEngine;
using UnityEngine.UI;

public class UIPulse : MonoBehaviour
{
    [Header("Configurações do Pulso")]
    [Tooltip("Velocidade do pulso.")]
    public float pulseSpeed = 5f;

    [Header("Pulso de Transparência (Padrão)")]
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;
    
    [Header("Pulso de Tamanho (Alternativo)")]
    [Tooltip("Marque se quiser que o tamanho aumente e diminua ao invés da transparência.")]
    public bool pulseScale = false;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;

    private Image img;
    private Vector3 originalScale;

    private void Awake()
    {
        img = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Cria uma onda suave de 0 até 1 usando o tempo do jogo
        float pingPong = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        if (pulseScale)
        {
            float currentScale = Mathf.Lerp(minScale, maxScale, pingPong);
            transform.localScale = originalScale * currentScale;
        }
        else if (img != null)
        {
            Color c = img.color;
            c.a = Mathf.Lerp(minAlpha, maxAlpha, pingPong);
            img.color = c;
        }
    }

    private void OnDisable()
    {
        // Quando o objeto é desligado (caixa de diálogo some), resetamos para o estado normal
        if (img != null)
        {
            Color c = img.color;
            c.a = maxAlpha;
            img.color = c;
        }
        if (originalScale != Vector3.zero)
        {
            transform.localScale = originalScale;
        }
    }
}
