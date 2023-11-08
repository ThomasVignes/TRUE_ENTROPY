using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteractable : Interactable
{
    [SerializeField] private GameObject mesh;

    protected override void InteractEffects()
    {
        mesh.SetActive(false);
    }
}
