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
    public int Speed = 5;


[Header("Combat")]
public int Attack = 5;
public int Defense = 2;

public int Accuracy = 100;
public int Avoid = 0;

public int AttackRange = 1;
}