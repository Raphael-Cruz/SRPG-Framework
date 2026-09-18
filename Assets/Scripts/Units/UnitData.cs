using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "SRPG/Units/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Identity")]
    public string UnitName;

    public Sprite Portrait;

    public GameObject Prefab;
    
    [Tooltip("Marca se esta unidade é o personagem principal da história.")]
    public bool IsMainCharacter;



    [Header("Stats")]
    public int Level = 1;
    public int MaxHP = 20;
    public int MaxSP = 10;
    public int MovementRange = 5;
    public int Speed = 5;


[Header("Combat")]
public int Attack = 5;
public int Defense = 2;

public int Accuracy = 100;
public int Avoid = 0;
public int Crit = 5;

public int AttackRange = 1;
}