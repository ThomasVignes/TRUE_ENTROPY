using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MainMenuMaster : MonoBehaviour
{
    public static MainMenuMaster Instance;

    [Header("States")]
    public bool CanInput;
    public bool AutoQuit;

    [Header("References")]
    [SerializeField] private Image BlackScreen;
    [SerializeField] private Animator hungAnimator, cam;
    [SerializeField] private GameObject FirstMenuButton, FirstOptionButton, Main, OptionsMenu;
    [SerializeField] private GameObject MainCam, OptionsCam, disclaimer, title;
    [SerializeField] private AudioSource click;

    bool disclaimerMode;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BlackFadeTo(1, 0.0001f);
        BlackFadeTo(0, 3f);
        if (EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(FirstMenuButton);

        CanInput = true;

        Screen.fullScreen = true;
        AudioListener.pause = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(5);
        }

        if (Input.anyKey || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (AutoQuit)
            {
                QuitGame();
                return;
            }

            if (!CanInput)
                return;

            click.Play();

            if (!disclaimerMode)
                StartDisclaimer();
            else
                StartGame();
        }


    }

    public void OpenOptions()
    {
        if (CanInput)
        {
            StartCoroutine(ClickCooldown());
            MainCam.SetActive(false);
            OptionsCam.SetActive(true);
            OptionsMenu.SetActive(true);

            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(FirstOptionButton);
        }
    }

    public void ReturnToMain()
    {
        if (CanInput)
        {
            StartCoroutine(ClickCooldown());
            StartCoroutine(DeactivateOptions());
            OptionsCam.SetActive(false);
            MainCam.SetActive(true);

            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);

            EventSystem.current.SetSelectedGameObject(FirstMenuButton);
        }
    }

    public void StartDisclaimer()
    {
        if (CanInput)
        {
            CanInput = false;
            //hungAnimator.SetTrigger("Kidnap");
            StartCoroutine(DisclaimerCoroutine());
        }
    }

    public void StartGame()
    {
        if (CanInput)
        {
            CanInput = false;
            //hungAnimator.SetTrigger("Kidnap");
            StartCoroutine(C_StartGame());
        }
    }


    public void QuitGame()
    {
        if (CanInput)
        {
            CanInput = false;
            StartCoroutine(QuitGameCoroutine());
        }
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        PersistentData.Instance.FullScreen = Screen.fullScreen;
    }

    public void ToggleSound()
    {
        AudioListener.pause = !AudioListener.pause;
        PersistentData.Instance.SoundOn = !AudioListener.pause;
    }

    IEnumerator ClickCooldown()
    {
        CanInput = false;
        yield return new WaitForSeconds(0.68f);

        CanInput = true;
    }

    IEnumerator DeactivateOptions()
    {
        yield return new WaitForSeconds(0.17f);

        OptionsMenu.SetActive(false);
    }

    IEnumerator DisclaimerCoroutine()
    {
        BlackFadeTo(1, 0.7f);
        yield return new WaitForSeconds(1.3f);
        disclaimer.SetActive(true);
        title.SetActive(false);
        BlackFadeTo(0, 0.7f);
        yield return new WaitForSeconds(1);

        disclaimerMode = true;
        CanInput = true;
    }

    IEnumerator C_StartGame()
    {
        BlackFadeTo(1, 0.7f);
        yield return new WaitForSeconds(1.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator QuitGameCoroutine()
    {
        BlackFadeTo(1, 1.3f);
        yield return new WaitForSeconds(1.3f);

        Application.Quit();
    }


    public void BlackFadeTo(int value)
    {
        BlackScreen.DOKill();
        BlackScreen.DOFade(value, 1.3f);
    }

    public void BlackFadeTo(int value, float speed)
    {
        BlackScreen.DOKill();
        BlackScreen.DOFade(value, speed);
    }
}
