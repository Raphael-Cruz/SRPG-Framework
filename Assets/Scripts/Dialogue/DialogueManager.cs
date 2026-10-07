using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

[System.Serializable]
public class DialogueBoxUI
{
    [Tooltip("O objeto raiz/pai dessa caixa de diálogo específica.")]
    public GameObject root;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    
    [Tooltip("Arraste aqui a imagem (ex: triângulo piscando) que indica o fim da fala.")]
    public GameObject nextIcon;

    public void SetUpLine(DialogueLine line)
    {
        if (nameText != null) nameText.text = line.speakerName;
        if (dialogueText != null) dialogueText.text = ""; // Limpa para o efeito de digitação
        if (nextIcon != null) nextIcon.SetActive(false); // Esconde o ícone no início
        
        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.sprite = line.portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }
    }

    public void Show()
    {
        if (root != null) root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

    public bool IsActive => root != null && root.activeSelf;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Painéis de Diálogo")]
    [Tooltip("Preencha aqui os elementos da UI da caixa do PLAYER")]
    [SerializeField] private DialogueBoxUI playerBox;
    
    [Tooltip("Preencha aqui os elementos da UI da caixa do NPC")]
    [SerializeField] private DialogueBoxUI npcBox;

    [Header("Configurações")]
    [Tooltip("Velocidade do efeito máquina de escrever (segundos por letra).")]
    public float typingSpeed = 0.02f;

    private System.Collections.Generic.List<DialogueLine> currentLines;
    private int currentLineIndex;
    
    private bool isTyping;
    private Coroutine typingCoroutine;

    // Ação chamada quando o diálogo termina
    private Action onDialogueEnded;

    public bool IsDialogueActive => playerBox.IsActive || npcBox.IsActive;

    private void Awake()
    {
        Instance = this;
        playerBox.Hide();
        npcBox.Hide();
    }

    private void Update()
    {
        if (IsDialogueActive && Input.GetKeyDown(KeyCode.E))
        {
            OnActionPressed();
        }
    }

    public void StartInteractiveDialogue(System.Collections.Generic.List<DialogueLine> linesToPlay, Action onEnd = null)
    {
        currentLines = linesToPlay;
        currentLineIndex = 0;
        onDialogueEnded = onEnd; // Guarda o callback
        ShowLine(currentLines[currentLineIndex]);
    }

    public void OnActionPressed()
    {
        if (!IsDialogueActive) return;

        if (isTyping)
        {
            // Se apertar enquanto digita, o texto se completa instantaneamente
            DialogueBoxUI currentBox = currentLines[currentLineIndex].isPlayer ? playerBox : npcBox;
            CompleteTyping(currentBox, currentLines[currentLineIndex].text);
        }
        else
        {
            // Se já acabou de digitar, avança para a próxima fala
            NextLine();
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        if (currentLineIndex < currentLines.Count)
        {
            ShowLine(currentLines[currentLineIndex]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void ShowLine(DialogueLine line)
    {
        playerBox.Hide();
        npcBox.Hide();

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (line.isPlayer)
        {
            playerBox.SetUpLine(line);
            playerBox.Show();
            typingCoroutine = StartCoroutine(TypeDialogue(line.text, playerBox));
        }
        else
        {
            npcBox.SetUpLine(line);
            npcBox.Show();
            typingCoroutine = StartCoroutine(TypeDialogue(line.text, npcBox));
        }
    }

    private IEnumerator TypeDialogue(string fullText, DialogueBoxUI box)
    {
        isTyping = true;
        box.dialogueText.text = "";

        foreach (char letter in fullText.ToCharArray())
        {
            box.dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        CompleteTyping(box, fullText);
    }

    private void CompleteTyping(DialogueBoxUI box, string fullText)
    {
        isTyping = false;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        box.dialogueText.text = fullText;
        if (box.nextIcon != null) box.nextIcon.SetActive(true); // Mostra o triângulo indicativo
    }

    private void EndDialogue()
    {
        playerBox.Hide();
        npcBox.Hide();
        currentLines = null;

        // Dispara o callback para o NPC saber que acabou e reseta
        onDialogueEnded?.Invoke();
        onDialogueEnded = null;
    }
}