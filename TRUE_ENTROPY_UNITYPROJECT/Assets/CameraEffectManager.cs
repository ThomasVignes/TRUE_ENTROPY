using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraEffect
{
    None,
    Shake,
    Bump,
    Rumble
}

public class CameraEffectManager : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource shake, bump, rumble;
    GameManager gm;

    public void Init(GameManager man)
    {
        gm = man;
    }

    public void PlayEffect(CameraEffect effect)
    {
        switch (effect)
        {
            case CameraEffect.None:
                break;

            case CameraEffect.Shake:
                shake.GenerateImpulse();
                break;

            case CameraEffect.Bump:
                bump.GenerateImpulse();
                break;

            case CameraEffect.Rumble:
                rumble.GenerateImpulse();   
                break;
        }
    }
}
