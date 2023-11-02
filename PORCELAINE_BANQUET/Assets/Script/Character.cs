using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [Header("Movement (Common)")]
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float minAngleToMove;

    [Header("Animation (Common)")]
    [SerializeField] protected float minDistanceToMove;
    [SerializeField] protected Animator animator;
    protected NavMeshAgent agent;

    protected bool rotating;
    protected Vector3 targetPos;
    protected Vector3 targetDir;
    protected Vector3 lastAgentVelocity;
    protected NavMeshPath lastAgentPath;
    protected Interactable targetInteractable;

    protected bool computing, movingToInteractable, canInteract;

    public bool Moving { get { return agent.remainingDistance > minDistanceToMove; } }

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void Step()
    {
        if (agent.velocity.magnitude > 0.15f && !canInteract && movingToInteractable)
        {
            canInteract = true;
        }

        if (movingToInteractable && agent.velocity.magnitude < 0.15f && canInteract)
        {
            if (targetInteractable != null)
            {
                canInteract = false;
                movingToInteractable = false;
                targetInteractable.Interact();
                targetInteractable = null;
            }
        }

        animator.SetBool("Walking", Moving && !rotating);

        if (computing)
        {
            if (!agent.pathPending)
            {
                computing = false;

                SetDirection();
            }
            else
            {
                return;
            }
        }

        if (Moving)
        {
            targetDir.y = transform.position.y;
        }

        if (rotating)
        {
            if (Vector3.Angle(transform.forward, targetDir) > minAngleToMove)
            {
                transform.forward = Vector3.Lerp(transform.forward, targetDir, rotationSpeed * Time.deltaTime);
            }
            else
            {
                rotating = false;
                ResumePath();
            }
        }
    }

    protected void SetDirection()
    {
        targetDir = Vector3.Normalize(agent.path.corners[1] - transform.position);

        rotating = true;

        PausePath();
    }

    public void SetDestination(Vector3 pos)
    {
        targetInteractable = null;
        movingToInteractable = false;
        canInteract = false;

        targetPos = pos;

        agent.ResetPath();

        agent.SetDestination(targetPos);

        computing = true;
    }

    public void SetDestination(Transform pos)
    {
        targetInteractable = null;
        movingToInteractable = false;
        canInteract = false;

        targetPos = pos.position;

        agent.ResetPath();

        agent.SetDestination(targetPos);

        computing = true;
    }

    public void SetDestination(Vector3 pos, Interactable interactable)
    {
        targetInteractable = interactable;

        targetPos = pos;

        agent.ResetPath();

        agent.SetDestination(targetPos);

        computing = true;
    }

    protected virtual void PausePath()
    {
        lastAgentVelocity = agent.velocity;
        lastAgentPath = agent.path;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

    protected virtual void ResumePath()
    {
        agent.velocity = lastAgentVelocity;
        agent.SetPath(lastAgentPath);

        if (targetInteractable != null)
        {
            movingToInteractable = true;
        }
    }
}
