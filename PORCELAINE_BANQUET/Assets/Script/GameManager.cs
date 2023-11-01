using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask moveLayer, interactLayer;
    [SerializeField] private GameObject currentCam, vnCam;

    [HideInInspector] public DialogueManager DialogueManager;

    public bool VNMode { get { return vnMode; } }

    bool vnMode;
    PlayerController player;


    private void Awake()
    {
        Instance = this;

        player = FindObjectOfType<PlayerController>();

        player.Init();

        DialogueManager = FindObjectOfType<DialogueManager>();

        DialogueManager.Init();
    }

    private void Update()
    {
        if (vnMode)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                SetVNMode(false);
                DialogueManager.TryEndDialogue();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
            TryClick();

        player.Step();
    }

    private void TryClick()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactLayer))
        {
            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                player.SetDestination(interactable.GetTargetPosition(), interactable);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, moveLayer))
            {
                player.SetDestination(hit.point);
            }
        }
    }

    public void SetVNMode(bool yes)
    {
        vnMode = yes;

        currentCam.SetActive(!yes);
        vnCam.SetActive(yes);
    }
}
