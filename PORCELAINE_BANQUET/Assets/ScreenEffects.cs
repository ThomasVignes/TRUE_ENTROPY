using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenEffects : MonoBehaviour
{
    [SerializeField] private Image blackScreen;

    public void SetBlackScreenAlpha(float amount)
    {
        Color color = blackScreen.color;
        color.a = amount;

        blackScreen.color = color;
    }

    public void FadeTo(float amount, float duration)
    {
        blackScreen.DOFade(amount, duration);
    }
}
