using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Whumpus;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

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

    [Header("Chapter Data")]
    public ChapterData ChapterData;
    public string StartCinematic;

    [Header("Clicking")]
    [SerializeField] private float clickdelay;
    [SerializeField] private LayerMask moveLayer, interactLayer, wallLayer, ignoreLayers;

    [Header("Cameras")]
    [SerializeField] private GameObject currentCam;
    [SerializeField] private GameObject vnCam;
    [SerializeField] private CameraZone firstCamZone;
    private Character character;

    [Header("References")]
    [SerializeField] ChapterManagerGeneric startGameManager;
    [SerializeField] private Transform characterStart;
    [SerializeField] AudioSource overrideAudio, startAudio;
    [SerializeField] GameObject inventoryCanvas;


    public List<Conditions> conditions = new List<Conditions>();
    public List<Area> areas = new List<Area>();

    PlayerController player;
    Vector3 startPos;
    Quaternion startRot;
    private List<Character> characters = new List<Character>();
    private int clicked;
    private float clicktime;


    private string currentArea;
    private float currentVolume;
    private AudioSource currentAudioSource;

    private CameraZone currentCamZone;
    private CursorManager cursorManager;
    private GhostManager ghostManager;

    [HideInInspector] public CinematicManager CinematicManager;
    [HideInInspector] public DialogueManager DialogueManager;
    [HideInInspector] public ScreenEffects ScreenEffects;
    [HideInInspector] public HitstopManager HitstopManager;
    [HideInInspector] public InventoryManager InventoryManager;
    [HideInInspector] public CameraEffectManager CameraEffectManager;

    public LayerMask IgnoreLayers { get { return ignoreLayers; } }
    public bool CinematicMode { get { return cinematicMode; } }
    public bool VNMode { get { return vnMode; } }
    public bool Ready { get { return ready; } set { ready = value; } }
    public bool End { get { return end; } set { end = value; } }
    public PlayerController Player { get { return player; } }
    public CursorManager CursorManager { get { return cursorManager; } }
    public AudioSource OverrideAudio { get { return overrideAudio; } }


    bool cinematicMode, vnMode, commentMode, end, overrideAmbiance, ready;
    bool cinematicStart;


    private void Awake()
    {
        Instance = this;
        /*
        if (player == null)
            player = FindObjectOfType<PlayerController>();
        */

        GameObject chara = Instantiate(ChapterData.StartCharacter.ControllerPrefab, characterStart.position, characterStart.rotation);
        character = chara.GetComponentInChildren<Character>();
        player = chara.GetComponentInChildren<PlayerController>();

        player.Init();


        ScreenEffects = GetComponent<ScreenEffects>();

        cursorManager = GetComponent<CursorManager>();
        cursorManager.Init();

        DialogueManager = FindObjectOfType<DialogueManager>();
        DialogueManager.Init(this);

        CinematicManager = FindObjectOfType<CinematicManager>();
        CinematicManager.Init(this);

        ghostManager = FindObjectOfType<GhostManager>();

        ghostManager.UpdateGhosts();

        HitstopManager = FindObjectOfType<HitstopManager>();

        InventoryManager = FindObjectOfType<InventoryManager>();

        CameraEffectManager = GetComponent<CameraEffectManager>();

        CameraEffectManager.Init(this);

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


        conditions = ChapterData.conditions;
        //areas = ChapterData.areas;
        InventoryManager.Init(ChapterData.items);


        foreach (var area in areas)
        {
            area.OriginalVolume = area.Music.volume;
        }

        if (StartCinematic != "")
        {
            CinematicManager.PlayCinematic(StartCinematic);

            cinematicStart = true;
        }
        else
        {
            startGameManager.Init(this);

            startPos = player.transform.position;
            startRot = player.transform.rotation;

            startGameManager.StartGame();
        }
    }

    public void PlayerReady()
    { 
        player.Ready();
    }

    public void PausePlayerPath()
    {
        player.Pause();
    }

    private void Update()
    {
        if (cinematicMode)
        {
            CinematicManager.Step();
            cursorManager.SetCursorType(CursorType.Base);
            return;
        }

        if (cinematicStart)
        {
            startGameManager.Init(this);

            startPos = player.transform.position;
            startRot = player.transform.rotation;

            startGameManager.StartGame();

            cinematicStart = false;
        }

        if (!ready || end)
        {
            cursorManager.SetCursorType(CursorType.Base);
            return;
        }

        player.ConstantStep();

        foreach (Character c in characters)
        {
            c.ConstantStep();
        }

        if (startGameManager.Intro)
        {
            cursorManager.SetCursorType(CursorType.Base);

            startGameManager.IntroStep();

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


    public void EndChapter()
    {
        end = true;

        startGameManager.EndChapter();
    }



    public void EndGame(string message)
    {
        startGameManager.Death(message);
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
                player.Special(hit.point);
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
            if (player.SpecialMode)
            {
                cursorManager.SetCursorType(player.CursorType);
                return;
            }

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

        if (!cinematicMode)
            inventoryCanvas.SetActive(!yes);

        if (!yes)
            ghostManager.UpdateGhosts();
    }

    public void SetCinematicMode(bool yes)
    {
        cinematicMode = yes;

        currentCamZone.active = !yes;

        if (!vnMode)
            inventoryCanvas.SetActive(!yes);

        if (!yes)
            ghostManager.UpdateGhosts();
    }

    CommentInteractable currentComment;

    public void WriteComment(string text)
    {
        if (commentMode)
            return;

        commentMode = true;
        DialogueManager.WriteSpecific(text);
    }

    public void WriteComment(string text, CommentInteractable comment)
    {
        if (commentMode)
            return;

        currentComment = comment;

        WriteComment(text);
    }

    public void EndComment()
    {
        if (!commentMode)
            return;

        if (currentComment != null)
        {
            currentComment.OnCommentEnd?.Invoke();
            currentComment = null;
        }

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
                    ghostManager.UpdateGhosts();

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
                    ghostManager.UpdateGhosts();

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

    public void CamZoneQuickUpdate(CameraZone zone)
    {
        currentCamZone = zone;
    }

    public void SetCamZone(CameraZone zone)
    {
        //Update zone specific objects
        List<GameObject> previous = currentCamZone.ShotSpecificObjects;
        List<GameObject> next = zone.ShotSpecificObjects;
        foreach (var item in previous)
        {
            if (!next.Contains(item) && item.activeSelf)
                item.SetActive(false);
        }

        foreach (var item in next)
        {
            if (!item.activeSelf)
                item.SetActive(true);
        }

        //Update zone hidden objects
        previous = currentCamZone.ShotSpecificHide;
        next = zone.ShotSpecificHide;
        foreach (var item in previous)
        {
            if (!next.Contains(item) && !item.activeSelf)
                item.SetActive(true);
        }

        foreach(var item in next)
        {
            if (item.activeSelf)
                item.SetActive(false);
        }


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

    public void ResetPlayer()
    {
        player.transform.position = startPos;
        player.transform.rotation = startRot;
        player.ResetState();
    }

    public void TeleportPlayer(Vector3 pos, Quaternion rot)
    {
        player.Freeze(true);
        player.transform.position = pos;
        player.transform.rotation = rot;
        player.Freeze(false);
    }

    public void TeleportPlayer(Transform transform)
    {
        TeleportPlayer(transform.position, transform.rotation);
    }

    public void ChopLimb(string ID)
    {
        foreach (var item in player.Choppers)
        {
            if (item.ID == ID)
                item.Chop();
        }
    }

    public void PutMask(bool on)
    {
        MaskManager maskManager = player.transform.GetComponent<MaskManager>();

        if (maskManager == null)
            player.transform.GetComponentInChildren<MaskManager>();

        if (maskManager != null)
            maskManager.PutMask(on);
    }
}
