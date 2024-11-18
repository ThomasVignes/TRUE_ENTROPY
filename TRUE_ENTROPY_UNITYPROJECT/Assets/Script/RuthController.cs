using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RuthController : PlayerController
{
    [Header("Ruth Settings")]
    [SerializeField] float drawDelay;
    [SerializeField] float castTime;
    [SerializeField] bool canMoveDuringCast;
    [SerializeField] float delayBeforeArmCross;


    bool casting, recovery;
    float castTimer;
    float crossTimer;
    bool crossed;

    public override void Init()
    {
        base.Init();
    }

    public override void Step()
    {
        base.Step();

        if (Mathf.Abs(agent.velocity.magnitude) <= 0)
            crossTimer += Time.deltaTime;
        else
            crossTimer = 0;

        animator.SetBool("ArmsCrossed", crossTimer > delayBeforeArmCross);


        if (recovery)
        {
            if (castTimer < Time.time)
            {
                recovery = false;
            }
        }
    }

    public override void Special(Vector3 spot, GameObject hitObject)
    {
        if (recovery)
            return;

        base.Special(spot, hitObject);

        animator.SetTrigger("Snap");
        castTimer = Time.time + castTime;

        recovery = true;
    }

    public override void ToggleSpecial(bool active)
    {
        casting = active;
        animator.SetBool("Aim", active);

        if (casting)
        {
            if (canMoveDuringCast)
                ToggleRun(false);

            castTimer = Time.time + drawDelay;
        }

        base.ToggleSpecial(active);

        if (!canMoveDuringCast)
        {
            computing = false;
            agent.isStopped = active;

            if (active)
            {
                PausePath();
            }
            else
            {
                agent.ResetPath();
            }
        }
    }
}
