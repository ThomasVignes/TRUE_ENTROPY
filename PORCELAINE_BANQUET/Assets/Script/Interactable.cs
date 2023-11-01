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

    private bool done;

    public virtual void Interact()
    {
        if (!done)
        {
            OnInteract?.Invoke();

            if (!Repeatable)
                done = true;
        }
    }

    public Vector3 GetTargetPosition()
    {
        if (interactionSpot != null)
            return interactionSpot.position;
        else
            return transform.position;
    }
}
