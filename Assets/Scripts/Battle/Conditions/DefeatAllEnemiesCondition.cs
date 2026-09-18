using UnityEngine;

[CreateAssetMenu(menuName = "SRPG/Battle Conditions/Defeat All Enemies")]
public class DefeatAllEnemiesCondition : BattleCondition
{
    public override bool IsConditionMet(BattleManager manager)
    {
        // Pega todos os inimigos na ordem de iniciativa e checa se algum está vivo.
        foreach (Unit unit in InitiativeOrderSystem.Instance.AllUnits)
        {
            // Se for um inimigo, não for nulo e estiver vivo, a condição AINDA NÃO foi atingida.
            if (unit != null && !unit.IsPlayerControlled && unit.IsAlive)
            {
                return false; 
            }
        }
        
        // Se o loop terminar sem achar inimigos vivos, a condição foi atingida.
        return true;
    }
}
