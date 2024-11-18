using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using static Cinemachine.CinemachineOrbitalTransposer;

public class RuthController : PlayerController
{
    [Header("Ruth Settings")]
    [SerializeField] float drawDelay;
    [SerializeField] float castTime;
    [SerializeField] bool canMoveDuringCast;
    [SerializeField] float delayBeforeArmCross;
    [SerializeField] ParticleSystem snapFx;
    [SerializeField] Rig rig;
    [SerializeField] Transform aimTarg;

    LayerMask ignoreLayers;

    bool casting, recovery;
    float castTimer;
    float crossTimer;
    bool crossed;

    public override void Init()
    {
        base.Init();

        ignoreLayers = GameManager.Instance.IgnoreLayers;

    }

    public override void Step()
    {
        base.Step();

        if (Mathf.Abs(agent.velocity.magnitude) <= 0)
            crossTimer += Time.deltaTime;
        else
            crossTimer = 0;

        if (Input.GetKeyDown(KeyCode.G))
            animator.SetTrigger("Clap");

        animator.SetBool("ArmsCrossed", crossTimer > delayBeforeArmCross);

        if (casting)
        {
            rig.weight = Mathf.Lerp(rig.weight, 1, 4f * Time.deltaTime);

            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
            {
                Vector3 targPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                //aimTarg.position = targPos;
                Vector3 dir = targPos - aimTarg.position;

                aimTarg.position += dir.normalized * 10f * Time.deltaTime;

                /*
                Vector3 dir = aimTarg.position - transform.position;

                var dot = Vector3.Dot(dir, transform.forward);

                if (dot > 0)
                    aimTarg.position = targPos;
                */
            }
        }
        else
            rig.weight = Mathf.Lerp(rig.weight, 0, 4f * Time.deltaTime);


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
        snapFx.Play();

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
