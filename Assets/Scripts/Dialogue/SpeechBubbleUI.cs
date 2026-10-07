using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SpeechBubbleUI : MonoBehaviour
{
    [Header("Identificação")]
    [Tooltip("ID único deste NPC. Tem que ser EXATAMENTE igual ao Speaker ID no arquivo de conversa.")]
    public string speakerID;

    [Header("UI do Balão")]
    [SerializeField] private GameObject bubbleContainer;
    [SerializeField] private TMP_Text speechText;
    
    [Header("Configurações")]
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float horizontalOffset = 0f;
    [Tooltip("Segundos por letra no efeito de digitação.")]
    public float typingSpeed = 0.03f;
    [Tooltip("Pausa em segundos quando o texto enche a caixa, antes de limpar e continuar.")]
    public float pageFlipPause = 1.5f;

    private Transform mainCamera;
    private Vector3 baseWorldPosition;
    private Canvas myCanvas;

    // Lista Telefônica global
    public static Dictionary<string, SpeechBubbleUI> Registry = new Dictionary<string, SpeechBubbleUI>();

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(speakerID)) Registry[speakerID] = this;
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(speakerID) && Registry.ContainsKey(speakerID)) Registry.Remove(speakerID);
    }

    private void Awake()
    {
        myCanvas = GetComponent<Canvas>();
        if (myCanvas != null) myCanvas.enabled = false;
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
        if (Camera.main != null) mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        Collider npcCollider = FindMainCollider();
        if (npcCollider != null)
        {
            baseWorldPosition = npcCollider.bounds.center;
            baseWorldPosition.y = npcCollider.bounds.max.y + heightOffset;
        }
        else
        {
            baseWorldPosition = transform.position + new Vector3(0, heightOffset, 0);
        }
        transform.position = baseWorldPosition;
    }

    private Collider FindMainCollider()
    {
        Transform npcRoot = transform;
        while (npcRoot.parent != null)
        {
            if (npcRoot.GetComponent<CharacterController>() != null || npcRoot.GetComponent<Animator>() != null)
                break;
            npcRoot = npcRoot.parent;
        }

        Collider[] colliders = npcRoot.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger) return col;
        }
        return GetComponentInParent<Collider>();
    }

    private void LateUpdate()
    {
        if ((myCanvas != null && myCanvas.enabled) || (bubbleContainer != null && bubbleContainer.activeSelf))
        {
            if (mainCamera != null) 
            {
                transform.position = baseWorldPosition + (mainCamera.right * horizontalOffset);
                transform.forward = mainCamera.forward;
            }
        }
    }

    public void ShowSpeech(string text)
    {
        StopAllCoroutines();
        StartCoroutine(Routine_ShowSpeech(text));
    }

    private IEnumerator Routine_ShowSpeech(string text)
    {
        // Liga tudo
        if (myCanvas != null) myCanvas.enabled = true;
        if (bubbleContainer != null) bubbleContainer.SetActive(true);
        if (speechText == null) yield break;

        // Guarda a altura máxima que o texto pode ocupar (o tamanho do RectTransform do TMP)
        RectTransform textRect = speechText.GetComponent<RectTransform>();
        float maxHeight = textRect.rect.height;

        // Começa limpo
        speechText.text = "";
        speechText.enableWordWrapping = true;
        speechText.overflowMode = TextOverflowModes.Overflow;

        // Buffer: guarda o texto que está sendo mostrado na "página" atual
        string currentPageText = "";

        for (int i = 0; i < text.Length; i++)
        {
            currentPageText += text[i];
            speechText.text = currentPageText;
            speechText.ForceMeshUpdate();

            // Checa se o texto renderizado ultrapassou a altura da caixa
            float renderedHeight = speechText.preferredHeight;

            if (renderedHeight > maxHeight)
            {
                // O texto estourou! Remove a última letra que causou o estouro
                currentPageText = currentPageText.Substring(0, currentPageText.Length - 1);
                speechText.text = currentPageText;

                // Pausa para o player ler o que está na tela
                yield return new WaitForSeconds(pageFlipPause);

                // Limpa a tela e recomeça a partir da letra que estourou
                currentPageText = "" + text[i];
                speechText.text = currentPageText;
            }

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void HideSpeech()
    {
        StopAllCoroutines();
        if (myCanvas != null) myCanvas.enabled = false;
        if (bubbleContainer != null) bubbleContainer.SetActive(false);
    }
}