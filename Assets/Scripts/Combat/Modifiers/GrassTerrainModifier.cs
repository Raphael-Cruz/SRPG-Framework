using UnityEngine;

public class GrassTerrainModifier : CombatModifier
{
    private readonly int nightAvoidBonus = 15; // Bônus adicional noturno

    // O construtor já define o tipo como Avoid automaticamente
    public GrassTerrainModifier(float baseValue, string source = "Grass Terrain") 
        : base(CombatModifierType.Avoid, baseValue, source)
    {
    }

    // Sobrescrevemos o método para injetar a lógica de clima/horário
    public override int GetContextualValue(CombatContext context)
    {
        // Começa com o valor base (ex: os 10% padrão da grama)
        int totalBonus = Mathf.RoundToInt(Value);

        // Checa a condição de tempo no contexto da batalha
        // (Certifique-se de que a propriedade CurrentTime existe no seu CombatContext)
        if (context.CurrentTime == TimeOfDay.Night)
        {
            totalBonus += nightAvoidBonus; // Adiciona os +15%
        }

        return totalBonus;
    }
}