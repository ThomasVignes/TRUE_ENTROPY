using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public bool Active;
    [SerializeField] float spotlightLerp, ughDist;
    [SerializeField] LayerMask spotlightLayers;
    [SerializeField] Transform vcam, spotlight, head;
    [SerializeField] Animator tempAnimator;

    Quaternion originalRotation, lookRotation;

    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (!Active)
            return;

        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, spotlightLayers))
        {
            lookRotation = Quaternion.LookRotation(hit.point - spotlight.position);
        }
        else
        {
            lookRotation = originalRotation;
        }

        spotlight.rotation = Quaternion.Lerp(spotlight.rotation, lookRotation, spotlightLerp * Time.deltaTime);

        tempAnimator.SetBool("Ugh", Vector3.Distance(new Vector3(head.position.x, head.position.y, hit.point.z), hit.point) < ughDist);
    }
}
