using System.Collections.Generic;
using UnityEngine;

public class TesterModifier : MonoBehaviour
{
    [SerializeField] private ModifierListView modifierListView;


    private void Start()
    {
        // TestModifiers(); // Desabilitado temporariamente (UI refatorada)
    }


    private void TestModifiers()
    {
        List<CombatModifier> test = new()
        {
            new CombatModifier(
                CombatModifierType.HitChance,
                -15,
                "Forest"
            ),

            new CombatModifier(
                CombatModifierType.CriticalChance,
                100,
                "Flanking"
            )
        };


        modifierListView.Show(test);
    }
}