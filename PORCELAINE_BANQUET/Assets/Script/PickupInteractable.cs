using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractable : Interactable
{
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject[] instances;

    protected override void InteractEffects()
    {
        mesh.SetActive(false);

        foreach (var item in instances)
        {
            item.SetActive(false);
        }
    }
}
