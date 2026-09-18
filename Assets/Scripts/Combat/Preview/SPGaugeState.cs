public struct SPGaugeState
{
    public int CurrentSP { get; }
    public int MaxSP { get; }
    public int PredictedCost { get; }

    public float CurrentFill =>
        MaxSP <= 0
            ? 0f
            : (float)CurrentSP / MaxSP;

    public float CostFill =>
        MaxSP <= 0
            ? 0f
            : (float)PredictedCost / MaxSP;

    public SPGaugeState(
        int currentSP,
        int maxSP,
        int predictedCost)
    {
        CurrentSP = currentSP;
        MaxSP = maxSP;
        PredictedCost = predictedCost;
    }
}
