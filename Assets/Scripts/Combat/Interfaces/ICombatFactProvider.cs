public interface ICombatFactProvider
{
    void ProvideFacts(
        CombatContext context,
        CombatFacts facts
    );
}