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
    [SerializeField] Transform[] cameraPivots;
    [SerializeField] TextMeshProUGUI dialogue;

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

        StartCoroutine(C_PlayCinematic());
    }

    IEnumerator C_PlayCinematic()
    {
        Cinematic current = cinematics[currentCinematic];

        gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        Camera.SetActive(true);
        Interface.SetActive(true);

        yield return new WaitForSeconds(current.Data.BlackScreenDuration);

        gameManager.ScreenEffects.FadeTo(0, 1f);

        foreach (var line in current.Data.lines)
        {
            //Write text
            #region Write text
            yield return new WaitForSeconds(delayBeforeDialogue);

            var text = line.Text;

            dialogue.text = "";

            writing = true;

            foreach (char c in text)
            {
                dialogue.text += c;

                if (skip)
                {
                    break;
                }

                yield return new WaitForSeconds(delayBetweenLetters);
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

            //Position camera
            Camera.transform.SetParent(cameraPivots[line.cameraIndex], true);
            Camera.transform.position = cameraPivots[line.cameraIndex].position;
            Camera.transform.rotation = cameraPivots[line.cameraIndex].rotation;

            //Play effects
            gameManager.CameraEffectManager.PlayEffect(line.cameraEffect);

            //Wait delay
            yield return new WaitForSeconds(line.Duration);
        }

        cinematicDone = true;

        gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        Camera.SetActive(false);
        Interface.SetActive(false);

        yield return new WaitForSeconds(current.Data.BlackScreenDuration);
        
        gameManager.ScreenEffects.FadeTo(0, 1f);

        CloseCinematic();
    }

    public void CloseCinematic()
    {
        dialogue.text = "";

        playing = false;
        gameManager.SetCinematicMode(false);

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
    public UnityEvent OnEnd;

    [Header("Scene References")]
    public CinematicPuppet[] CinematicPuppets;
    
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