using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaking : MonoBehaviour
{
    [Header("Animation Size")]
    public float animationLoopTime = 1.0f;
    public float posRange = 2.0f;
    public float rotRange = 1.0f;

    [Header("Settings")]
    public bool noZMove = true;
    public bool shouldReturnToStart = false;
    public bool lockRotToZ = false;

    Vector3 initPos, initRot, startPos, startRot, endPos, endRot;
    float timePassed;

    // Use this for initialization
    void Start()
    {
        initPos = startPos = transform.localPosition;
        initRot = startRot = transform.localRotation.eulerAngles;

        SetNewPositions();
    }

    float RandomFromOrderCorrectedRange(float a, float b)
    {
        if (a > b)
        {
            return Random.Range(b, a);
        }
        else
        {
            return Random.Range(a, b);
        }
    }

    void SetNewPositions()
    {
        if (shouldReturnToStart)
        {
            startPos = initPos;
            startRot = initRot;
        }
        else
        {
            startPos = transform.localPosition;
            startRot = transform.localRotation.eulerAngles;
        }

        endPos = new Vector3(
              RandomFromOrderCorrectedRange(initPos.x - posRange, initPos.x + posRange),
              RandomFromOrderCorrectedRange(initPos.y - posRange, initPos.y + posRange),
              noZMove ? 0 : RandomFromOrderCorrectedRange(initPos.z - posRange, initPos.z + posRange)
          );

        endRot = new Vector3(
            lockRotToZ ? 0 : RandomFromOrderCorrectedRange(initRot.x - rotRange, initRot.x + rotRange),
            lockRotToZ ? 0 : RandomFromOrderCorrectedRange(initRot.y - rotRange, initRot.y + rotRange),
            RandomFromOrderCorrectedRange(initRot.z - rotRange, initRot.z + rotRange)
        );
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime / animationLoopTime;

        if (timePassed < 0.5f)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, timePassed * 2f);
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), timePassed * 2f);
        }
        else if (shouldReturnToStart && timePassed < 1.0f)
        {
            // tween back
            float pct2 = (1.0f - timePassed) * 2f;
            transform.localPosition = Vector3.Lerp(startPos, endPos, pct2);
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startRot), Quaternion.Euler(endRot), pct2);

        }
        else if (!shouldReturnToStart || timePassed >= 1.0f)
        {
            timePassed = 0;
            SetNewPositions();
            return;
        }

    }
}