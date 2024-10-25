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

public class EnemyAI : Character
{
    [Header("AI Behaviour")]
    [SerializeField] EnemyArchetype archetype;

    [Header("AI Chase Settings")]
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackRange;

    [Header("AI Lose Settings")]
    [SerializeField] protected float loseDelay;   
    [SerializeField] protected float loseRange;

    [Header("Ai Floating Params")]
    [SerializeField] protected Character target;


    public override void Step()
    {
        base.Step();
    }

    public override void ConstantStep()
    {
        base.ConstantStep(); 
    }
}
