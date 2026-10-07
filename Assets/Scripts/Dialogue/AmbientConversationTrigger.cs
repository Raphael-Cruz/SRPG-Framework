using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AmbientConversationTrigger : MonoBehaviour
{
    [Header("Conversa de Cenário")]
    [Tooltip("O arquivo que contém as falas da conversa.")]
    public DialogueSequence conversationData;

    [Tooltip("Pausa de silêncio (em segundos) entre um balão apagar e o próximo NPC responder.")]
    public float pauseBetweenLines = 0.5f;

    // Rastreia se a conversa principal (firstTimeLines) já foi completada ATÉ O FIM
    private bool firstTimeCompleted = false;
    private Coroutine conversationCoroutine;
    
    private List<SpeechBubbleUI> activeBubbles = new List<SpeechBubbleUI>();
    private SpeechBubbleUI lastBubble = null;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (conversationCoroutine != null) return;
        if (conversationData == null) return;

        if (!firstTimeCompleted)
        {
            // Primeira vez: toca a sequência inteira
            if (conversationData.firstTimeLines != null && conversationData.firstTimeLines.Count > 0)
            {
                conversationCoroutine = StartCoroutine(PlaySequence(conversationData.firstTimeLines, true));
            }
        }
        else
        {
            // Vezes seguintes: sorteia UMA fala aleatória do randomRepeatedLines
            if (conversationData.randomRepeatedLines != null && conversationData.randomRepeatedLines.Count > 0)
            {
                int randomIndex = Random.Range(0, conversationData.randomRepeatedLines.Count);
                DialogueLine randomLine = conversationData.randomRepeatedLines[randomIndex];
                
                // Cria uma lista temporária com essa única fala
                List<DialogueLine> singleLine = new List<DialogueLine> { randomLine };
                conversationCoroutine = StartCoroutine(PlaySequence(singleLine, false));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Para a conversa na hora
        if (conversationCoroutine != null)
        {
            StopCoroutine(conversationCoroutine);
            conversationCoroutine = null;
        }

        // Apaga todos os balões ativos
        foreach (var bubble in activeBubbles)
        {
            if (bubble != null) bubble.HideSpeech();
        }
        activeBubbles.Clear();
        lastBubble = null;
    }

    private IEnumerator PlaySequence(List<DialogueLine> lines, bool markAsFirstTimeCompleted)
    {
        bool isSingleLine = lines.Count == 1;

        for (int i = 0; i < lines.Count; i++)
        {
            DialogueLine line = lines[i];

            if (SpeechBubbleUI.Registry.TryGetValue(line.speakerID, out SpeechBubbleUI bubble))
            {
                // Se mudou de personagem, apaga o balão anterior
                if (lastBubble != null && lastBubble != bubble)
                {
                    lastBubble.HideSpeech();
                    activeBubbles.Remove(lastBubble);
                }

                bubble.ShowSpeech(line.text);
                
                if (!activeBubbles.Contains(bubble)) activeBubbles.Add(bubble);
                lastBubble = bubble;

                // Espera o tempo de digitação + o displayTime configurado na fala
                float waitTime = (line.text.Length * bubble.typingSpeed) + line.displayTime;
                yield return new WaitForSeconds(waitTime);

                // Pausa entre falas (se não for a última)
                if (i < lines.Count - 1)
                {
                    yield return new WaitForSeconds(pauseBetweenLines);
                }
            }
        }

        // A sequência terminou com sucesso (o player não saiu no meio)
        if (markAsFirstTimeCompleted)
        {
            firstTimeCompleted = true;
        }

        if (!isSingleLine)
        {
            // Conversa com múltiplas falas: apaga o último balão
            if (lastBubble != null)
            {
                lastBubble.HideSpeech();
                activeBubbles.Remove(lastBubble);
            }
        }
        // Se for uma única fala, o balão fica até o player sair do trigger (OnTriggerExit)

        conversationCoroutine = null;
    }
}
