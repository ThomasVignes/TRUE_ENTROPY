using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Cinematic")]
public class CinematicData : ScriptableObject
{
    public string ID;
    public float BlackScreenDuration;
    public CinematicLine[] lines;
    public float EndBlackScreenDuration;
}

[System.Serializable]
public class CinematicLine
{
    public string Text;
    public float Duration;
    public PuppetAction[] PuppetActions;
    public int cameraIndex;
    public CameraEffect cameraEffect;
}

[System.Serializable]
public class PuppetAction
{
    public string PuppetName;
    public string Action;
}
