// IAIAction.cs
public interface IAIAction
{
    Unit Actor { get; }

    AIActionOutcome Predict();

    void Execute(System.Action onComplete);
}