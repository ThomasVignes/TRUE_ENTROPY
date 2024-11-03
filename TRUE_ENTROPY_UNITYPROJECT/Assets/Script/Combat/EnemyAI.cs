using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyArchetype
{
    Neutral,
    Heavy,
    Aggressor,
    Scared
}

public enum EnemyFreeRoamArchetype
{
    Still,
    Patrol,
    Wander
}


public class EnemyAI : Character
{
    [Header("AI Behaviour")]
    [SerializeField] EnemyArchetype archetype;
    [SerializeField] EnemyFreeRoamArchetype freeRoamArchetype;

    [Header("AI Chase Settings")]
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackRange, windupRange;
    [SerializeField] protected float attackRecovery;
    [SerializeField] protected float chainDelay;

    [Header("AI Search Settings")]
    [SerializeField] protected float searchRange;
    [SerializeField] protected float loseRange;
    [SerializeField] protected float loseDelay;

    [Header("Ai Floating Params")]
    [SerializeField] protected Character target;
    [SerializeField] protected Hitshape hitshape;

    [Header("Gizmo")]
    [SerializeField] protected bool showGizmos;

    bool active, aggroed, canMove;
    bool attacking, recovering;
    float attackTimer, recoveryTimer, chainTimer;

    public override void Init()
    {
        base.Init();

        active = true;
        target = GameManager.Instance.Player;

        canMove = true;
    }

    public override void Step()
    {
        if (!active)
            return;

        base.Step();

        if (stunned)
            return;

        if (!aggroed)
        {
            switch (freeRoamArchetype)
            {
                case EnemyFreeRoamArchetype.Still: 
                    
                    break;

                case EnemyFreeRoamArchetype.Patrol: 
                    
                    break;
                    
                case EnemyFreeRoamArchetype.Wander: 
                    
                    break;
            }

            Detect();
            return;
        }

        if (attacking)
        {
            if (!recovering && attackTimer < Time.time)
            {
                animator.SetTrigger("Melee");

                recovering = true;
                recoveryTimer = Time.time + attackRecovery;

                hitshape.Trigger();
            }

            if (recovering && recoveryTimer < Time.time)
            {
                animator.SetBool("Windup", false);
                animator.SetTrigger("MeleeRecovery");

                ResumePath();
                canMove = true;
                attacking = false;
                recovering = false;

                chainTimer = Time.time + chainDelay;
            }

            return;
        }

        switch (archetype)
        {
            case EnemyArchetype.Neutral:
                break; 
            
            case EnemyArchetype.Heavy:
                HeavyLoop();
                break;

            case EnemyArchetype.Aggressor:
                break;

            case EnemyArchetype.Scared:
                break;
        }

        if (canMove)
            SetDestination(target.transform.position);
    }

    public override void ConstantStep()
    {
        base.ConstantStep(); 
    }

    public void Detect()
    {
        var fromTo = transform.position - target.transform.position;
        var dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist < searchRange)
        {
            aggroed = true;
        }
    }

    public void ManualAggro()
    {
        if (target == null)
            return;

        aggroed = true;
    }

    public void HeavyLoop()
    {
        var dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist < attackRange)
        {
            if (chainTimer < Time.time)
                Melee();
        }
        else
        {
            animator.SetBool("Windup", dist < windupRange);
        }

        canMove = dist > attackRange;
    }

    void Melee()
    {
        animator.SetBool("Windup", true);

        canMove = false;
        attacking = true;
        attackTimer = Time.time + attackDelay;

        PausePath();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, windupRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
