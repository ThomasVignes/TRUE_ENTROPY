using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAudioSwap : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        if (PersistentData.Instance.CopyrightFree)
        {
            source.clip = clip;
            source.volume = 1;
        }

        source.Play();
    }
}
