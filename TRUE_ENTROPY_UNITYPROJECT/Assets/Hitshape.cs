using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;

public class Hitshape : MonoBehaviour
{
    [SerializeField] Lifeform owner;
    [SerializeField] LayerMask hitLayer;
    [SerializeField] int damage;
    [SerializeField] int stun;
    [SerializeField] float force;
    [SerializeField] float linger;

    bool active;
    float lingerTimer;
    Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        Disable();
    }

    private void Update()
    {
        if (active && lingerTimer < Time.time)
            Disable();
    }

    public void Trigger()
    {
        active = true;
        col.enabled = true;

        lingerTimer = Time.time + linger;
    }

    public void Disable()
    {
        col.enabled = false;
        active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(hitLayer))
        {
            TargetLimb targetLimb = other.gameObject.GetComponent<TargetLimb>();

            if (targetLimb != null && targetLimb.Lifeform != owner)
            {
                targetLimb.Hit(damage, stun, force, owner.transform.forward.normalized);
                Disable();
            }
        }
    }
}
