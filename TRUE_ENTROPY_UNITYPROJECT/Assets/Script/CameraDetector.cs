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
        CameraZone cameraZone = other.gameObject.GetComponent<CameraZone>();

        if (other.gameObject.GetComponent<CustomCameraZone>() != null && cameraZone == null)
            cameraZone = other.gameObject.GetComponent<CustomCameraZone>().CameraZone;

        if (cameraZone != null)
        {
            SwitchCam(cameraZone);

            if (cameraZone.ChangeVolume)
                GameManager.Instance.NewArea(cameraZone.Ambiance, cameraZone.NewVolume);
            else
                GameManager.Instance.NewArea(cameraZone.Ambiance);

            ChangedCam?.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        CameraZone cameraZone = other.gameObject.GetComponent<CameraZone>();

        if (other.gameObject.GetComponent<CustomCameraZone>() != null && cameraZone == null)
            cameraZone = other.gameObject.GetComponent<CustomCameraZone>().CameraZone;

        if (cameraZone != null)
        {
            if (CurrentCam == cameraZone)
            {
                LastCamCheck(cameraZone);
            }
        }
    }

    private void SwitchCam(CameraZone camZone)
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

    private void LastCamCheck(CameraZone exitedCamerazone)
    {
        if (LastCam != null)
            CurrentCam = LastCam;

        CurrentCam.active = true;

        if (exitedCamerazone.ChangeVolume)
            GameManager.Instance.NewArea(exitedCamerazone.Ambiance, exitedCamerazone.NewVolume);
        else
            GameManager.Instance.NewArea(exitedCamerazone.Ambiance);

        if (LastCam != null && LastCam != exitedCamerazone)
        {
            LastCam = exitedCamerazone;
            LastCam.active = false;
        }

        GameManager.Instance.SetCamZone(CurrentCam);

        ChangedCam?.Invoke();
    }
}
