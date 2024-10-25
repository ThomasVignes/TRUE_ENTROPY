using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendrilBatch : MonoBehaviour
{
    [SerializeField] Florp[] tendrils;

    public void Trigger()
    {
        StartCoroutine(C_TendrilBatch());
    }

    IEnumerator C_TendrilBatch()
    {
        foreach (var t in tendrils) 
        { 
            t.Trigger();

            yield return new WaitForSeconds(0.1f);
        }
    }
}
