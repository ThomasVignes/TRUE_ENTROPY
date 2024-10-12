using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosRot : MonoBehaviour
{
    [SerializeField] private Transform original;
    [SerializeField] bool global;

    private void Update()
    {
        Step();
    }

    public void Step()
    {
        if (global)
        {
            transform.position = original.position;
            transform.rotation = original.rotation;
        }
        else
        {
            transform.localPosition = original.localPosition;
            transform.localRotation = original.localRotation;
        }
    }
}
