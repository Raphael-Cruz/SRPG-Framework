public struct HPGaugeState
{
    public int CurrentHP { get; }
    public int MaxHP { get; }
    public int PredictedDamage { get; }


    public float CurrentFill =>
        MaxHP <= 0
            ? 0f
            : (float)CurrentHP / MaxHP;


    public float DamageFill =>
        MaxHP <= 0
            ? 0f
            : (float)PredictedDamage / MaxHP;


    public HPGaugeState(
        int currentHP,
        int maxHP,
        int predictedDamage)
    {
        CurrentHP = currentHP;
        MaxHP = maxHP;
        PredictedDamage = predictedDamage;
    }
}