using UnityEngine;
using System.Collections;

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


    public void Attack(Unit attacker, Unit target, System.Action onComplete = null)
    {
        if(!CanAttack(attacker, target))
        {
            onComplete?.Invoke();
            return;
        }

        int damage = CalculateDamage(attacker, target);
        attacker.ActionUsed();

        StartCoroutine(AttackRoutine(attacker, target, damage, onComplete));
    }

    private IEnumerator AttackRoutine(Unit attacker, Unit target, int damage, System.Action onComplete)
    {
        // Face the target before attacking
        if (attacker != null && target != null)
        {
            Vector3 dir = target.transform.position - attacker.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                attacker.transform.rotation = Quaternion.LookRotation(dir.normalized);
        }

        attacker.Visual?.TriggerAttack();
        
        yield return new WaitForSeconds(0.5f);

        target.TakeDamage(damage);

        Debug.Log(
            $"{attacker.Data.UnitName} attacked {target.Data.UnitName} for {damage} damage"
        );
        
        // Wait additional time for the attack recovery (and potential death animation) to play
        // before releasing control back to the turn system
        yield return new WaitForSeconds(1.5f);
        
        onComplete?.Invoke();
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