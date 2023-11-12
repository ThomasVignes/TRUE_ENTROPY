using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whumpus;

[System.Serializable]
public class Conditions
{
    public string Name;
    public bool Met;
}

[System.Serializable]
public class Area
{
    public string Name;
    public AudioSource Music;
    public float Volume;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LayerMask moveLayer, interactLayer, wallLayer, ignoreLayers;
    [SerializeField] private GameObject currentCam, vnCam;
    [SerializeField] private CameraZone firstCamZone;
    [SerializeField] private Character character;
    [SerializeField] private float clickdelay;
    [SerializeField] private TextMeshProUGUI endText;

    public List<Conditions> conditions = new List<Conditions>();
    public List<Area> areas = new List<Area>();

    private string currentArea;
    private float currentVolume;
    private AudioSource currentAudioSource;

    private CameraZone currentCamZone;

    [HideInInspector] public DialogueManager DialogueManager;
    [HideInInspector] public ScreenEffects ScreenEffects;

    public bool VNMode { get { return vnMode; } }

    bool vnMode, commentMode, end;
    PlayerController player;
    private List<Character> characters = new List<Character>();
    private int clicked;
    private float clicktime;

    private void Awake()
    {
        Instance = this;

        player = FindObjectOfType<PlayerController>();

        player.Init();

        ScreenEffects = GetComponent<ScreenEffects>();

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

        endText.text = "";
    }

    private void Update()
    {
        if (end)
        {
            return;
        }

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
        {
            TryClick();

            player.ToggleRun(HandleDoubleClick());
        }

        player.Step();

        foreach (Character c in characters)
        {
            c.Step();
        }
    }

    public void EndGame(string message)
    {
        end = true;

        StartCoroutine(C_EndTimer(message));
    }

    IEnumerator C_EndTimer(string message)
    {
        SetAmbianceVolume(0f);
        ScreenEffects.FadeTo(1, 0.3f);

        yield return new WaitForSeconds(1.4f);

        endText.text = "";

        foreach (char c in message)
        {
            yield return new WaitForSeconds(0.06f);
            EffectsManager.Instance.audioManager.Play("Click");

            endText.text += c;
        }

        yield return new WaitForSeconds(2.3f);

        endText.text = "";
        EffectsManager.Instance.audioManager.Play("Gunshot");

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(0);
    }

    private bool HandleDoubleClick()
    {
        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            clicked = 0;
            clicktime = 0;
            return true;

        }
        else if (clicked > 2 || Time.time - clicktime > 1) clicked = 0;

        return false;
    }

    private void TryClick()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
            {
                return;
            }

            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                player.SetDestination(interactable.GetTargetPosition(), interactable);

                if (interactable is PickupInteractable)
                {
                    player.PickUpAnim();
                }

                return;
            }

            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer)) 
            {
                player.SetDestination(hit.point);
                return;
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
        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                if (item.Name != currentArea)
                {
                    item.Music.Play();
                    currentAudioSource = item.Music;
                    currentArea = item.Name;
                    currentVolume = currentAudioSource.volume;
                }
            }
            else
                item.Music.Stop();
        }
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

    public bool ConditionMet(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                return c.Met;
        }

        return false;
    }

    public void SetAmbianceVolume(float sound)
    {
        currentAudioSource.volume = currentVolume * sound;
    }
}
