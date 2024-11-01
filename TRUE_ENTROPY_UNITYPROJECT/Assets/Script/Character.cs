using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    [Header("Special (Common)")]
    [SerializeField] protected CursorType cursorType;

    [Header("Cinematic (Common)")]
    public bool CanMoveInCinematic;

    [Header("Movement (Common)")]
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float minAngleToMove, minDistanceToInteract, closeInteractRange;
    [SerializeField] protected float runMultiplier, injuredMultiplier;

    [Header("Animation (Common)")]
    [SerializeField] protected float moveStateLerp;
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
    protected bool specialMode;

    protected int targetMoveState;
    protected float currentMoveState;

    protected float stunnedTimer;
    protected bool stunned;

    public NavMeshAgent Agent { get { return agent; } } 
    public bool SpecialMode { get { return specialMode; } }
    public bool Moving { get { if (gameObject.activeInHierarchy && agent.enabled) { return agent.remainingDistance > minDistanceToMove; } else { return false; } } }
    public bool Running { get { return running; } }
    public CursorType CursorType { get { return cursorType; } }


    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();

        originalSpeed = agent.speed;
    }

    public void Injure(bool injure)
    {
        if (injure == injured)
            return;

        injured = injure;

        targetMoveState = 2;
    }

    public void ToggleRun(bool run)
    {
        if (SpecialMode)
            return;

        if (injured)
        {
            running = false;
            targetMoveState = 2;
            return;
        }

        if (run)
        {
            targetMoveState = 1;
        }
        else
        {
            targetMoveState = 0;
        }

        running = run;
    }

    public virtual void Step()
    {
        if (stunned)
        {
            if (stunnedTimer < Time.time)
                stunned = false;
            else
                return;
        }

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

    public virtual void ConstantStep()
    {
        currentMoveState = Mathf.Lerp(currentMoveState, targetMoveState, moveStateLerp * Time.deltaTime);

        animator.SetFloat("MoveState", currentMoveState);
        animator.SetBool("Walking", Moving && !rotating);
    }

    public virtual void ToggleSpecial(bool active)
    {
        specialMode = active;
    }

    public virtual void Special(Vector3 spot, GameObject hitObject)
    {
        if (!specialMode)
            return;
    }

    protected void SetDirection()
    {
        Vector3 dirPos = transform.position + transform.forward.normalized;

        if (agent.path.corners.Length > 1)
        {
            dirPos = agent.path.corners[1];
        }
        else if (agent.path.corners.Length == 1)
            dirPos = agent.path.corners[0]; 

        targetDir = Vector3.Normalize(dirPos - transform.position);

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

        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance <= closeInteractRange)
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
        else
        {
            agent.SetDestination(targetPos);
            computing = true;
        }
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

    public virtual void Stun(float duration)
    {
        PausePath();

        stunnedTimer = Time.time + duration;
        stunned = true;
    }

    public void Pause()
    {
        PausePath();
    }

    public void PickUpAnim()
    {
        willPickUp = true;
    }
}
