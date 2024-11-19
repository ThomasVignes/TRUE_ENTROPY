using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiTrigger : MonoBehaviour
{
    public int NumberBeforeTrigger;
    public UnityEvent Delegates;

    bool done;

    public void Trigger()
    {
        if (done)
            return;

        NumberBeforeTrigger--;

        if (NumberBeforeTrigger <= 0)
        {
            done = true;
            Delegates.Invoke();
        }
    }

    public void Disable()
    {
        done = true;
    }
}
