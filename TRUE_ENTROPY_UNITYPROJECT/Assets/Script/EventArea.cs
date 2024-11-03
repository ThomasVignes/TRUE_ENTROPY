using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Whumpus;

public class EventArea : MonoBehaviour
{
    public UnityEvent OnTrigger;

    public LayerMask Layer;

    public GameObject Specific;

    public bool Repeatable, OnExit, StopsPlayer;

    public float Delay;

    private float delayTimer;

    private bool eventStarted;

    private void Update()
    {
        if (eventStarted)
        {
            if (delayTimer > 0)
                delayTimer -= Time.deltaTime;
            else if (delayTimer < 0)
            {
                delayTimer = 0;
            }

            if (delayTimer == 0)
            {
                PlayEvent();
                eventStarted = false;
            }
        }
    }

    public void PlayEvent()
    {
        OnTrigger.Invoke();

        if (StopsPlayer)
        {
            GameManager.Instance.PausePlayerPath();
        }

        if (!Repeatable)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OnExit)
            return;

        if (other.gameObject.layer == WhumpusUtilities.ToLayer(Layer) && !eventStarted)
        {
            eventStarted = true;
            delayTimer = Delay;
        }
        else if (other.gameObject == Specific)
        {
            eventStarted = true;
            delayTimer = Delay;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!OnExit)
            return;

        if (other.gameObject.layer == WhumpusUtilities.ToLayer(Layer) && !eventStarted)
        {
            eventStarted = true;
            delayTimer = Delay;
        }
        else if (other.gameObject == Specific)
        {
            eventStarted = true;
            delayTimer = Delay;
        }
    }
}
