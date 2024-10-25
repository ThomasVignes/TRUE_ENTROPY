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

    [Header("AI Search Settings")]
    [SerializeField] protected float searchRange;
    [SerializeField] protected float loseRange;
    [SerializeField] protected float loseDelay;

    [Header("Ai Floating Params")]
    [SerializeField] protected Character target;


    public override void Step()
    {
        base.Step();

        if (target == null)
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
    }

    public override void ConstantStep()
    {
        base.ConstantStep(); 
    }

    public void Detect()
    {

    }

    public void HeavyLoop()
    {
        var dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist < attackRange)
        {
            if (dist < windupRange)
            {
                if (dist < attackRange)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }
        else
        {

        }
    }
}
