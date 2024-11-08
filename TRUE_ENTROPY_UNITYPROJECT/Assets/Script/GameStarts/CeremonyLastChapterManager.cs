using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CeremonyLastChapterManager : ChapterManagerGeneric
{
    public string startCinematic;
    [SerializeField] private float introDelay;
    [SerializeField] Stroboscopic stroboscopic;
    bool gettingUp;
    float introTimer;

    public override void StartGame()
    {
        if (startCinematic == "")
            StartCoroutine(C_Start());
        else
            gameManager.CinematicManager.PlayCinematic(startCinematic);
    }
    public override void IntroStep()
    {
        gameManager.CursorManager.SetCursorType(CursorType.Base);

        if (Input.GetMouseButtonDown(0) && !gettingUp)
        {
            gettingUp = true;
            introTimer = Time.time + introDelay;

            gameManager.Player.WakeUp();

            stroboscopic.Trigger();
        }

        if (introTimer < Time.time && gettingUp)
        {
            gettingUp = false;
            Intro = false;

            gameManager.PlayerReady();
        }
    }

    public override void RestartGame()
    {
        StartCoroutine(C_Restart());
    }

    public override void Death(string message)
    {

    }

    public override void EndChapter()
    {
        StartCoroutine(C_EndChapter());
    }


    IEnumerator C_EndChapter()
    {
        gameManager.ScreenEffects.FadeTo(1, 0f);

        yield return new WaitForSeconds(6f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator C_Start()
    {
        gameManager.CursorManager.SetCursorType(CursorType.Base);
        gameManager.ScreenEffects.FadeTo(1, 0.01f);

        yield return new WaitForSeconds(3f);

        RestartGame();
    }

    IEnumerator C_Restart()
    {
        gameManager.ResetPlayer();

        yield return new WaitForSeconds(3f);

        gameManager.ScreenEffects.StartFade();

        yield return new WaitForSeconds(4.2f);

        gameManager.DialogueManager.TryEndDialogue();
        gameManager.SetAmbianceVolume(1f);
        
        Intro = true;
        gameManager.Ready = true;
    }
}
