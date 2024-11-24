using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : Manager
{
    [SerializeField] GameObject pauseUI;
    [SerializeField] Slider masterVolume;
    bool canPause;

    float baseScale;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);

        baseScale = Time.timeScale;

        canPause = true;

        if (PersistentData.Instance != null)
            AudioListener.volume = PersistentData.Instance.Volume;
    }

    public override void Step()
    {
        if (!canPause)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape)) 
        { 
            gm.Paused = !gm.Paused;

            pauseUI.SetActive(gm.Paused);

            AudioListener.pause = gm.Paused;

            if (gm.Paused)
                Time.timeScale = 0;
            else
                Time.timeScale = baseScale;
        }
    }

    public void UnPause()
    {
        gm.Paused = false;
        pauseUI.SetActive(false);

        Time.timeScale = baseScale;

        AudioListener.pause = false;
    }

    public void UpdateVolume()
    {
        AudioListener.volume = masterVolume.value;

        if (PersistentData.Instance != null)
            PersistentData.Instance.Volume = AudioListener.volume;
    }

    public void QuitGame()
    {
        UnPause();

        Application.Quit();

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
