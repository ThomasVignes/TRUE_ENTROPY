using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuMaster : MonoBehaviour
{
    public static MainMenuMaster Instance;

    [Header("States")]
    public bool CanInput;

    [Header("References")]
    [SerializeField] private Image BlackScreen;
    [SerializeField] private Animator hungAnimator, cam;
    [SerializeField] private GameObject FirstMenuButton, FirstOptionButton, Main, OptionsMenu;
    [SerializeField] private GameObject MainCam, OptionsCam;

    

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
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

    public void StartGame()
    {
        if (CanInput)
        {
            CanInput = false;
            //hungAnimator.SetTrigger("Kidnap");
            StartCoroutine(StartGameCoroutine());
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

    IEnumerator StartGameCoroutine()
    {
        //yield return new WaitForSeconds(0.7f);

        yield return new WaitForSeconds(0.3f);
        BlackFadeTo(1, 1.7f);
        yield return new WaitForSeconds(2.3f);
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
        BlackScreen.DOFade(value, 1.3f);
    }

    public void BlackFadeTo(int value, float speed)
    {
        BlackScreen.DOFade(value, speed);
    }
}
