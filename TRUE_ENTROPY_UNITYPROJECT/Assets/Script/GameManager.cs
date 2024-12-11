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

[System.Serializable]
public class Conditions
{
    public string Name;
    public bool Met;

    public Conditions(string Name,  bool Met)
    {
        this.Name = Name;
        this.Met = Met;
    }
}

[System.Serializable]
public class Area
{
    public string Name;
    public AudioSource Music;
    public AudioSource CopyrightFree;
    public bool ImmuneExperimental;
    [HideInInspector] public float OriginalVolume;

    public void Init()
    {
        if (PersistentData.Instance != null && PersistentData.Instance.CopyrightFree)
        {
            Music = CopyrightFree;
            ImmuneExperimental = true;
        }

        OriginalVolume = Music.volume;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Chapter Data")]
    public ChapterData ChapterData;
    public string StartCinematic;

    [Header("Scene Settings")]
    public bool Paused;
    public bool SpecialActive;

    [Header("Clicking")]
    [SerializeField] private float clickdelay;
    [SerializeField] private LayerMask moveLayer, interactLayer, wallLayer, ignoreLayers, hitboxLayer;

    [Header("Cameras")]
    [SerializeField] private GameObject currentCam;
    [SerializeField] private GameObject vnCam;
    [SerializeField] private CameraZone firstCamZone;
    private Character character;

    [Header("References")]
    [SerializeField] ChapterManagerGeneric startGameManager;
    [SerializeField] private Transform characterStart;
    [SerializeField] GameObject inventoryCanvas;
    [SerializeField] ThemeManager themeManager;
    [SerializeField] Manager[] genericManagers;
    public PauseManager PauseManager;


    public List<Conditions> conditions = new List<Conditions>();

    PlayerController player;
    Vector3 startPos;
    Quaternion startRot;
    private List<Character> characters = new List<Character>();
    private int clicked;
    private float clicktime;
    private bool specialActive;


    private CameraZone currentCamZone;
    private CursorManager cursorManager;
    private GhostManager ghostManager;

    [HideInInspector] public CinematicManager CinematicManager;
    [HideInInspector] public DialogueManager DialogueManager;
    [HideInInspector] public ScreenEffects ScreenEffects;
    [HideInInspector] public HitstopManager HitstopManager;
    [HideInInspector] public InventoryManager InventoryManager;
    [HideInInspector] public CameraEffectManager CameraEffectManager;
    [HideInInspector] public PartnerManager PartnerManager;

    public LayerMask MoveLayer {  get { return moveLayer; } }
    public LayerMask IgnoreLayers { get { return ignoreLayers; } }
    public bool CinematicMode { get { return cinematicMode; } }
    public bool VNMode { get { return vnMode; } }
    public bool Ready { get { return ready; } set { ready = value; } }
    public bool End { get { return end; } set { end = value; } }
    public PlayerController Player { get { return player; } }
    public CursorManager CursorManager { get { return cursorManager; } } 


    bool cinematicMode, vnMode, commentMode, end, overrideAmbiance, ready;
    bool cinematicStart;

    private void Awake()
    {
        Instance = this;

        GameObject chara = Instantiate(ChapterData.StartCharacter.ControllerPrefab, characterStart.position, characterStart.rotation);
        character = chara.GetComponentInChildren<Character>();
        player = chara.GetComponentInChildren<PlayerController>();

        player.Init();

        PauseManager.Init(this);
        themeManager.Init();

        PartnerManager = GetComponent<PartnerManager>();
        PartnerManager.Init(this);

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


        foreach(var c in ChapterData.conditions)
        {
            conditions.Add(new Conditions(c.Name, c.Met));
        }

        //areas = ChapterData.areas;
        InventoryManager.Init(ChapterData.items);

        foreach (var item in genericManagers)
        {
            item.Init(this);
        }



        if (StartCinematic != "")
        {
            CinematicManager.PlayCinematic(StartCinematic);

            cinematicStart = true;
        }
        else
        {
            if (startGameManager != null)
            {
                startGameManager.Init(this);

                startPos = player.transform.position;
                startRot = player.transform.rotation;

                startGameManager.StartGame();
            }
            else
                ready = true;
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
        PauseManager.Step();

        if (Paused)
        {
            cursorManager.SetCursorType(CursorType.Base);
            return;
        }

        foreach (var item in genericManagers)
        {
            item.Step();
        }

        if (cinematicMode)
        {
            CinematicManager.Step();
            cursorManager.SetCursorType(CursorType.Invisible);

            foreach (Character c in characters)
            {
                if (c.CanMoveInCinematic)
                {
                    c.Step();
                    c.ConstantStep();
                }
            }
            return;
        }

        if (cinematicStart)
        {
            if (startGameManager != null)
            {
                startGameManager.Init(this);

                startPos = player.transform.position;
                startRot = player.transform.rotation;

                startGameManager.StartGame();

                cinematicStart = false;
            }
            else
                ready = true;
        }

        if (!ready || end)
        {
            if (end)
                cursorManager.SetCursorType(CursorType.Invisible);
            else
                cursorManager.SetCursorType(CursorType.Base);

            return;
        }

        player.ConstantStep();
        PartnerManager.ConstantStep();

        foreach (Character c in characters)
        {
            c.ConstantStep();
        }

        if (startGameManager != null && startGameManager.Intro)
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

        if (SpecialActive)
        {
            if (Input.GetMouseButton(1) && !specialActive)
            {
                player.ToggleSpecial(true);

                specialActive = true;
            }

            if (!Input.GetMouseButton(1) && specialActive)
            {
                player.ToggleSpecial(false);

                specialActive = false;
            }
        }

        player.Step();
        PartnerManager.Step();

        foreach (Character c in characters)
        {
            c.Step();
        }

        CursorHover();
    }

    public void SetPartner(Character character)
    {
        PartnerManager.SetPartner(character);
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
        /*
        if (player.Running)
        {
            if (Time.time - clicktime < clickdelay)
            {

            }
        }
        */

        if (Time.time - clicktime > clickdelay)
            clicked = 0;

        clicked++;
        if (clicked == 1) clicktime = Time.time;

        if (clicked > 1 && Time.time - clicktime < clickdelay)
        {
            //clicked = 0;
            //clicktime = 0;

            clicktime = Time.time;
            return true;
        }
        else if (Time.time - clicktime > clickdelay)
        {
            clicked = 0;
            clicktime = 0;
        }

        return false;
    }

    private void TryClick()
    {
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (player.SpecialMode)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitboxLayer))
            {
                player.Special(hit.point, hit.transform.gameObject);
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayers))
        {
            if (player.SpecialMode)
            {
                player.Special(hit.point, hit.transform.gameObject);
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
        themeManager.NewArea(areaName);
    }

    public void NewArea(string areaName, float volume)
    {
        themeManager.NewArea(areaName, volume);
    }

    public void StopOverride()
    {
        themeManager.StopOverride();
    }

    public void StopOverride(string  areaName)
    {
        themeManager.StopOverride(areaName);
    }

    public void StopAmbiance()
    {
        themeManager.StopAmbiance();
    }

    public void PlayEndAmbiance()
    {
        themeManager.PlayEndAmbiance();
    }

    public void CamZoneQuickUpdate(CameraZone zone)
    {
        currentCamZone = zone;
    }

    public void SetCamZone(CameraZone zone)
    {
        //Safety
        if (currentCamZone == null)
            currentCamZone = firstCamZone;

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

    public void OverrideAmbiance(string overrideArea)
    {
        themeManager.OverrideAmbiance(overrideArea);
    }

    public void SetAmbianceVolume(float sound)
    {
        themeManager.SetAmbianceVolume(sound);
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

        if (PartnerManager.Partner != null)
        {
            maskManager = PartnerManager.Partner.transform.GetComponent<MaskManager>();

            if (maskManager == null)
                PartnerManager.Partner.transform.GetComponentInChildren<MaskManager>();

            if (maskManager != null)
                maskManager.PutMask(on);
        }
    }

    public void ExperimentalDeactivatePlayer()
    {
        player.transform.root.gameObject.SetActive(false);

        if (PartnerManager.Partner != null)
        {
            PartnerManager.Partner.transform.root.gameObject.SetActive(false);
        }
    }
}
