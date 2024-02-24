using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [Header("Movement (Common)")]
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float minAngleToMove, minDistanceToInteract;
    [SerializeField] protected float runMultiplier, injuredMultiplier;

    [Header("Animation (Common)")]
    [SerializeField] protected float minDistanceToMove;
    [SerializeField] protected Animator animator;
    protected NavMeshAgent agent;

    protected bool rotating, running, willPickUp, injured;
    protected Vector3 targetPos;
    protected Vector3 targetDir;
    protected Vector3 lastAgentVelocity;
    protected NavMeshPath lastAgentPath;
    protected Interactable targetInteractable;
    protected float originalSpeed;

    protected bool computing, movingToInteractable, canInteract;

    public bool Moving { get { if (gameObject.activeInHierarchy && agent.enabled) { return agent.remainingDistance > minDistanceToMove; } else { return false; } } }


    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();

        originalSpeed = agent.speed;
    }

    public void Injure(bool injure)
    {
        injured = injure;
        animator.SetFloat("MoveState", 2);
    }

    public void ToggleRun(bool run)
    {
        if (injured)
        {
            running = false;
            animator.SetFloat("MoveState", 2);
            return;
        }

        if (run)
        {
            animator.SetFloat("MoveState", 1);
        }
        else
        {
            animator.SetFloat("MoveState", 0);
        }

        running = run;
    }

    public virtual void Step()
    {
        if (agent.velocity.magnitude > 0.15f && !canInteract && movingToInteractable)
        {
            canInteract = true;
        }

        if (movingToInteractable && agent.velocity.magnitude < 0.15f && canInteract)
        {
            float distance = Vector3.Distance(transform.position, targetPos);

            if (distance < minDistanceToInteract)
            {
                if (targetInteractable != null)
                {
                    canInteract = false;
                    movingToInteractable = false;
                    targetInteractable.Interact();
                    targetInteractable = null;

                    if (willPickUp)
                    {
                        willPickUp = false;
                        animator.SetTrigger("PickUp");
                    }
                }
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

        if (running && agent.speed != originalSpeed * runMultiplier)
            agent.speed = originalSpeed * runMultiplier;

        if (!running && agent.speed != originalSpeed)
            agent.speed = originalSpeed;

        if (injured && agent.speed != originalSpeed * injuredMultiplier)
            agent.speed = originalSpeed * injuredMultiplier;

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
        willPickUp = false;

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
        //agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

    protected virtual void ResumePath()
    {
        //agent.velocity = lastAgentVelocity;
        agent.SetPath(lastAgentPath);

        if (targetInteractable != null)
        {
            movingToInteractable = true;
        }
    }

    public void PickUpAnim()
    {
        willPickUp = true;
    }
}
