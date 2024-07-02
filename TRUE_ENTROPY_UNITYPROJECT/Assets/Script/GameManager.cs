using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
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
    [HideInInspector] public float OriginalVolume;
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
    [SerializeField] private float introDelay;

    public List<Conditions> conditions = new List<Conditions>();
    public List<Area> areas = new List<Area>();

    [SerializeField] AudioSource overrideAudio, startAudio;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] GameObject ghostPrefab;


    private string currentArea;
    private float currentVolume;
    private AudioSource currentAudioSource;

    private CameraZone currentCamZone;
    private CursorManager cursorManager;
    private GhostManager ghostManager;

    [HideInInspector] public DialogueManager DialogueManager;
    [HideInInspector] public ScreenEffects ScreenEffects;

    public LayerMask IgnoreLayers { get { return ignoreLayers; } }
    public bool VNMode { get { return vnMode; } }

    bool vnMode, commentMode, end, overrideAmbiance, intro, gettingUp, ready;
    float introTimer;


    PlayerController player;
    Vector3 startPos;
    Quaternion startRot;
    private List<Character> characters = new List<Character>();
    private int clicked;
    private float clicktime;

    private void Awake()
    {
        Instance = this;

        if (player == null)
            player = FindObjectOfType<PlayerController>();

        player.Init();

        ScreenEffects = GetComponent<ScreenEffects>();

        cursorManager = GetComponent<CursorManager>();
        cursorManager.Init();

        DialogueManager = FindObjectOfType<DialogueManager>();

        DialogueManager.Init();

        ghostManager = FindObjectOfType<GhostManager>();

        ghostManager.UpdateManager();


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

        foreach (var area in areas)
        {
            area.OriginalVolume = area.Music.volume;
        }

        startPos = player.transform.position;
        startRot = player.transform.rotation;

        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        ScreenEffects.FadeTo(1, 0.01f);
        AudioListener.volume = 0;

        yield return new WaitForSeconds(2.3f);

        EffectsManager.Instance.audioManager.Play("Gunshot");

        AudioListener.volume = 1;

        StartCoroutine(C_Restart());
    }

    IEnumerator C_Restart()
    {
        yield return new WaitForSeconds(3f);

        DialogueManager.TryEndDialogue();
        SetAmbianceVolume(1f);
        ScreenEffects.StartFade();
        intro = true;
        ready = true;
    }

    private void Update()
    {
        if (!ready || end)
        {
            cursorManager.SetCursorType(CursorType.Base);
            return;
        }

        if (intro)
        {
            cursorManager.SetCursorType(CursorType.Base);

            if (Input.GetMouseButtonDown(0) && !gettingUp)
            {
                gettingUp = true;
                introTimer = Time.time + introDelay;

                player.WakeUp();
                player.Injure(true);
            }

            if (introTimer < Time.time && gettingUp)
            {
                gettingUp = false;
                intro = false;

                player.Ready();
                WriteComment("Ugh. My head.");
            }

            return;
        }

        if (commentMode)
        {
            cursorManager.SetCursorType(CursorType.Base);
            DialogueManager.Step();
            return;
        }

        if (vnMode)
        {
            cursorManager.SetCursorType(CursorType.Base);
            DialogueManager.Step();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryClick();

            player.ToggleRun(HandleDoubleClick());
        }

        if (Input.GetMouseButtonDown(1))
        {
            player.ToggleSpecial(true);
        }

        if (Input.GetMouseButtonUp(1))
        {
            player.ToggleSpecial(false);
        }

        player.Step();

        foreach (Character c in characters)
        {
            c.Step();
        }

        CursorHover();
    }

    public void InjurePlayer(bool injure)
    {
        player.Injure(injure);
    }

    public void EndDemo()
    {
        end = true;

        StartCoroutine(C_EndTimer());
    }

    IEnumerator C_EndTimer()
    {
        ScreenEffects.FadeTo(1, 2.9f);

        yield return new WaitForSeconds(5);

        ScreenEffects.OumuamuaFade();

        yield return new WaitForSeconds(5);

        overrideAudio.Stop();

        WriteComment("THERE IS NO ESCAPE.");

        yield return new WaitForSeconds(4);

        Application.Quit();
    }


    public void EndGame(string message)
    {
        end = true;

        StartCoroutine(C_RestartTimer(message));
    }

    IEnumerator C_RestartTimer(string message)
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

        GameObject go = Instantiate(ghostPrefab);
        go.transform.position = player.transform.position;
        go.transform.rotation = player.transform.rotation;

        player.transform.position = startPos;
        player.transform.rotation = startRot;
        player.ResetState();
 
        end = false;

        StartCoroutine(C_Restart());
    }

    private bool HandleDoubleClick()
    {
        if (Time.time - clicktime > clickdelay)
            clicked = 0;

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
            if (player.SpecialMode)
            {
                player.Special();
            }
            else
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
        
    }

    private void CursorHover()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(wallLayer))
            {
                cursorManager.SetCursorType(CursorType.Base);
                return;
            }

            Interactable interactable = hit.transform.gameObject.GetComponent<Interactable>();

            if (interactable != null)
            {
                cursorManager.SetCursorType(CursorType.Look);

                return;
            }

            if (hit.transform.gameObject.layer == WhumpusUtilities.ToLayer(moveLayer))
            {
                cursorManager.SetCursorType(CursorType.Move);
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

        inventoryCanvas.SetActive(!yes);

        if (!yes)
            ghostManager.UpdateManager();
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
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                item.Music.volume = item.OriginalVolume;

                if (item.Name != currentArea)
                {
                    ghostManager.UpdateManager();

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

    public void NewArea(string areaName, float volume)
    {
        if (overrideAmbiance)
            return;

        foreach (var item in areas)
        {
            if (item.Name == areaName)
            {
                item.Music.volume = volume;

                if (item.Name != currentArea)
                {
                    ghostManager.UpdateManager();

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

    public void StopAmbiance()
    {
        foreach (var item in areas)
        {
            item.Music.Stop();
        }
    }

    public void PlayEndAmbiance()
    {
        overrideAmbiance = true;

        overrideAudio.Play();
    }

    public void SetCamZone(CameraZone zone)
    {
        currentCamZone = zone;
    }

    public void UpdateCondition(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                c.Met = true;
        }
    }

    public void FalseCondition(string condition)
    {
        foreach (var c in conditions)
        {
            if (c.Name == condition)
                c.Met = false;
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
        if (currentAudioSource != null)
            currentAudioSource.volume = currentVolume * sound;
    }
}
