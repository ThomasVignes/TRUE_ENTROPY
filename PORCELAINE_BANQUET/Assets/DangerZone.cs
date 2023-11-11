using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Whumpus;

public class DangerZone : MonoBehaviour
{
    [SerializeField] private string deathText;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float timeBeforeDanger, timeBeforeDeath;

    private float timerOne, timerTwo;
    private bool increasing, stop;

    private void Update()
    {
        if (stop)
            return;

        if (!increasing)
            return;

        if (timerOne < timeBeforeDanger)
        {
            timerOne += Time.deltaTime;
        }
        else
        {
            if (timerTwo < timeBeforeDeath)
            {
                timerTwo += Time.deltaTime;
                GameManager.Instance.ScreenEffects.SetBlackScreenAlpha(timerTwo / timeBeforeDeath);
                GameManager.Instance.SetAmbianceVolume(Mathf.Abs(1 - timerTwo / timeBeforeDeath));
            }
            else
            {
                GameManager.Instance.EndGame(deathText);
                stop = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(layer))
        {
            increasing = true;
            GameManager.Instance.SetAmbianceVolume(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == WhumpusUtilities.ToLayer(layer))
        {
            increasing = false;

            timerOne = 0;
            timerTwo = 0;
            GameManager.Instance.ScreenEffects.FadeTo(0, 0.3f);
        }
    }
}
