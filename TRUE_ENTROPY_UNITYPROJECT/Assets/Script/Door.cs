using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Door : Interactable
{
    public bool StartOpen;

    [SerializeField] private string lockedMessage;
    [SerializeField] private Animator animator;

    public UnityEvent OnOpen, OnUnlock;

    [SerializeField] bool reversedOpen;

    public bool CanOpen;

    private bool isOpen;

    private void Start()
    {
        ToggleDoor(StartOpen);
    }

    protected override void InteractEffects()
    {
        if (CanOpen)
        {
            if (!isOpen)
                ToggleDoor(true);
        }
        else
        {
            if (lockedMessage != "")
                GameManager.Instance.WriteComment(lockedMessage);
        }
    }

    public void ToggleDoor(bool open)
    {
        animator.SetBool("Open", open);
        animator.SetBool("Reversed", reversedOpen);

        isOpen = open;

        GetComponent<BoxCollider>().enabled = !open;
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null)
            obstacle.enabled = !open;

        if (open)
            OnOpen?.Invoke(); 
    }

    public void ToggleDoorNoEvent(bool open)
    {
        animator.SetBool("Open", open);

        isOpen = open;

        GetComponent<BoxCollider>().enabled = !open;
        NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

        if (obstacle != null)
            obstacle.enabled = !open;
    }

    public void Unlock()
    {
        CanOpen = true;

        OnUnlock?.Invoke();
    }

    public void ChangeLockMessage(string message)
    {
        lockedMessage = message;
    }
}
