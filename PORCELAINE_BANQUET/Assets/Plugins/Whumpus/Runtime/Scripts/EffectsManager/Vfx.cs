using UnityEngine;

[System.Serializable]
public class Vfx 
{
    public string name;

    public GameObject particle;

    [HideInInspector]
    public ParticleSystem source;
}
