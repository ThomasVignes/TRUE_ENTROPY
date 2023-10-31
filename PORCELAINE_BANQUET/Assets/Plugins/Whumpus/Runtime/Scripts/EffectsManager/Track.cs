using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]

public class Track
{
    public string name;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;
}