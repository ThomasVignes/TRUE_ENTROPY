using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Whumpus;

public class TargetLimb : MonoBehaviour
{
    public Lifeform Owner;
    public int Multiplier = 1;
    public float StunMultiplier = 1;
    public float ForceMultiplier = 1;
    public RagdollLimb limb;
    public UnityEvent OnHit;

    LimbShield shield;

    public bool Shielded { get { if (shield == null) return false; else return shield.Active; } }

    private void Awake()
    {
        shield = GetComponent<LimbShield>();
    }

    public void Hit(int damage, float stun, float force, Vector3 dir)
    {
        Debug.Log(limb.gameObject.name + " hit");
        

        if (shield == null || !shield.Active)
        {
            Owner.Hurt(damage * Multiplier);
            Owner.Stun(stun * StunMultiplier);
            limb.rb.AddForce(force * dir);
        }
        else
        {
            shield.Absorb(damage * Multiplier);
        }

        OnHit.Invoke();
    }
}
