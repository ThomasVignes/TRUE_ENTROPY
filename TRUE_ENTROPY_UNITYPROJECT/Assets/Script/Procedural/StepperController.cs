using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StepperController : MonoBehaviour
{
    [SerializeField] LegStepper leftLegStepper;
    [SerializeField] LegStepper rightLegStepper;
    [SerializeField] private Transform leftTarget, rightTarget;

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    private void Start()
    {
        leftTarget.SetParent(null);
        rightTarget.SetParent(null);
    }

    // Only allow diagonal leg pairs to step together
    IEnumerator LegUpdateCoroutine()
    {
        // Run continuously
        while (true)
        {
            // Try moving one diagonal pair of legs
            do
            {
                rightLegStepper.TryMove();
                // Wait a frame
                yield return null;

                // Stay in this loop while either leg is moving.
                // If only one leg in the pair is moving, the calls to TryMove() will let
                // the other leg move if it wants to.
            } while (rightLegStepper.Moving);

            // Do the same thing for the other diagonal pair
            do
            {
                leftLegStepper.TryMove();
                yield return null;
            } while (leftLegStepper.Moving);
        }
    }
}
