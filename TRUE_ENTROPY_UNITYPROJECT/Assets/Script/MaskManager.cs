using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] GameObject mask;
    [SerializeField] GameObject[] toRemove;
    [SerializeField] GameObject[] toAdd;

    public void PutMask(bool on)
    {
        mask.SetActive(on);

        foreach (GameObject o in toRemove) 
        {
            o.SetActive(!on);
        }

        foreach (GameObject o in toAdd)
        {
            o.SetActive(o);
        }
    }
}
