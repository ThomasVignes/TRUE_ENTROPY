using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CinematicManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] Cinematic[] cinematics;
    [SerializeField] float delayBeforeDialogue;
    [SerializeField] float delayBetweenLetters;

    [Header("References")]
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject Interface;
    [SerializeField] TextMeshProUGUI dialogue;

    [Header("Experimental")]
    [SerializeField] bool lastCinematic;

    GameManager gameManager;

    bool playing, writing, skip, cinematicDone;
    int currentCinematic;

    public void Init(GameManager gm)
    {
        gameManager = gm;

        foreach (var item in cinematics)
        {
            item.Init();
        }

        dialogue.text = "";
    }

    public void Step()
    {
        return; 
        if (Input.GetMouseButtonDown(0))
        {
            /*
            if (writing)
                skip = true;
            */

            if (cinematicDone)
            {
                CloseCinematic();
            }
        }
    }

    public void PlayCinematic(string ID)
    {
        PlayCinematic(ID, false);
    }

    public void PlayCinematic(string ID, bool instaFade)
    {
        if (playing)
            return;

        bool found = true;

        foreach (var c in cinematics)
        {
            if (c.Data.ID == ID)
            {
                currentCinematic = Array.IndexOf(cinematics, c);
                found = true;
                break;
            }
        }

        if (!found)
            return;

        playing = true;
        gameManager.SetCinematicMode(true);
        cinematics[currentCinematic].OnStart?.Invoke();

        StartCoroutine(C_PlayCinematic(instaFade));
    }

    IEnumerator C_PlayCinematic(bool instaFade)
    {
        Cinematic current = cinematics[currentCinematic];

        gameManager.OverrideAmbiance(current.Data.Ambience);

        if (instaFade)
            gameManager.ScreenEffects.FadeTo(1, 0.001f);
        else
            gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        cinematics[currentCinematic].OnStartAfterBlackScreen?.Invoke();

        Camera.SetActive(true);

        //Position camera
        Transform pivot = current.CinematicCameraPivots[current.Data.lines[0].cameraIndex];

        Camera.transform.SetParent(pivot, true);
        Camera.transform.position = pivot.position;
        Camera.transform.rotation = pivot.rotation;

        Interface.SetActive(true);

        yield return new WaitForSeconds(current.Data.BlackScreenDuration);

        gameManager.ScreenEffects.FadeTo(0, 1f);

        foreach (var line in current.Data.lines)
        {        
            //Position camera
            pivot = current.CinematicCameraPivots[line.cameraIndex];

            Camera.transform.SetParent(pivot, true);
            Camera.transform.position = pivot.position;
            Camera.transform.rotation = pivot.rotation;
            //Write text
            #region Write text
            yield return new WaitForSeconds(delayBeforeDialogue);

            var text = line.Text;

            dialogue.text = "";

            writing = true;

            char last = 'a';

            foreach (char c in text)
            {
                dialogue.text += c;

                EffectsManager.Instance.audioManager.Play("SmallClick");

                if (skip)
                {
                    break;
                }

                yield return new WaitForSeconds(delayBetweenLetters);

                /*
                if (c == '.' && last != c)
                    yield return new WaitForSeconds(delayBetweenLetters);
                */

                last = c;
            }

            writing = false;

            if (skip)
            {
                dialogue.text = text;
                skip = false;
            }
            #endregion

            //Play animations
            foreach (var item in line.PuppetActions)
            {
                PlayPuppetAction(item.PuppetName, item.Action);
            }

            //Play effects
            gameManager.CameraEffectManager.PlayEffect(line.cameraEffect);

            foreach (var s in line.soundEffects)
            {
                EffectsManager.Instance.audioManager.Play(s);
            }

            //Wait delay
            yield return new WaitForSeconds(line.Duration);
        }

        cinematicDone = true;

        if (!current.Data.NoEndBlackscreen)
            gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        Camera.SetActive(false);
        Interface.SetActive(false);

        current.OnEndBeforeBlackScreen?.Invoke();

        yield return new WaitForSeconds(current.Data.BlackScreenDuration);

        if (!current.Data.NoFadeOut)
            gameManager.ScreenEffects.FadeTo(0, 1f);

        CloseCinematic();
    }

    public void CloseCinematic()
    {
        dialogue.text = "";

        cinematics[currentCinematic].OnEnd?.Invoke();

        playing = false;
        gameManager.SetCinematicMode(false);

        if (cinematics[currentCinematic].Data.ResumeTheme == "")
            gameManager.StopOverride();
        else
            gameManager.StopOverride(cinematics[currentCinematic].Data.ResumeTheme);

        if (cinematics[currentCinematic].ChainCinematic != "")
            PlayCinematic(cinematics[currentCinematic].ChainCinematic);
    }

    public void PlayPuppetAction(string puppet, string action)
    {
        CinematicPuppet p = Array.Find(cinematics[currentCinematic].CinematicPuppets, p => p.Name == puppet);

        if (p == null)
            return;

        p.Animator.SetTrigger(action);
    }
}



[System.Serializable]
public class Cinematic
{
    [Header("Settings")]
    public CinematicData Data;
    public UnityEvent OnStart;
    public UnityEvent OnStartAfterBlackScreen;
    public UnityEvent OnEndBeforeBlackScreen;
    public UnityEvent OnEnd;
    public string ChainCinematic;

    [Header("Scene References")]
    public CinematicPuppet[] CinematicPuppets;
    public Transform[] CinematicCameraPivots;

    public void Init()
    {
        foreach (var item in CinematicPuppets)
        {
            item.Init();
        }
    }
}

[System.Serializable]
public class CinematicPuppet
{
    public string Name;
    public GameObject Puppet;
    [HideInInspector] public Animator Animator;

    public void Init()
    {
        Animator = Puppet.GetComponent<Animator>();

        if (Animator == null)
            Animator = Puppet.GetComponentInChildren<Animator>();
    }
}