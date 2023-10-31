using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minAngleToMove;

    [Header("Animation")]
    [SerializeField] private float minDistanceToMove;
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;

    private bool rotating;
    private Vector3 targetPos;
    private Vector3 targetDir;
    private Vector3 lastAgentVelocity;
    private NavMeshPath lastAgentPath;

    private bool computing;

    public bool Moving { get { return agent.remainingDistance > minDistanceToMove; } }

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void Step()
    {
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

            Debug.Log(Vector3.Angle(transform.forward, targetDir));
        }
    }

    private void SetDirection()
    {
        targetDir = Vector3.Normalize(agent.path.corners[1] - transform.position);

        rotating = true;

        PausePath();
    }

    public void SetDestination(Vector3 pos)
    {
        //targetDir = Vector3.Normalize(pos - transform.position);

        targetPos = pos;

        agent.ResetPath();

        agent.SetDestination(targetPos);

        computing = true;
    }

    void PausePath()
    {
        lastAgentVelocity = agent.velocity;
        lastAgentPath = agent.path;
        agent.velocity = Vector3.zero;
        agent.ResetPath();
    }

    void ResumePath()
    {
        agent.velocity = lastAgentVelocity;
        agent.SetPath(lastAgentPath);

        
    }
}
