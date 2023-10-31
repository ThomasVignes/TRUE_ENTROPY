using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LayerMask moveLayer, interactLayer;

    PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        player.Init();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, moveLayer))
            {
                player.SetDestination(hit.point);
            }
        }

        player.Step();

    }
}
