using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraDetector : MonoBehaviour
{
    public UnityEvent ChangedCam;
    [SerializeField] private CameraZone CurrentCam;
    [SerializeField] private CameraZone LastCam;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CameraZone>() != null)
        {
            SwitchCam(other.gameObject.GetComponent<CameraZone>());
            GameManager.Instance.NewArea(other.gameObject.GetComponent<CameraZone>().Ambiance);
            ChangedCam?.Invoke();
        }

        if (other.gameObject.GetComponent<CustomCameraZone>() != null)
        {
            SwitchCam(other.gameObject.GetComponent<CustomCameraZone>().CameraZone);
            GameManager.Instance.NewArea(other.gameObject.GetComponent<CustomCameraZone>().CameraZone.Ambiance);
            ChangedCam?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CameraZone>() != null)
        {
            if (CurrentCam == other.gameObject.GetComponent<CameraZone>())
            {
                LastCamCheck(other.gameObject.GetComponent<CameraZone>());
            }
        }

        if (other.gameObject.GetComponent<CustomCameraZone>() != null)
        {
            if (CurrentCam == other.gameObject.GetComponent<CustomCameraZone>().CameraZone)
            {
                LastCamCheck(other.gameObject.GetComponent<CustomCameraZone>().CameraZone);
            }
        }
    }

    private void SwitchCam(CameraZone camZone)
    {
        if (CurrentCam != camZone)
        {
            if (CurrentCam != null)
            {
                CurrentCam.active = false;
                LastCam = CurrentCam;
            }

            CameraZone thisCameraZone = camZone;
            if (!thisCameraZone.active)
            {
                thisCameraZone.active = true;
                CurrentCam = thisCameraZone;
            }

            GameManager.Instance.SetCamZone(CurrentCam);
        }
    }

    private void LastCamCheck(CameraZone zone)
    {
        if (LastCam != null)
            CurrentCam = LastCam;

        CurrentCam.active = true;
        GameManager.Instance.NewArea(zone.Ambiance);

        if (LastCam != null)
        {
            LastCam = zone;
            LastCam.active = false;
        }

        GameManager.Instance.SetCamZone(LastCam);

        ChangedCam?.Invoke();
    }
}
