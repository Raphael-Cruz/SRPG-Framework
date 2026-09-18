using UnityEngine;
using UnityEngine.Events;

public class BattleTrigger : MonoBehaviour
{
    [Tooltip("Nome da cena de batalha para a qual queremos ir (ex: BattleMap1)")]
    [SerializeField] private string battleSceneName = "BattleMap1";

    [Header("Events")]
    [Tooltip("Eventos disparados quando a batalha inicia (ex: Desabilitar controle do jogador, esconder UI de exploração).")]
    public UnityEvent OnBattleStart;

    private bool hasTriggered = false;
    private GameObject currentBattleInstance;

    private bool isPlayerInRange = false;

    private void Update()
    {
        // Verifica se o jogador está na área, se a batalha ainda não começou, e se ele apertou a tecla E
        if (isPlayerInRange && !hasTriggered && Input.GetKeyDown(KeyCode.E))
        {
            StartBattle();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Certifique-se de que a tag do seu player seja "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Pressione 'E' para iniciar a batalha.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // Exemplo: Disparar via código (ex: após um diálogo de Quest)
    public void StartBattle()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        Debug.Log("Iniciando Batalha!");

        // Avisa o GameManager para trocar de cena (e ele salvará a cena atual do quarto)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToBattleScene(battleSceneName);
        }
        else
        {
            Debug.LogError("Nenhum GameManager encontrado na cena! Certifique-se de que ele existe.");
        }

        // 2. Dispara eventos locais se precisar
        OnBattleStart?.Invoke();
    }
}
