using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LongestDayFirstManager : ChapterManagerGeneric
{
    [Header("Settings")]
    public bool Skip;
    [SerializeField] private string startCinematic;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] GameObject endUi;
    [SerializeField] GameObject endTitle;

    [Header("Skip")]
    [SerializeField] private List<GameObject> destroyOnSkip;

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

            foreach (var item in destroyOnSkip)
            {
                Destroy(item);
            }
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
        StartCoroutine(C_Title());
    }

    public override void Death(string message)
    {

    }

    IEnumerator C_Title()
    {
        gameManager.ScreenEffects.SetBlackScreenAlpha(0);

        yield return new WaitForSeconds(4f);

        var t = endText.text;

        endText.text = "";

        endTitle.SetActive(false);

        endUi.SetActive(true);

        foreach (char c in t)
        {
            yield return new WaitForSeconds(0.1f);
            EffectsManager.Instance.audioManager.Play("Click");

            endText.text += c;
        }

        yield return new WaitForSeconds(2.3f);

        endTitle.SetActive(true);
        gameManager.OverrideAmbiance("Empty");

        yield return new WaitForSeconds(6f);

        endUi.SetActive(false);

        yield return new WaitForSeconds(1f);

        gameManager.ScreenEffects.FadeTo(1, 2f);

        yield return new WaitForSeconds(2f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
