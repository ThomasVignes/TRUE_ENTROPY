using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelegateBlock : MonoBehaviour
{
    public UnityEvent Delegates;

    public void Trigger()
    {
        Delegates?.Invoke();
    }
}
