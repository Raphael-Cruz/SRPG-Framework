using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "SRPG/Units/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string UnitName;

    public Sprite Portrait;

    public GameObject Prefab;

    [Header("Stats")]
    public int MaxHP = 20;
     public int MovementRange = 5;
     public int AttackRange = 1;
    public int Defense = 2;
    public int Speed = 5;
}

