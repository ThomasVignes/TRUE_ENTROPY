using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LongestDaySecondManager : ChapterManagerGeneric
{
    [Header("Settings")]
    public bool Skip;
    [SerializeField] private string startCinematic;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI actText;
    [SerializeField] GameObject titleUI;
    [SerializeField] GameObject titleText;

    [Header("Changes on start")]
    [SerializeField] private List<GameObject> toDestroy;

    [Header("Restart")]
    [SerializeField] private float respawnDelay;
    bool gettingUp;
    float introTimer;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] GameObject deathUI;


    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
    }

    public override void IntroStep()
    {
        gameManager.CursorManager.SetCursorType(CursorType.Base);

        if (Input.GetMouseButtonDown(0) && !gettingUp)
        {
            gettingUp = true;
            introTimer = Time.time + respawnDelay;

            gameManager.Player.WakeUp();
        }

        if (introTimer < Time.time && gettingUp)
        {
            gettingUp = false;
            Intro = false;

            gameManager.WriteComment("My head's killing me. Ugh.");
            gameManager.PlayerReady();
        }
    }

    public override void StartGame()
    {
        if (Skip)
        {
            gameManager.Ready = true;

            foreach (var item in toDestroy)
            {
                Destroy(item);
            }

            gameManager.StopOverride();

            return;
        }

        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        Intro = true;

        gameManager.ScreenEffects.FadeTo(1, 0.01f);

        yield return new WaitForSeconds(2.3f);

        if (startCinematic != "")
            gameManager.CinematicManager.PlayCinematic(startCinematic);

        gameManager.ScreenEffects.FadeTo(0, 4f);

        yield return new WaitForSeconds(2.6f);

        Intro = false;
        gameManager.Ready = true;
    }

    public override void RestartGame()
    {
        StartCoroutine(C_Restart());
    }


    public override void EndChapter()
    {
        StartCoroutine(C_EndChapter());
    }


    IEnumerator C_EndChapter()
    {
        gameManager.ScreenEffects.FadeTo(1, 2.9f);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void Title()
    {
        StartCoroutine(C_Title());
    }

    IEnumerator C_Title()
    {
        gameManager.ScreenEffects.SetBlackScreenAlpha(0);

        yield return new WaitForSeconds(4f);

        var t = actText.text;

        actText.text = "";

        titleText.SetActive(false);

        titleUI.SetActive(true);

        foreach (char c in t)
        {
            yield return new WaitForSeconds(0.1f);
            EffectsManager.Instance.audioManager.Play("Click");

            actText.text += c;
        }

        yield return new WaitForSeconds(2.3f);

        titleText.SetActive(true);
        gameManager.OverrideAmbiance("Empty");

        yield return new WaitForSeconds(6f);

        titleUI.SetActive(false);

        yield return new WaitForSeconds(1f);

        gameManager.ScreenEffects.FadeTo(1, 2f);

        yield return new WaitForSeconds(2f);


        foreach (var item in toDestroy)
        {
            Destroy(item);
        }

        gameManager.HidePlayer(false);

        yield return new WaitForSeconds(2f);

        gameManager.StopOverride();
        gameManager.ScreenEffects.FadeTo(0, 0.8f);

        gameManager.Ready = true;

        yield return new WaitForSeconds(0.8f);

        gameManager.CursorManager.SetCursorType(CursorType.Base);
    }

    public override void Death(string message)
    {
        gameManager.End = true;

        StartCoroutine(C_DeathCinematic(message));
    }

    IEnumerator C_DeathCinematic(string message)
    {
        gameManager.SetAmbianceVolume(0f);
        gameManager.ScreenEffects.FadeTo(1, 0.3f);

        yield return new WaitForSeconds(1.4f);
        deathUI.SetActive(true);

        deathText.text = "";

        foreach (char c in message)
        {
            yield return new WaitForSeconds(0.06f);
            EffectsManager.Instance.audioManager.Play("Click");

            deathText.text += c;
        }

        yield return new WaitForSeconds(2.3f);

        deathText.text = "";
        EffectsManager.Instance.audioManager.Play("Gunshot");

        deathUI.SetActive(false);


        gameManager.ResetPlayer();

        gameManager.End = false;

        RestartGame();
    }

    IEnumerator C_Restart()
    {
        yield return new WaitForSeconds(3f);

        gameManager.DialogueManager.TryEndDialogue();
        gameManager.SetAmbianceVolume(1f);
        gameManager.ScreenEffects.StartFade();

        gameManager.Ready = true;

        Intro = true;
    }
}
