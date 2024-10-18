using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbChopper : MonoBehaviour
{
    public string ID;
    [SerializeField] GameObject previous, replacement;
    [SerializeField] Animator animator;
    [SerializeField] string chopTrigger;

    public void Chop()
    {
        previous.transform.localScale = Vector3.zero;
        replacement.SetActive(true);

        if (animator != null )
            animator.SetTrigger(chopTrigger);
    }
}
