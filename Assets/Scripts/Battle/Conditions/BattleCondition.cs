using UnityEngine;

public abstract class BattleCondition : ScriptableObject
{
    /// <summary>
    /// Verifica se a condição foi atendida no estado atual da batalha.
    /// </summary>
    /// <param name="manager">O BattleManager contendo todas as unidades e estado da batalha.</param>
    /// <returns>Verdadeiro se a condição foi atingida.</returns>
    public abstract bool IsConditionMet(BattleManager manager);
}
