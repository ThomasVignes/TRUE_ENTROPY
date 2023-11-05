using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Interactable Settings (Common)")]
    public UnityEvent OnInteract;
    public bool Repeatable;

    [SerializeField] private Transform interactionSpot;

    protected bool done;

    public void Interact()
    {
        if (!done)
        {
            OnInteract?.Invoke();
            InteractEffects();

            if (!Repeatable)
                done = true;
        }
    }

    protected virtual void InteractEffects()
    {

    }

    public Vector3 GetTargetPosition()
    {
        if (interactionSpot != null)
            return interactionSpot.position;
        else
            return transform.position;
    }
}
