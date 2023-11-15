using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class MainMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent delegates;
    [SerializeField] private float bounceStr;
    [SerializeField] private float actionDelay = 0.2f;
    [SerializeField] private GameObject Indicator;
    public bool Selected, Clicked, DelegatePrep;

    private Vector3 startPos;
    private float delay;
    

    void Awake()
    {
        if (delegates == null)
            delegates = new UnityEvent();
    }

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        Indicator.SetActive(Selected);


        if (Time.realtimeSinceStartup > delay && DelegatePrep)
        {
            DelegatePrep = false;
            delegates.Invoke();
        }
    }

    public void ClickEffects()
    {
        if (!Clicked)
        {
            EffectsManager.Instance.audioManager.Play("Click");
            transform.DOKill();
            transform.localPosition = startPos;
            transform.DOPunchPosition(new Vector3(0, 0, bounceStr), 0.3f, 10, 0, false).SetUpdate(true).OnComplete(() => ClickReset());

            Clicked = true;
            DelegatePrep = true;
            delay = Time.realtimeSinceStartup + actionDelay;
        }
    }

    public void ClickReset()
    {
        Clicked = false;
    }


    public void OnSelect(BaseEventData eventData)
    {
        Selected = true;
        EffectsManager.Instance.audioManager.Play("Hit");
    }

    public void OnDeselect(BaseEventData data)
    {
        Selected = false;
    }
}

