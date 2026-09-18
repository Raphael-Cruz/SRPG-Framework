using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    [Tooltip("A unidade que deve nascer neste ponto.")]
    public UnitData unitToSpawn;
    
    [Tooltip("Ative para o jogador. Desative para inimigos.")]
    public bool isPlayer = false;

    // Apenas para visualização no Editor, não executa no jogo compilado
    private void OnDrawGizmos()
    {
        // Define a cor: Azul para jogador, Vermelho para inimigo
        Gizmos.color = isPlayer ? new Color(0, 0, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
        
        // Desenha uma esfera indicando onde o personagem vai nascer
        Gizmos.DrawSphere(transform.position, 0.5f);

        // Desenha uma linha apontando a direção que o personagem vai olhar
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}
