using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class JerController : PlayerController
{
    [SerializeField] Rig legIK;
    [SerializeField] GameObject hipGun, handGun;
    LayerMask ignoreLayers;
    bool Aiming, RigOn;

    public override void Init()
    {
        base.Init();

        ignoreLayers = GameManager.Instance.IgnoreLayers;

        legIK.weight = 0;

        hipGun.SetActive(true);
        handGun.SetActive(false);
    }

    public override void Special()
    {
        base.Special();

        animator.SetTrigger("Shoot");
    }

    public override void ToggleSpecial(bool active)
    {
        base.ToggleSpecial(active);

        Aiming = active;

        hipGun.SetActive(!active);
        handGun.SetActive(active);

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

        animator.SetBool("Aim", active);
        RigOn = active;
    }

    public override void Step()
    {
        if (Aiming)
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
            {
                Vector3 targPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                targetDir = Vector3.Normalize(targPos - transform.position);

                transform.forward = Vector3.Lerp(transform.forward, targetDir, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            base.Step();
        }

        if (RigOn)
            legIK.weight = Mathf.Lerp(legIK.weight, 1, Time.deltaTime * 4);
        else
            legIK.weight = Mathf.Lerp(legIK.weight, 0, Time.deltaTime * 4);
    }
}
