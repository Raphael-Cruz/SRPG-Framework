using UnityEngine;

[CreateAssetMenu(
    fileName = "AIPersonalityProfile",
    menuName = "AI/Personality Profile")]
public class AIPersonalityProfile : ScriptableObject
{
    [Header("Attack Scoring")]
    public float DamageWeight = 1f;
    public float KillBonus = 50f;

    [Header("Move Scoring")]
    [Tooltip("Multiplier applied when scoring a move that only sets up " +
             "a future attack rather than landing one this turn.")]
    public float MoveOpportunityDiscount = 0.5f;

    [Header("Safety Scoring")]
    [Tooltip("Penalty per enemy that could reach and attack a tile. " +
             "Higher values make the AI prioritize retreating over " +
             "repositioning for offense.")]
    public float ThreatWeight = 10f;
}