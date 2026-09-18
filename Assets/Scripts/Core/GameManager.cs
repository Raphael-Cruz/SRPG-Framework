using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Guarda o nome da cena de exploração para podermos voltar depois
    public string PreviousSceneName { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToBattleScene(string battleSceneName)
    {
        // Salva a cena atual antes de mudar
        PreviousSceneName = SceneManager.GetActiveScene().name;
        
        // Carrega a cena de batalha
        SceneManager.LoadScene(battleSceneName);
    }

    public void ReturnToExploration()
    {
        if (!string.IsNullOrEmpty(PreviousSceneName))
        {
            SceneManager.LoadScene(PreviousSceneName);
        }
        else
        {
            Debug.LogWarning("Nenhuma cena anterior salva!");
        }
    }
}