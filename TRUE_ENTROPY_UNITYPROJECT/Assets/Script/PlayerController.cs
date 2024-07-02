using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Character
{
    [SerializeField] protected CopyPosRot copyPosRot;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Vector3 resetPos;


    public override void Init()
    {
        base.Init();

        rb.isKinematic = true;
    }

    public virtual void WakeUp()
    {
        animator.SetTrigger("WakeUp");
    }

    public virtual void Ready()
    {
        rb.transform.localPosition = new Vector3(resetPos.x, rb.transform.localPosition.y, resetPos.z);
        rb.isKinematic = false;
        copyPosRot.enabled = false;
        agent.enabled = true;
    }

    public void ResetState()
    {
        agent.enabled = false;
        animator.SetTrigger("Reset");
        rb.isKinematic = true;
        copyPosRot.enabled = true;  
    }
}
