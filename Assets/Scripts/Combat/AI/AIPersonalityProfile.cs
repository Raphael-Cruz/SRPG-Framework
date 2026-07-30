
using UnityEngine;

[CreateAssetMenu(
    fileName = "AIPersonalityProfile",
    menuName = "AI/Personality Profile")]
public class AIPersonalityProfile : ScriptableObject
{
    [Header("Attack Scoring")]
    public float DamageWeight = 1f;
    public float KillBonus = 50f;
}