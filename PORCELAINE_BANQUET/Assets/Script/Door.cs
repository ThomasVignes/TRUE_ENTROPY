using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    [SerializeField] private string lockedMessage;
    [SerializeField] private Animator animator;

    public bool CanOpen;

    private bool isOpen;

    protected override void InteractEffects()
    {
        if (CanOpen)
        {
            if (!isOpen)
            {
                animator.SetBool("Open", true);

                isOpen = true;

                GetComponent<BoxCollider>().enabled = false;
                NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();

                if (obstacle != null)
                    obstacle.enabled = false;
            }
        }
        else
        {
            if (lockedMessage != "")
            {
                GameManager.Instance.WriteComment(lockedMessage);
            }
        }
    }
}
