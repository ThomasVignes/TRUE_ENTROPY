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

    public void Init()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void _Update()
    {
        animator.SetBool("Walking", agent.remainingDistance > minDistanceToMove && !rotating);

        if (rotating)
        {
            if (Vector3.Angle(transform.forward, targetDir) > minAngleToMove)
            {
                transform.forward = Vector3.Lerp(transform.forward, targetDir, rotationSpeed * Time.deltaTime);
            }
            else
            {
                rotating = false;
                agent.SetDestination(targetPos);
            }

            Debug.Log(Vector3.Angle(transform.forward, targetDir));
        }
    }

    public void SetDestination(Vector3 pos)
    {
        targetDir = Vector3.Normalize(pos - transform.position);
        targetDir.y = transform.position.y;

        targetPos = pos;

        if (Vector3.Angle(transform.forward, targetDir) > minAngleToMove)
        {
            rotating = true;
            agent.ResetPath();
        }
        else
        {
            agent.SetDestination(targetPos);
        }
    }
}
