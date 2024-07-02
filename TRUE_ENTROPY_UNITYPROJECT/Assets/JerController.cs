using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class JerController : PlayerController
{
    LayerMask ignoreLayers;
    bool Aiming;

    public override void Init()
    {
        base.Init();

        ignoreLayers = GameManager.Instance.IgnoreLayers;
    }

    public override void Special(bool active)
    {
        Aiming = active;

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
    }
}
