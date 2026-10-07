using UnityEngine;
using UnityEngine.UI;

public class HPGaugeView : MonoBehaviour
{
    [Header("Os Dois Sliders")]
    [Tooltip("Slider que fica POR TRÁS. Ele vai até o HP ATUAL (ex: 30) e tem a cor vermelha.")]
    public Slider damagePreviewSlider;
    
    [Tooltip("Slider que fica NA FRENTE. Ele vai até a VIDA RESTANTE (ex: 25) e tem a cor verde.")]
    public Slider currentHPSlider;

    [Header("Efeito de Piscar")]
    [Tooltip("Arraste aqui o objeto 'Fill' do seu damagePreviewSlider")]
    public Image imagemDoDano;
    public float velocidadeDePiscar = 5f;
    public Color corForte = new Color(1f, 0.2f, 0.2f, 1f);
    public Color corFraca = new Color(1f, 0.2f, 0.2f, 0.4f);

    private bool vaiTomarDano;

    public void SetGauge(HPGaugeState state)
    {
        float maxHP = Mathf.Max(state.MaxHP, 1);
        float hpAtual = state.CurrentHP;                 // Ex: 30
        float dano = state.PredictedDamage;              // Ex: 5
        float hpRestante = Mathf.Max(hpAtual - dano, 0); // Ex: 25

        // Configura os valores máximos
        if (damagePreviewSlider != null) damagePreviewSlider.maxValue = maxHP;
        if (currentHPSlider != null) currentHPSlider.maxValue = maxHP;

        // O Slider de trás (Dano) preenche até o HP Atual (30)
        if (damagePreviewSlider != null) damagePreviewSlider.value = hpAtual;

        // O Slider da frente (Vida) preenche até a Vida Restante (25)
        if (currentHPSlider != null) currentHPSlider.value = hpRestante;

        vaiTomarDano = (dano > 0);
    }

    private void Update()
    {
        // Se vai tomar dano, pisca a imagem vermelha
        if (vaiTomarDano && imagemDoDano != null)
        {
            float tempo = (Mathf.Sin(Time.time * velocidadeDePiscar) + 1f) / 2f;
            imagemDoDano.color = Color.Lerp(corFraca, corForte, tempo);
        }
        else if (imagemDoDano != null)
        {
            // Se não vai tomar dano, deixa da mesma cor da barra de vida normal
            // para não ficar vermelho sem motivo
            imagemDoDano.color = Color.clear;
        }
    }

    public void Hide()
    {
        if (damagePreviewSlider != null) damagePreviewSlider.value = 0;
        if (currentHPSlider != null) currentHPSlider.value = 0;
        vaiTomarDano = false;
    }
}