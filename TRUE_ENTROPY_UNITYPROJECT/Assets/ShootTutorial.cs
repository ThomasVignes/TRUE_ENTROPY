using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootTutorial : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tutorialText;

    bool active;

    public void Activate()
    {
        if (active)
            return;

        active = true;

        tutorialText.DOFade(1, 0.5f);
    }

    private void Update()
    {
        if (!active)
            return;

        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(0))
            {
                EndTutorial();
            }
        }
    }

    public void EndTutorial()
    {
        active = false;

        tutorialText.DOFade(0, 0.2f);
    }
}
