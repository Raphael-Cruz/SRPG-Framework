using System.Collections.Generic;
using UnityEngine;

public class PositionFactProvider : ICombatFactProvider
{
    public void ProvideFacts(CombatContext context, CombatFacts facts)
    {
        Unit attacker = context.Attacker;
        Unit defender = context.Defender;

        if (attacker == null || defender == null) return;

        // 1. BACKSTAB CHECK
        // Calcula a direção do defensor para o atacante
        Vector3 dirToAttacker = (attacker.transform.position - defender.transform.position).normalized;
        // Produto escalar com o 'forward' do defensor. 
        // Se < -0.2f, o atacante está consideravelmente nas costas do defensor.
        float dot = Vector3.Dot(defender.transform.forward, dirToAttacker);
        if (dot < -0.2f)
        {
            facts.Set("IsBackstabbed", true);
        }

        // 2. FLANKING CHECK
        int adjacentAlliesOfAttacker = 0;
        List<Unit> flankingUnits = new List<Unit>();

        // Procura todos os vizinhos do defensor
        List<GridTile> defenderNeighbors = GridManager.Instance.GetNeighbors(defender.EffectiveTile);
        
        // Verifica quem está ocupando esses vizinhos
        foreach (GridTile neighbor in defenderNeighbors)
        {
            Unit occupant = neighbor.Occupant;
            if (occupant != null && occupant.IsAlive && occupant.Team == attacker.Team)
            {
                adjacentAlliesOfAttacker++;
                flankingUnits.Add(occupant);
            }
        }

        // Se houver 2 ou mais, é flanqueamento!
        if (adjacentAlliesOfAttacker >= 2)
        {
            bool isOpposite = false;

            // Checa se existe qualquer par oposto
            for (int i = 0; i < flankingUnits.Count; i++)
            {
                for (int j = i + 1; j < flankingUnits.Count; j++)
                {
                    Unit u1 = flankingUnits[i];
                    Unit u2 = flankingUnits[j];

                    int dx1 = u1.EffectiveTile.X - defender.EffectiveTile.X;
                    int dy1 = u1.EffectiveTile.Y - defender.EffectiveTile.Y;
                    
                    int dx2 = u2.EffectiveTile.X - defender.EffectiveTile.X;
                    int dy2 = u2.EffectiveTile.Y - defender.EffectiveTile.Y;

                    // Se a soma dos deltas for 0, estão em lados perfeitamente opostos.
                    if (dx1 + dx2 == 0 && dy1 + dy2 == 0)
                    {
                        isOpposite = true;
                        break;
                    }
                }
                if (isOpposite) break;
            }

            if (isOpposite)
            {
                facts.Set("IsFlankedOpposite", true);
            }
            else
            {
                facts.Set("IsFlankedSide", true);
            }
        }
    }
}
