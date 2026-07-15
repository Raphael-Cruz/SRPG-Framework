using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static CombatSystem Instance { get; private set; }


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    public void Attack(Unit attacker, Unit target)
    {
        if(!CanAttack(attacker, target))
            return;


        int damage = CalculateDamage(attacker, target);

        attacker.ActionUsed();
        target.TakeDamage(damage);


   


        Debug.Log(
            $"{attacker.Data.UnitName} attacked {target.Data.UnitName} for {damage} damage"
        );
    }


    private bool CanAttack(Unit attacker, Unit target)
    {
        if(attacker == null || target == null)
            return false;


        if(!attacker.IsAlive)
            return false;


        if(!target.IsAlive)
            return false;


        if(!attacker.CanAct)
        {
            Debug.Log("Unit already acted");
            return false;
        }


        return true;
    }


    private int CalculateDamage(Unit attacker, Unit target)
    {
        int damage =
            attacker.Data.Attack -
            target.Data.Defense;


        return Mathf.Max(1, damage);
    }
}