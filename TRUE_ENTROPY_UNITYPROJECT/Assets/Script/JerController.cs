using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class JerController : PlayerController
{
    [SerializeField] float shootCD, drawDelay;
    [SerializeField] Rig legIK;
    [SerializeField] GameObject hipGun, handGun, muzzle;
    [SerializeField] GameObject shootParticle, bloodParticle, muzzleParticle, bloodDecal;
    LayerMask ignoreLayers;
    bool Aiming, RigOn;
    float shootTimer;
    Lifeform lifeform;

    public override void Init()
    {
        base.Init();

        ignoreLayers = GameManager.Instance.IgnoreLayers;

        legIK.weight = 0;

        hipGun.SetActive(true);
        handGun.SetActive(false);

        lifeform = GetComponent<Lifeform>();
    }

    public override void Special(Vector3 spot, GameObject hitObject)
    {
        if (stunned)
            return;

        if (shootTimer > Time.time)
            return;

        shootTimer = Time.time + shootCD;

        base.Special(spot, hitObject);

        var shotLifeform = false;
        var shielded = false;

        TargetLimb targetLimb = hitObject.GetComponent<TargetLimb>();

        if (targetLimb != null && targetLimb.Owner != lifeform)
        {
            targetLimb.Hit(1, 1, 2500, transform.forward.normalized);

            if (!targetLimb.Shielded)
            {
                GameObject blood = Instantiate(bloodDecal, hitObject.transform);
                blood.transform.position = spot;
                blood.transform.forward = transform.forward;
            }

            shielded = targetLimb.Shielded;
            shotLifeform = true;
        }

        animator.SetTrigger("Shoot");

        EffectsManager.Instance.audioManager.Play("JerGun");
        GameManager.Instance.HitstopManager.StartHitstop();

        GameObject go = Instantiate(muzzleParticle, muzzle.transform.position, muzzle.transform.rotation);

        if (shielded)
        {
            go = Instantiate(shootParticle, spot, Quaternion.identity);
            EffectsManager.Instance.audioManager.Play("Shield");
        }
        else
        {
            if (shotLifeform)
            {
                go = Instantiate(bloodParticle, spot, Quaternion.identity);
                go.transform.LookAt(transform.position + Vector3.up * 2f);

                EffectsManager.Instance.audioManager.Play("Blood");
            }
            else
            {
                go = Instantiate(shootParticle, spot, Quaternion.identity);
                EffectsManager.Instance.audioManager.Play("Surface");
            }
        }
    }

    public override void ToggleSpecial(bool active)
    {
        base.ToggleSpecial(active);

        Aiming = active;

        hipGun.SetActive(!active);
        handGun.SetActive(active);

        if (Aiming)
        {
            shootTimer = Time.time + drawDelay;
        }

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
        if (Aiming && !stunned)
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

        if (RigOn && !stunned)
            legIK.weight = Mathf.Lerp(legIK.weight, 1, Time.deltaTime * 4);
        else
            legIK.weight = Mathf.Lerp(legIK.weight, 0, Time.deltaTime * 4);
    }

    public override void Stun(float duration)
    {
        base.Stun(duration);

        animator.SetTrigger("Punched");
        GameManager.Instance.HitstopManager.StartHitstop();
        GameManager.Instance.CameraEffectManager.PlayEffect(CameraEffect.PlayerHit);
    }
}
