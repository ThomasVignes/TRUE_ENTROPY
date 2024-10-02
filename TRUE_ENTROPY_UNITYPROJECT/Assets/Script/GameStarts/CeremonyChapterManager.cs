using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CeremonyChapterManager : ChapterManagerGeneric
{
    [SerializeField] private float introDelay;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] private TextMeshProUGUI endText;
    bool gettingUp;
    float introTimer;

    public override void StartGame()
    {
        endText.text = "";

        StartCoroutine(C_Start());
    }
    public override void IntroStep()
    {
        gameManager.CursorManager.SetCursorType(CursorType.Base);

        if (Input.GetMouseButtonDown(0) && !gettingUp)
        {
            gettingUp = true;
            introTimer = Time.time + introDelay;

            gameManager.Player.WakeUp();
            gameManager.Player.Injure(true);
        }

        if (introTimer < Time.time && gettingUp)
        {
            gettingUp = false;
            Intro = false;

            gameManager.WriteComment("Ugh. My head.");
            gameManager.PlayerReady();
        }
    }

    public override void RestartGame()
    {
        StartCoroutine(C_Restart());
    }

    public override void Death(string message)
    {
        gameManager.End = true;

        StartCoroutine(C_DeathCinematic(message));
    }

    public override void EndChapter()
    {
        StartCoroutine(C_EndChapter());
    }

    #region Coroutines
    IEnumerator C_EndChapter()
    {
        gameManager.ScreenEffects.FadeTo(1, 2.9f);

        yield return new WaitForSeconds(5);

        gameManager.ScreenEffects.OumuamuaFade();

        yield return new WaitForSeconds(5);

        gameManager.OverrideAudio.Stop();

        gameManager.WriteComment("THERE IS NO ESCAPE.");

        yield return new WaitForSeconds(4);

        Application.Quit();
    }
    
    IEnumerator C_Start()
    {
        gameManager.ScreenEffects.FadeTo(1, 0.01f);
        AudioListener.volume = 0;

        yield return new WaitForSeconds(2.3f);

        EffectsManager.Instance.audioManager.Play("Gunshot");

        AudioListener.volume = 1;

        RestartGame();
    }

    IEnumerator C_DeathCinematic(string message)
    {
        gameManager.SetAmbianceVolume(0f);
        gameManager.ScreenEffects.FadeTo(1, 0.3f);

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
        go.transform.position = gameManager.Player.transform.position;
        go.transform.rotation = gameManager.Player.transform.rotation;

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
        Intro = true;
        gameManager.Ready = true;
    }
    #endregion
}
