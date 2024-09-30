using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyStartManager : StartGameManagerGeneric
{
    [SerializeField] private float introDelay;

    bool gettingUp;
    float introTimer;

    public override void StartGame()
    {
        StartCoroutine(C_Start());
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

    public override void RestartGame()
    {
        StartCoroutine(C_Restart());
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
}
