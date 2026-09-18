using UnityEngine;

[CreateAssetMenu(menuName = "SRPG/Battle Conditions/Main Character Death")]
public class MainCharacterDeathCondition : BattleCondition
{
    public override bool IsConditionMet(BattleManager manager)
    {
        // Procura entre todas as unidades do jogador
        foreach (Unit unit in InitiativeOrderSystem.Instance.AllUnits)
        {
            if (unit != null && unit.IsPlayerControlled && unit.IsMainCharacter)
            {
                // Se o personagem principal está com 0 HP (ou morto), a condição de morte foi atingida
                if (!unit.IsAlive)
                {
                    return true;
                }
            }
        }
        
        // Ninguém morreu, ou o Main Character ainda está vivo.
        return false;
    }
}
