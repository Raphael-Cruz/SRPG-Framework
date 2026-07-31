using System.Collections.Generic;

public class AIPerception
{
    private readonly UnitManager unitManager;

    public AIPerception(UnitManager unitManager)
    {
        this.unitManager = unitManager;
    }

    public BattlefieldSnapshot Observe(Unit self)
    {
        List<Unit> allies = new List<Unit>();
        List<Unit> enemies = new List<Unit>();

        foreach (Unit unit in unitManager.Units)
        {
            if (unit == null || !unit.IsAlive || unit == self)
                continue;

            if (unit.Team == self.Team)
            {
                allies.Add(unit);
            }
            else
            {
                enemies.Add(unit);
            }
        }

        return new BattlefieldSnapshot(self, allies, enemies);
    }
}