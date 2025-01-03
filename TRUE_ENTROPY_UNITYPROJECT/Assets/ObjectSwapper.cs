using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSwapper : MonoBehaviour
{
    [SerializeField] List<GameObject> OnEnterHide = new List<GameObject>();
    [SerializeField] List<GameObject> OnExitHide = new List<GameObject>();

    private void Awake()
    {
        foreach (var item in OnEnterHide)
        {
            if (!item.activeSelf)
                item.SetActive(true);
        }

        foreach (var item in OnExitHide)
        {
            if (item.activeSelf)
                item.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CameraDetector>() != null)
        {
            foreach (var item in OnEnterHide)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }

            foreach (var item in OnExitHide) 
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CameraDetector>() != null)
        {
            foreach (var item in OnEnterHide)
            {
                if (!item.activeSelf)
                    item.SetActive(true);
            }

            foreach (var item in OnExitHide)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }
        }
    }
}
