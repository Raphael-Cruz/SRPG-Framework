using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class CombatPreviewController : MonoBehaviour
{
    public static CombatPreviewController Instance { get; private set; }


    private CombatSimulator simulator;


    public CombatPrediction CurrentPrediction { get; private set; }

[SerializeField]
private CombatPreviewUI previewUI;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        

        Instance = this;


        InitializeSimulator();
    }



    private void InitializeSimulator()
    {
        simulator = new CombatSimulator(
            new List<ICombatFactProvider>(),
            new List<ICombatEvaluator>(),
            new CombatResolver()
        );
    }


public CombatPrediction PreviewAttack(
    Unit attacker,
    Unit target
)
{
    if(attacker == null || target == null)
    {
        return null;
    }


    int baseDamage =
        Mathf.Max(
            attacker.Data.Attack -
            target.Data.Defense,
            1
        );


    float baseHitChance = 100f;


    CombatContext context =
        new CombatContext(
            attacker,
            target,
            baseDamage,
            baseHitChance
        );


    CurrentPrediction =
        simulator.Simulate(
            context
        );
        previewUI.Show(
    attacker,
    target,
    CurrentPrediction);


    Debug.Log(
        $"Combat Preview: {attacker.Data.UnitName} -> {target.Data.UnitName} | Damage: {CurrentPrediction.Damage} | Hit: {CurrentPrediction.HitChance}%"
    );


    return CurrentPrediction;
}


    public void ClearPreview()
    {
        CurrentPrediction = null;

        previewUI.Hide();
            }
}