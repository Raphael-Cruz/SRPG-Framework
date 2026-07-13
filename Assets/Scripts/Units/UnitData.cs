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
    public int Movement = 5;
    public int Attack = 5;
    public int Defense = 2;
    public int Speed = 5;
}