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


    public void Attack(Unit attacker, Unit target, System.Action onComplete = null, bool isSecondary = false)
    {
        if (!isSecondary && !CanAttack(attacker, target))
        {
            onComplete?.Invoke();
            return;
        }

        if (isSecondary)
        {
            if (attacker == null || target == null || !attacker.IsAlive || !target.IsAlive)
            {
                onComplete?.Invoke();
                return;
            }
        }

        int damage = CalculateDamage(attacker, target);
        
        if (!isSecondary)
        {
            attacker.ActionUsed();
        }

        StartCoroutine(AttackRoutine(attacker, target, damage, onComplete, isSecondary));
    }

    private IEnumerator AttackRoutine(Unit attacker, Unit target, int damage, System.Action onComplete, bool isSecondary)
    {
        CombatPresentationController presentation = FindObjectOfType<CombatPresentationController>();

        bool useCombatStage = (presentation != null && !isSecondary);

        if (useCombatStage)
        {
            // 1. Entra na visão de combate
            yield return presentation.EnterCombatViewRoutine(attacker, target);
            
            // 2. Animação no palco de batalha
            presentation.TriggerAttackerAnimation();
            yield return new WaitForSeconds(0.5f);

            // 3. Aplica dano
            target.TakeDamage(damage);
            Debug.Log($"{attacker.Data.UnitName} attacked {target.Data.UnitName} for {damage} damage no Palco de Batalha!");

            if (target.CurrentHP <= 0)
            {
                presentation.TriggerDefenderDeathAnimation();
            }

            yield return new WaitForSeconds(1.5f);

            // 4. Retorna para o Grid
            yield return presentation.ExitCombatViewRoutine();
        }
        else
        {
            // --- Fallback / Secondary Attack: Luta direto no Grid Tático ---
            if (attacker != null && target != null)
            {
                Vector3 dir = target.transform.position - attacker.transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    attacker.transform.rotation = Quaternion.LookRotation(dir.normalized);
            }

            // Força a liberação da animação no Animator (caso o Animator trave ataques fora do turno)
            if (isSecondary) attacker.Visual?.ForceAnimatorBool("isActiveTurn", true);

            attacker.Visual?.TriggerAttack();
            
            yield return new WaitForSeconds(0.5f);

            target.TakeDamage(damage);
            Debug.Log($"{attacker.Data.UnitName} attacked {target.Data.UnitName} for {damage} damage no Grid! (Secondary: {isSecondary})");
            
            yield return new WaitForSeconds(1.5f);

            // Restaura o bloqueio e limpa triggers enfileirados que não rodaram
            if (isSecondary) 
            {
                attacker.Visual?.ForceAnimatorBool("isActiveTurn", false);
                attacker.Visual?.ClearPendingTriggers();
            }
        }

        // --- REGRAS DE CONTRA-ATAQUE ---
        if (!isSecondary && target.IsAlive)
        {
            int dist = Mathf.Abs(target.CurrentTile.X - attacker.CurrentTile.X) + Mathf.Abs(target.CurrentTile.Y - attacker.CurrentTile.Y);
            if (dist == 1) // Apenas Melee
            {
                Vector3 dirToAttacker = (attacker.transform.position - target.transform.position).normalized;
                float dot = Vector3.Dot(target.transform.forward, dirToAttacker);
                bool wasBackstabbed = (dot < -0.2f);

                if (!wasBackstabbed)
                {
                    int roll = Random.Range(0, 100);
                    if (roll < target.Data.CounterChance)
                    {
                        Debug.Log($"{target.Data.UnitName} VAI CONTRA-ATACAR!");
                        bool counterFinished = false;
                        Attack(target, attacker, () => counterFinished = true, true);
                        yield return new WaitUntil(() => counterFinished);
                    }
                }
                else
                {
                    Debug.Log($"{target.Data.UnitName} levou backstab e não pode contra-atacar.");
                }
            }
        }

        onComplete?.Invoke();
    }

    public IEnumerator ExecuteAoORoutine(Unit attacker, Unit target)
    {
        Debug.Log($"{attacker.Data.UnitName} executa ATAQUE DE OPORTUNIDADE em {target.Data.UnitName}!");
        bool finished = false;
        Attack(attacker, target, () => finished = true, true);
        yield return new WaitUntil(() => finished);
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
        // 1. Monta o contexto com o atacante e o alvo
        CombatContext context = new CombatContext(
            attacker,
            target,
            attacker.Data.Attack,
            target.Data.Defense,
            attacker.Data.Accuracy,
            target.Data.Avoid,
            attacker.Data.Crit
        );

        // 2. Roda o simulador que vai passar por todos os Facts e Evaluators (terreno, flanking, etc)
        CombatPrediction prediction = CombatSimulatorFactory.CreateStandard().Simulate(context);

        // 3. Pega o resultado final processado!
        // (O Prediction tem MinDamage e MaxDamage. Por enquanto, usamos o MaxDamage para manter o determinismo anterior)
        return prediction.MaxDamage;
    }
}