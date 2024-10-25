using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    [SerializeField] float interval;

    Light Light;
    bool flashing;
    float timer;

    bool down;
    float originalIntensity;

    private void Awake()
    {
        Light = GetComponent<Light>();
        originalIntensity = Light.intensity;
    }

    void Update()
    {
        if (!flashing)
            return;

        if (timer < Time.time)
        {
            if (down)
                Light.intensity = originalIntensity;
            else
                Light.intensity = 0;

            down = !down;
            timer = Time.time + interval;
        }
    }

    [ContextMenu("F")]
    public void F()
    {
        Flash(true);
    }

    public void Flash(bool yes)
    {
        flashing = yes;
    }
}
