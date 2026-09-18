using System.Collections.Generic;
using UnityEngine;

public class VisualSpawner : MonoBehaviour
{
    private void Start()
    {
        // Se o GridManager ou UnitManager ainda não carregaram, a Unity vai resolver pela ordem de execução.
        // O ideal é chamar essa função a partir do BattleManager ou garantir que os managers carreguem antes.
        SpawnAllUnits();
    }

    public void SpawnAllUnits()
    {
        // Procura todos os objetos na cena que têm o script SpawnPoint
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Nenhum SpawnPoint encontrado na cena!");
        }

        foreach (SpawnPoint point in spawnPoints)
        {
            if (point.unitToSpawn == null)
            {
                Debug.LogWarning($"O SpawnPoint '{point.name}' não tem uma UnitData atribuída. Ignorando.", point.gameObject);
                continue;
            }

            // Descobre qual tile está debaixo daquele ponto
            GridTile tile = GridManager.Instance.GetTileFromWorldPosition(point.transform.position);

            if (tile == null)
            {
                Debug.LogError($"SpawnPoint '{point.name}' está fora dos limites do grid!", point.gameObject);
                continue;
            }

            if (!tile.IsWalkable)
            {
                Debug.LogError($"SpawnPoint '{point.name}' tentou nascer no Tile ({tile.X}, {tile.Y}) mas ele está bloqueado!", point.gameObject);
                continue;
            }

            // Invoca a unidade
            Unit unit = UnitManager.Instance.SpawnUnit(point.unitToSpawn, tile);

            if (unit != null)
            {
                // Se sua UnitData ou classe Unit tiver a propriedade IsPlayerControlled, você pode forçar aqui
                // unit.IsPlayerControlled = point.isPlayer;

                // Registra na ordem de iniciativa
                InitiativeOrderSystem.Instance.Register(unit);
                
                // Rotaciona a unidade para olhar para onde a setinha (azul/vermelha) estava apontando no editor
                unit.transform.rotation = point.transform.rotation;

                Debug.Log($"Spawnou {unit.name} no Tile ({tile.X}, {tile.Y}) via Visual Spawner.");
            }
        }

        // Se quiser que a batalha comece automaticamente assim que terminar de spawnar:
        BattleManager.Instance.StartBattle();
    }
}
