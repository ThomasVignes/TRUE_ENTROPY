using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyAltChapterManager : ChapterManagerGeneric
{
    public bool Skip;
    [SerializeField] private string startCinematic;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
    }

    public override void IntroStep()
    {

    }

    public override void StartGame()
    {
        if (Skip)
        {
            gameManager.Ready = true;
            return;
        }

        StartCoroutine(C_Start());
    }

    IEnumerator C_Start()
    {
        gameManager.ScreenEffects.FadeTo(1, 0.01f);
        AudioListener.volume = 0;

        yield return new WaitForSeconds(2.3f);

        AudioListener.volume = 1;

        gameManager.CinematicManager.PlayCinematic(startCinematic);
        gameManager.Ready = true;
    }

    public override void RestartGame()
    {

    }

    public override void EndChapter()
    {

    }

    public override void Death(string message)
    {

    }
}
