using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuthController : PlayerController
{
    [Header("Ruth Settings")]
    [SerializeField] float drawDelay;
    [SerializeField] float castTime;
    [SerializeField] bool canMoveDuringCast;

    bool casting, recovery;
    float castTimer;

    public override void Init()
    {
        base.Init();
    }

    public override void Step()
    {
        base.Step();

        if (recovery)
        {
            if (castTimer < Time.time)
            {
                recovery = false;
            }
        }
    }

    public override void Special(Vector3 spot)
    {
        if (castTimer > Time.time)
            return;

        if (recovery)
            return;

        base.Special(spot);

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

            castTime = Time.time + drawDelay;
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
