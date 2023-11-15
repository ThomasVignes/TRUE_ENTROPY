using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    private static EffectsManager _instance;

    public AudioManager audioManager;
    public VFXManager vfxManager;

    public static EffectsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EffectsManager>();
            }

            return _instance;
        }
    }
}
