using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using Whumpus;

[System.Serializable]
public class Conditions
{
    public string Name;
    public bool Met;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask moveLayer, interactLayer, wallLayer;
    [SerializeField] private GameObject currentCam, vnCam;
    [SerializeField] private CameraZone firstCamZone;

    public List<Conditions> conditions = new List<Conditions>();

    private CameraZone currentCamZone;

    [HideInInspector] public DialogueManager DialogueManager;

    public bool VNMode { get { return vnMode; } }

    bool vnMode, commentMode;
    PlayerController player;
    private List<Character> characters = new List<Character>();

    private void Awake()
    {
        Instance = this;

        player = FindObjectOfType<PlayerController>();

        player.Init();

        DialogueManager = FindObjectOfType<DialogueManager>();

        DialogueManager.Init();

        Character[] chars = FindObjectsOfType<Character>();

        foreach (Character c in chars)
        {
            if (!(c is PlayerController))
            {
                c.Init();
                characters.Add(c);
            }
        }

        currentCamZone = firstCamZone;
    }

    private void Update()
    {
        if (commentMode)
        {
            DialogueManager.Step();
            return;
        }

        if (vnMode)
        {
            DialogueManager.Step();
            return;
        }

        if (Input.GetMouseButtonDown(0))
            TryClick();

        player.Step();

        foreach (Character c in characters)
        {
            c.Step();
        }
    }

    private void TryClick()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
                return;
        }


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

        //currentCam.SetActive(!yes);
        vnCam.SetActive(yes);

        currentCamZone.active = !yes;
    }

    public void WriteComment(string text)
    {
        if (commentMode)
            return;

        commentMode = true;
        DialogueManager.WriteSpecific(text);
    }

    public void EndComment()
    {
        if (!commentMode)
            return;

        commentMode = false;
    }

    public void NewArea(string areaName)
    {

    }

    public void SetCamZone(CameraZone zone)
    {
        currentCamZone = zone;
    }

    public void UpdateCondition(string condition, bool state)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                c.Met = state;
        }
    }

    public bool GetCondition(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                return c.Met;
        }

        return false;
    }
}