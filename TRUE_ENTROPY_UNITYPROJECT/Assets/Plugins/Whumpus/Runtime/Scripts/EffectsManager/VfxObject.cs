using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxObject : MonoBehaviour
{
    public ParticleSystem _particleSystem;

    public Action VfxDone;

    public string id;

    public IEnumerator VfxPlayed()
    {
        yield return new WaitForSeconds(_particleSystem.main.duration);

        if (!_particleSystem.main.loop)
        {
            VfxDone?.Invoke();
        }
    }
}
