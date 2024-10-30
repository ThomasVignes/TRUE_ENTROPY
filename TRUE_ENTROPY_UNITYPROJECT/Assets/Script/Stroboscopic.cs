using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stroboscopic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float flashDelay;
    [SerializeField] float flashDuration;
    [SerializeField] int flashesBeforeMove;
    [SerializeField] float finalDuration;

    [Header("Cams")]
    [SerializeField] GameObject finalCam;
    [SerializeField] GameObject[] orderedCams;

    [Header("References")]
    [SerializeField] GameObject overrideBlackScreen;

    float delayTimer;
    float durationTimer;
    bool active, flashing, final;

    int flashes, moves;
    float finalTimer;

    void Update()
    {
        if (!active)
            return;

        if (final)
        {
            if (finalTimer < Time.time)
            {
                finalCam.SetActive(false);
                final = false;
                active = false;
            }

            return;
        }


        if (delayTimer < Time.time)
        {
            if (!flashing)
            {
                flashing = true;

                if (overrideBlackScreen != null)
                    overrideBlackScreen.SetActive(true);

                foreach (var item in orderedCams)
                {
                    item.SetActive(false);
                }

                finalCam.SetActive(false);

                durationTimer = Time.time + flashDuration;
            }
            else
            {
                if (durationTimer < Time.time)
                {
                    if (overrideBlackScreen != null)
                        overrideBlackScreen.SetActive(false);

                    flashing = false;
                    delayTimer = Time.time + flashDelay;

                    UpdateImage();
                }
            }
        }
    }

    [ContextMenu("Trigger")]
    public void Trigger()
    {
        active = true;

        moves = 0;
        flashes = 0;

        finalCam.SetActive(false);
    }

    void UpdateImage()
    {
        if (flashes < flashesBeforeMove)
        {
            flashes++;

            if (moves < orderedCams.Length)
                orderedCams[moves].SetActive(true);
            else
                finalCam.SetActive(true);

        }
        else
        {
            if (moves < orderedCams.Length)
            {
                orderedCams[moves].SetActive(false);
                moves++;

                if (moves < orderedCams.Length)
                    orderedCams[moves].SetActive(true);
            }
            else
            {
                final = true;
                orderedCams[orderedCams.Length - 1].SetActive(false);

                finalCam.SetActive(true);
                finalTimer = Time.time + finalDuration;
            }

            flashes = 0;
        }
    }
}
