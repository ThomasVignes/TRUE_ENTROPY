using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public bool FullScreen;
    public bool SoundOn;
    public bool CopyrightFree;

    public float Volume = 1;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        FullScreen = true;
        SoundOn = true;
    }
}
