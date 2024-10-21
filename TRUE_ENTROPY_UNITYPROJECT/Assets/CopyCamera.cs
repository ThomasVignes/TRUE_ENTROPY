using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyCamera : MonoBehaviour
{
    [SerializeField] Camera cam;

    Camera c;

    private void Awake()
    {
        c = GetComponent<Camera>();
    }

    void Update()
    {
        c.fieldOfView = cam.fieldOfView;
        c.focalLength = cam.focalLength;
        c.nearClipPlane = cam.nearClipPlane;
        c.farClipPlane = cam.farClipPlane;
    }
}
