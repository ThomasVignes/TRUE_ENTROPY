using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagdollHider : MonoBehaviour
{
    [SerializeField] bool hideOnStart;
    [SerializeField] Vector3 hideOffset;
    [SerializeField] GameObject[] deactivateObjects;
    [SerializeField] Rigidbody rb;

    Vector3 originalPos;
    NavMeshAgent agent;

    private void Awake()
    {
        originalPos = transform.position;

        agent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Start()
    {
        if (hideOnStart)
            Hide();
    }

    public void Hide()
    {
        if (agent != null)
            agent.enabled = false;

        if (rb != null)
            rb.isKinematic = true;

        transform.position = originalPos + hideOffset;

        foreach (var item in deactivateObjects)
        {
            if (item != null)
                item.SetActive(false);
        }
    }

    [ContextMenu("Show")]
    public void Show()
    {
        transform.position = originalPos;

        foreach (var item in deactivateObjects)
        {
            if (item != null)
                item.SetActive(true);
        }

        if (agent != null)
            agent.enabled = true;

        if (rb != null)
            rb.isKinematic = false;
    }
}
