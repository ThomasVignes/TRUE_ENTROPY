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
    Elevator,
    PlayerHit
}

public class CameraEffectManager : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource shake, bump, rumble, elevatorShake, elevatorBump, playerHit;
    [SerializeField] bool disableElevatorEffects;
    [SerializeField] Shaking shaker;
    GameManager gm;

    bool elevator;
    float elevatorTimer;
    float multiplier = 1;

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
                if (!disableElevatorEffects)
                    elevatorBump.GenerateImpulse();

                shaker.enabled = false;
                elevator = false;
            }
            else
            {
                if (disableElevatorEffects)
                    return;

                multiplier *= -1;
                elevatorShake.GenerateImpulseWithVelocity(new Vector3(elevatorShake.m_DefaultVelocity.x * multiplier, 
                    elevatorShake.m_DefaultVelocity.y * multiplier, elevatorShake.m_DefaultVelocity.z * multiplier));
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

            case CameraEffect.PlayerHit:
                playerHit.GenerateImpulse();
                break;
        }
    }

    IEnumerator C_Elevator()
    {
        elevatorBump.GenerateImpulse();

        if (disableElevatorEffects)
            shaker.enabled = false;

        yield return new WaitForSeconds(2);

        elevator = true;
        elevatorTimer = Time.time + 10f;
    }

    [ContextMenu("Elevato")]
    public void ElevatorEffect()
    {
        StartCoroutine(C_Elevator());
    }
}
