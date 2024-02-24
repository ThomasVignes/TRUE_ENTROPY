using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : Character
{
    [SerializeField] CopyPosRot copyPosRot;
    [SerializeField] Rigidbody rb;
    [SerializeField] Vector3 resetPos;


    public override void Init()
    {
        base.Init();

        rb.isKinematic = true;
    }

    public void WakeUp()
    {
        animator.SetTrigger("WakeUp");
    }

    public void Ready()
    {
        rb.transform.localPosition = new Vector3(resetPos.x, rb.transform.localPosition.y, resetPos.z);
        rb.isKinematic = false;
        Destroy(copyPosRot);
    }
}
