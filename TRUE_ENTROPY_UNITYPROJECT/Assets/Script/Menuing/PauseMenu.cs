using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public static bool GameIsPaused = false;
    [SerializeField] private Camera renderCam;
    [SerializeField] Transform MenuPivot, OptionsPivot, closedSpot, openSpot;
    [SerializeField] private RawImage MenuRender;
    public GameObject FirstMenuButton, FirstOptionButton;
    private bool Options;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        MenuRender.color = new Color(MenuRender.color.r, MenuRender.color.g, MenuRender.color.b, 0);
        Options = false;
    }

    void Update()
    {
        //EventSystem.current.currentSelectedGameObject.GetComponent<MainMenuButton>().Selected = true;
        
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!GameIsPaused)
            {
                EffectsManager.Instance.audioManager.Play("Click");
                GameIsPaused = true;
                
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstMenuButton);
                
            }
            else
            {
                EffectsManager.Instance.audioManager.Play("Click");
                GameIsPaused = false;
            }
        }

        if (GameIsPaused)
        {
            Time.timeScale = 0f;
            if (Options)
            {
                MenuPivot.position = Vector3.Lerp(MenuPivot.position, closedSpot.position, 0.17f);
                OptionsPivot.position = Vector3.Lerp(OptionsPivot.position, openSpot.position, 0.17f);
            }
            else
            {
                MenuPivot.position = Vector3.Lerp(MenuPivot.position, openSpot.position, 0.17f);
                OptionsPivot.position = Vector3.Lerp(OptionsPivot.position, closedSpot.position, 0.17f);
            }
            Color color = MenuRender.color;
            color = new Color(MenuRender.color.r, MenuRender.color.g, MenuRender.color.b, Mathf.Lerp(color.a, 0.99f, 0.3f));
            MenuRender.color = color;
            renderCam.Render();
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Time.timeScale = 1f;
            MenuPivot.position = Vector3.Lerp(MenuPivot.position, closedSpot.position, 0.17f);
            OptionsPivot.position = Vector3.Lerp(OptionsPivot.position, closedSpot.position, 0.17f);
            Color color = MenuRender.color;
            color = new Color(MenuRender.color.r, MenuRender.color.g, MenuRender.color.b, Mathf.Lerp(color.a, 0, 0.3f));
            MenuRender.color = color;
        }
    }

    public void OnClickResume()
    {
        if (GameIsPaused)
            GameIsPaused = false;
    }

    public void OnClickOptions(bool state)
    {
        if (GameIsPaused)
        {
            Options = state;
            if (state)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstOptionButton);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(FirstMenuButton);
            }
        }
    }

    public void ToggleFullScreen()
    {
        if (GameIsPaused)
        {
            Screen.fullScreen = !Screen.fullScreen;
            PersistentData.Instance.FullScreen = Screen.fullScreen;
        }
    }

    public void ToggleSound()
    {
        if (GameIsPaused)
        {
            AudioListener.pause = !AudioListener.pause;
            PersistentData.Instance.SoundOn = !AudioListener.pause;
        }
    }


    public void OnClickQuit()
    {
        if (GameIsPaused)
        {
            GameIsPaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }
}
