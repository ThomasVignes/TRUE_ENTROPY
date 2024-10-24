using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalCinematic : MonoBehaviour
{
    [Header("Settings")]
    public string ID;
    public string Ambience;
    public LocalCinematicLine[] lines;
    public float delayBeforeDialogue;
    public float delayBetweenLetters;
    public UnityEvent OnStart;
    public UnityEvent OnEndBeforeBlackScreen;
    public UnityEvent OnEnd;

    [Header("Transition Settings")]
    public float StartBlackScreenDuration;
    public float EndBlackScreenDuration;
    public string ResumeTheme;
    public string ChainCinematic;

    [Header("References")]
    public CinematicPuppet[] CinematicPuppets;
    public GameObject Camera;
    public GameObject Interface;

    bool cinematicDone, playing, init;

    [ContextMenu("Play")]
    public void PlayLocal()
    {
        if (playing)
            return;

        if (!init)
        {
            init = true;

            foreach (var item in CinematicPuppets)
            {
                item.Init();
            }
        }


        var gameManager = GameManager.Instance;

        playing = true;
        gameManager.SetCinematicMode(true);

        Interface.SetActive(true);
        Camera.SetActive(true);

        OnStart?.Invoke();

        StartCoroutine(C_PlayCinematic(gameManager, true));
    }

    IEnumerator C_PlayCinematic(GameManager gameManager, bool instaFade)
    {
        LocalCinematic current = this;

        if (current.Ambience != "")
            gameManager.OverrideAmbiance(current.Ambience);

        if (instaFade)
            gameManager.ScreenEffects.FadeTo(1, 0.001f);
        else
            gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(current.StartBlackScreenDuration);

        gameManager.ScreenEffects.FadeTo(0, 1f);

        foreach (var line in current.lines)
        {
            //Write text
            #region Write text
            yield return new WaitForSeconds(delayBeforeDialogue);
            /*
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
            */
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

            //Play delegates
            line.Delegates?.Invoke();

            //Wait delay
            yield return new WaitForSeconds(line.Duration);
        }

        cinematicDone = true;

        if (instaFade)
            gameManager.ScreenEffects.FadeTo(1, 0.001f);
        else
            gameManager.ScreenEffects.FadeTo(1, 0.2f);

        yield return new WaitForSeconds(0.2f);

        Camera.SetActive(false);
        Interface.SetActive(false);

        OnEndBeforeBlackScreen?.Invoke();

        yield return new WaitForSeconds(current.EndBlackScreenDuration);

        gameManager.ScreenEffects.FadeTo(0, 1f);

        CloseCinematic(gameManager);
    }

    public void CloseCinematic(GameManager gameManager)
    {
        //dialogue.text = "";

        OnEnd?.Invoke();

        playing = false;
        gameManager.SetCinematicMode(false);

        if (ResumeTheme == "")
            gameManager.StopOverride();
        else
            gameManager.StopOverride(ResumeTheme);

        if (ChainCinematic != "")
            gameManager.CinematicManager.PlayCinematic(ChainCinematic);
    }

    public void PlayPuppetAction(string puppet, string action)
    {
        CinematicPuppet p = Array.Find(CinematicPuppets, p => p.Name == puppet);

        if (p == null)
            return;

        p.Animator.SetTrigger(action);
    }
}

[System.Serializable]
public class LocalCinematicLine
{
    public string Text;
    public float Duration;
    public PuppetAction[] PuppetActions;
    public CameraEffect cameraEffect;
    public string newAmbience;
    public string[] soundEffects;
    public UnityEvent Delegates;
}
