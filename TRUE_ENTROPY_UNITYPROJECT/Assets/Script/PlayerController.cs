using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Character
{
    //[SerializeField] protected CopyPosRot copyPosRot;
    //[SerializeField] protected Rigidbody rb;
    [SerializeField] protected Vector3 resetPos;
    public LimbChopper[] Choppers;
    public GameObject[] Maskables;

    public override void Init()
    {
        base.Init();

        //rb.isKinematic = true;
    }

    public virtual void WakeUp()
    {
        animator.SetTrigger("WakeUp");
    }

    public virtual void Ready()
    {
        /*
        rb.transform.localPosition = new Vector3(resetPos.x, rb.transform.localPosition.y, resetPos.z);

        //copyPosRot.Step();
        copyPosRot.enabled = false;

        rb.isKinematic = false;
        */
       
        agent.enabled = true;
    }

    public void Freeze(bool frozen)
    {
        /*
        if (rb != null) 
            rb.isKinematic = frozen;
        */

        if (agent != null)
            agent.enabled = !frozen;

        /*
        if (frozen)
        {
            copyPosRot.enabled = true;
            copyPosRot.Step();
            copyPosRot.enabled = false;
        }
        */
    }

    public virtual void ResetState()
    {
        agent.enabled = false;
        animator.SetTrigger("Reset");

        /*
        rb.isKinematic = true;
        copyPosRot.enabled = true;  
        */
    }

    public void Hide(bool hidden)
    {
        foreach (var m in Maskables)
        {
            m.SetActive(!hidden);
        }
    }
}
