using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraEffect
{
    None,
    Shake,
    Bump,
    Rumble,
    Elevator
}

public class CameraEffectManager : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource shake, bump, rumble, elevatorShake, elevatorBump;
    [SerializeField] Shaking shaker;
    GameManager gm;

    bool elevator;
    float elevatorTimer;

    public void Init(GameManager man)
    {
        gm = man;
    }

    private void Update()
    {
        if (elevator)
        {
            if (elevatorTimer < Time.time)
            {
                elevatorBump.GenerateImpulse();
                shaker.enabled = false;
                elevator = false;
            }
            else
            {
                elevatorShake.GenerateImpulse();
                elevatorShake.m_DefaultVelocity = new Vector3(elevatorShake.m_DefaultVelocity.x * -1,elevatorShake.m_DefaultVelocity.y * -1, elevatorShake.m_DefaultVelocity.z * -1);
            }
        }
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

            case CameraEffect.Elevator:
                StartCoroutine(C_Elevator());
                break;
        }
    }

    IEnumerator C_Elevator()
    {
        elevatorBump.GenerateImpulse();

        yield return new WaitForSeconds(2);

        elevator = true;
        elevatorTimer = Time.time + 10f;
    }
}
