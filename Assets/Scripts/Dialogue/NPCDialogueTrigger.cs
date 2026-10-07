using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dados do Diálogo")]
    [Tooltip("O arquivo único que contém a primeira conversa e as frases aleatórias.")]
    [SerializeField] private DialogueSequence dialogueData;

    [Header("Animação")]
    [Tooltip("Arraste o Animator deste NPC (opcional)")]
    [SerializeField] private Animator npcAnimator;

    private bool hasTalkedOnce = false;
    private bool isPlayerInRange = false;
    private bool isTalking = false; // Flag para garantir que o NPC continue virado
    private float cooldownTimer = 0f;
    private Transform playerTransform;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && cooldownTimer <= 0f)
        {
            if (DialogueManager.Instance != null && !DialogueManager.Instance.IsDialogueActive)
            {
                StartInteraction();
            }
        }
    }

    private void LateUpdate()
    {
        // LateUpdate acontece DEPOIS que o Animator calcula a pose. 
        // Isso quebra a trava do "Bake Into Pose" e nos dá a palavra final na rotação.
        if (isTalking && playerTransform != null)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            dirToPlayer.y = 0;
            if (dirToPlayer != Vector3.zero)
            {
                BasicBehaviour npcBehaviour = GetComponent<BasicBehaviour>();
                if (npcBehaviour != null)
                {
                    // Se o NPC usa o mesmo sistema de movimento do player
                    npcBehaviour.SetLastDirection(dirToPlayer);
                }
                else
                {
                    // Sobrescreve a rotação da animação
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * 5f);
                }
            }
        }
    }

    private void StartInteraction()
    {
        if (dialogueData == null) return;

        List<DialogueLine> linesToPlay = new List<DialogueLine>();

        if (!hasTalkedOnce && dialogueData.firstTimeLines != null && dialogueData.firstTimeLines.Count > 0)
        {
            linesToPlay = dialogueData.firstTimeLines;
        }
        else if (dialogueData.randomRepeatedLines != null && dialogueData.randomRepeatedLines.Count > 0)
        {
            int randomIndex = Random.Range(0, dialogueData.randomRepeatedLines.Count);
            linesToPlay.Add(dialogueData.randomRepeatedLines[randomIndex]);
        }
        else if (dialogueData.firstTimeLines != null && dialogueData.firstTimeLines.Count > 0)
        {
            linesToPlay = dialogueData.firstTimeLines;
        }

        if (linesToPlay.Count == 0) return;

        isTalking = true;

        // Rotação inicial do Player (o NPC começa a girar no Update)
        if (playerTransform != null)
        {
            Vector3 dirToNpc = (transform.position - playerTransform.position).normalized;
            dirToNpc.y = 0;
            if (dirToNpc != Vector3.zero)
            {
                BasicBehaviour basicBehaviour = playerTransform.GetComponent<BasicBehaviour>();
                if (basicBehaviour != null)
                {
                    basicBehaviour.SetLastDirection(dirToNpc);
                }
                else
                {
                    playerTransform.rotation = Quaternion.LookRotation(dirToNpc);
                }
            }
        }

        // Liga a animação no NPC
        if (npcAnimator != null)
        {
            npcAnimator.SetBool("TalkingWithThePlayer", true);
        }

        DialogueManager.Instance.StartInteractiveDialogue(linesToPlay, OnDialogueEnded);
    }

    private void OnDialogueEnded()
    {
        hasTalkedOnce = true;
        isTalking = false;
        cooldownTimer = 1.5f; // Trava por 1.5 segundos

        // Desliga a animação para voltar ao idle original
        if (npcAnimator != null)
        {
            npcAnimator.SetBool("TalkingWithThePlayer", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (playerTransform == other.transform)
            {
                playerTransform = null;
            }
        }
    }
}
