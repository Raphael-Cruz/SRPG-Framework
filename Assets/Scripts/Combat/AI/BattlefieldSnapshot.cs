using System.Collections.Generic;

public class BattlefieldSnapshot
{
    public Unit Self { get; }

    public IReadOnlyList<Unit> Allies { get; }

    public IReadOnlyList<Unit> Enemies { get; }

    public BattlefieldSnapshot(
        Unit self,
        List<Unit> allies,
        List<Unit> enemies)
    {
        Self = self;
        Allies = allies;
        Enemies = enemies;
    }
}